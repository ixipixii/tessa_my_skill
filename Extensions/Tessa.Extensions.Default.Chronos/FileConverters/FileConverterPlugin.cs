﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Chronos.Contracts;
using NLog;
using Tessa.Cards;
using Tessa.FileConverters;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.IO;
using Tessa.Platform.Operations;
using Tessa.Platform.Runtime;
using Tessa.Platform.Scopes;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Unity;
using Unity.Lifetime;

namespace Tessa.Extensions.Default.Chronos.FileConverters
{
    /// <summary>
    /// Делает преобразование файлов, сохраняет их в карточку кэша
    /// </summary>
    [Plugin(
        Name = "File converter plugin",
        Description = "Convert files to specific formats and stores them to the cache card",
        Version = 1,
        ConfigFile = ConfigFilePath)]
    public sealed class FileConverterPlugin :
        Plugin
    {
        #region Fields

        private IDbScope dbScope;

        private IOperationRepository operationRepository;

        private IErrorManager errorManager;

        private IExtensionContainer extensionContainer;

        private IFileConverterComposer fileConverterComposer;

        private IFileConverterCache fileConverterCache;

        private IFileConverterWorker fileConverterWorker;

        private ICardStreamServerRepository cardStreamServerRepository;

        private ICardServerPermissionsProvider permissionsProvider;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static readonly string tempPathBase = Path.Combine(FileHelper.GetPath(FileSpecialFolder.TempBase), "FileConverter");

        private static readonly string tempPathInput = Path.Combine(tempPathBase, "input");

        private static readonly string tempPathOutput = Path.Combine(tempPathBase, "output");

        #endregion

        #region Constants

        /// <summary>
        /// Относительный путь к конфигурационному файлу плагина.
        /// </summary>
        private const string ConfigFilePath = "configuration/FileConverter.xml";

        private const string CacheCleanPeriodPropertyName = "FileConverter.CacheCleanPeriod";

        private const string OldestPreviewFilePeriodPropertyName = "FileConverter.OldestPreviewFilePeriod";

        private const string MaintenancePeriodPropertyName = "FileConverter.MaintenancePeriod";


        private static TimeSpan cacheCleanPeriod;

        private static TimeSpan CacheCleanPeriod
        {
            get
            {
                if (cacheCleanPeriod == default)
                {
                    cacheCleanPeriod = TimeSpan.Parse(ConfigurationManager.Settings.Get<string>(CacheCleanPeriodPropertyName));
                }

                return cacheCleanPeriod;
            }
        }


        private static TimeSpan oldestPreviewFilePeriod;

        private static TimeSpan OldestPreviewFilePeriod
        {
            get
            {
                if (oldestPreviewFilePeriod == default)
                {
                    oldestPreviewFilePeriod = TimeSpan.Parse(ConfigurationManager.Settings.Get<string>(OldestPreviewFilePeriodPropertyName));
                }

                return oldestPreviewFilePeriod;
            }
        }


        private static TimeSpan maintenancePeriod;

        private static TimeSpan MaintenancePeriod
        {
            get
            {
                if (maintenancePeriod == default)
                {
                    maintenancePeriod = TimeSpan.Parse(ConfigurationManager.Settings.Get<string>(MaintenancePeriodPropertyName));
                }

                return maintenancePeriod;
            }
        }

        #endregion

        #region Private Members

        private async Task PerformOperationCycleAsync(CancellationToken cancellationToken = default)
        {
            // очистка кэша будет запущена сразу же после старта плагина
            // обслуживание (например, перезапуск процесса Office) будет выполняться периодически, но не сразу же

            DateTime utcNow = DateTime.UtcNow;
            DateTime lastCacheCleanDate = DateTime.MinValue;
            DateTime lastMaintenanceDate = utcNow;

            lastCacheCleanDate = await this.CheckCleanCacheAsync(utcNow, lastCacheCleanDate, cancellationToken);

            IDbScopeInstance dbScopeInstance = null;
            int dbCounter = 0;
            const int maxDbCounter = 10;

            try
            {
                dbScopeInstance = this.dbScope.Create();

                while (true)
                {
                    if (this.StopRequested)
                    {
                        return;
                    }

                    utcNow = DateTime.UtcNow;
                    bool wait = false;
                    bool recoveryAfterError = false;

                    if (dbCounter++ >= maxDbCounter)
                    {
                        dbCounter = 0;
                        await dbScopeInstance.DisposeAsync();

                        dbScopeInstance = this.dbScope.Create();
                    }

                    bool processed;

                    try
                    {
                        processed = await this.ProcessOperationAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogException(ex);

                        processed = false;
                        wait = true;
                        recoveryAfterError = true;
                    }

                    if (processed)
                    {
                        lastCacheCleanDate = await this.CheckCleanCacheAsync(utcNow, lastCacheCleanDate, cancellationToken);
                    }
                    else
                    {
                        bool performed;
                        (performed, lastMaintenanceDate) = await this.CheckPerformMaintenanceAsync(utcNow, lastMaintenanceDate, cancellationToken);

                        if (!performed)
                        {
                            wait = true;
                        }
                    }

                    if (wait)
                    {
                        if (recoveryAfterError)
                        {
                            // возвращаем коннекшен в пул, поскольку ждать будем долго
                            dbCounter = 0;
                            await dbScopeInstance.DisposeAsync();

                            dbScopeInstance = this.dbScope.Create();
                        }

                        int waitCount = recoveryAfterError ? 60 : 1;
                        for (int i = 0; i < waitCount; i++)
                        {
                            if (this.StopRequested)
                            {
                                // в случае ожидания после ошибки - сюда будем часто попадать, и при вежливой остановке плагина надо скорее завершаться
                                return;
                            }

                            await Task.Delay(1000, cancellationToken);

                            if (this.StopRequested)
                            {
                                // в случае ожидания после ошибки - сюда будем часто попадать, и при вежливой остановке плагина надо скорее завершаться
                                return;
                            }
                        }
                    }
                    else
                    {
                        // на следующей итерации пересоздаём dbScope
                        dbCounter = maxDbCounter;
                    }
                }
            }
            finally
            {
                if (dbScopeInstance != null)
                {
                    await dbScopeInstance.DisposeAsync();
                }
            }
        }


        private async Task<(bool performed, DateTime lastMaintenanceDate)> CheckPerformMaintenanceAsync(
            DateTime utcNow,
            DateTime lastMaintenanceDate,
            CancellationToken cancellationToken = default)
        {
            if (utcNow.Subtract(lastMaintenanceDate) <= MaintenancePeriod)
            {
                return (false, lastMaintenanceDate);
            }

            DateTime resultingDateTime;

            try
            {
                logger.Info("Performing maintenance: started");
                await this.fileConverterWorker.PerformMaintenanceAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogException(ex, LogLevel.Warn);
            }
            finally
            {
                resultingDateTime = utcNow;
                logger.Info("Performing maintenance: completed");
            }

            return (true, resultingDateTime);
        }


        private async Task<DateTime> CheckCleanCacheAsync(DateTime utcNow, DateTime lastCacheCleanDate, CancellationToken cancellationToken = default)
        {
            DateTime resultingDate = lastCacheCleanDate;

            if (utcNow.Subtract(lastCacheCleanDate) > CacheCleanPeriod)
            {
                try
                {
                    logger.Info("Cache cleaning: started");

                    ValidationResult cleanResult = await this.fileConverterCache
                        .CleanCacheAsync(utcNow.Subtract(OldestPreviewFilePeriod), cancellationToken);

                    logger.LogResult(cleanResult);
                }
                catch (Exception ex)
                {
                    logger.LogException(ex, LogLevel.Warn);
                }
                finally
                {
                    resultingDate = utcNow;
                    logger.Info("Cache cleaning: completed");
                }
            }

            return resultingDate;
        }


        private async Task<bool> ProcessOperationAsync(CancellationToken cancellationToken = default)
        {
            Guid? operationID = await this.operationRepository.StartFirstAsync(OperationTypes.FileConvert, cancellationToken);
            if (!operationID.HasValue)
            {
                return false;
            }

            string inputFilePath = null;
            string outputFilePath = null;
            IFileConverterRequest request = null;

            try
            {
                // получаем параметры операции
                IOperation operation = await this.operationRepository.TryGetAsync(operationID.Value, cancellationToken: cancellationToken);
                if (operation == null)
                {
                    return false;
                }

                request = new FileConverterRequest();
                request.Deserialize(operation.Request.Info);

                var validationResult = new ValidationResultBuilder();

                if (this.fileConverterWorker is IFileConverterAggregateWorker aggregateWorker
                    && !aggregateWorker.IsRegistered(request.OutputFormat))
                {
                    validationResult.AddError(this,
                        "Unsupported output format \"{0}\" while trying to convert file \"{1}\".",
                        request.OutputFormat, request.FileName);

                    await this.CompleteWithErrorAsync(operationID.Value, request, validationResult, cancellationToken);
                    return false;
                }

                if (!FileConverterFormat.IsSupportedConversion(request.OutputFormat, request.InputFormat))
                {
                    validationResult.AddError(this,
                        "Unsupported input format \"{0}\" while trying to convert file \"{1}\".",
                        request.InputFormat, request.FileName);

                    await this.CompleteWithErrorAsync(operationID.Value, request, validationResult, cancellationToken);
                    return false;
                }

                logger.Info("Converting file '{0}': cardID='{1}', fileID='{2}'", request.FileName, request.CardID, request.FileID);

                // получаем файл из карточки
                var fileRequest = new CardGetFileContentRequest
                {
                    CardID = request.CardID,
                    FileID = request.FileID,
                    VersionRowID = request.VersionID,
                    FileName = request.FileName,
                    FileTypeID = request.FileTypeID,
                    FileTypeName = request.FileTypeName,
                    CardTypeID = request.CardTypeID,
                    CardTypeName = request.CardTypeName,
                };

                if (!request.FileRequestInfo.IsEmpty())
                {
                    fileRequest.Info = StorageHelper.Clone(request.FileRequestInfo.GetStorage());
                }

                // в момент открытия файла нам мог прийти или не прийти токен, здесь мы его выбрасываем и устанавливаем пермишены "всегда можно"
                this.permissionsProvider.SetFullPermissions(fileRequest);

                ICardFileContentResult fileResult = await this.cardStreamServerRepository.GetFileContentAsync(fileRequest, cancellationToken);
                CardGetFileContentResponse fileResponse = fileResult.Response;

                ValidationResult result = fileResponse.ValidationResult.Build();
                validationResult.Add(result);

                if (!result.IsSuccessful || !fileResult.HasContent)
                {
                    await this.CompleteWithErrorAsync(operationID.Value, request, validationResult, cancellationToken);
                    return false;
                }

                // копируем файл во временную папку
                inputFilePath = Path.Combine(tempPathInput, request.VersionID.ToString());
                CreateDirectorySafe(tempPathInput);

                await using (Stream inputFileStream = File.Create(inputFilePath))
                {
                    await using Stream contentStream = await fileResult.GetContentOrThrowAsync(cancellationToken);
                    await contentStream.CopyToAsync(inputFileStream, cancellationToken);
                }

                // преобразуем файл во временную папку
                Guid convertedFileID = Guid.NewGuid();
                outputFilePath = Path.Combine(tempPathOutput, convertedFileID.ToString());
                CreateDirectorySafe(tempPathOutput);

                // преобразуем файл с расширениями
                var context = new FileConverterContext(inputFilePath, outputFilePath, request, operation, cancellationToken);

                IDbScopeInstance dbScopeInstance = null;
                IExtensionExecutor<IFileConverterExtension> executor = null;
                IInheritableScopeInstance<IScopeHolderContext> scopeInstance = null;

                try
                {
                    // расширения на конвертацию и fileConverterWorker будут выполняться в своём соединении, которое не зависит от внешнего цикла операций
                    dbScopeInstance = this.dbScope.CreateNew();
                    executor = await this.extensionContainer.ResolveExecutorAsync<IFileConverterExtension>(cancellationToken);
                    scopeInstance = ScopeHolderContext.Create();

                    try
                    {
                        await executor.ExecuteAsync(x => x.BeforeRequest, context);
                    }
                    catch (Exception ex)
                    {
                        context.ValidationResult.AddException(this, ex);
                    }

                    context.RequestIsSuccessful = context.ValidationResult.IsSuccessful();

                    if (context.RequestIsSuccessful)
                    {
                        try
                        {
                            await this.fileConverterWorker.ConvertFileAsync(context, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            context.ValidationResult.AddException(this, ex);
                        }
                    }

                    try
                    {
                        await executor.ExecuteAsync(x => x.AfterRequest, context);
                    }
                    catch (Exception ex)
                    {
                        context.ValidationResult.AddException(this, ex);
                    }
                }
                finally
                {
                    scopeInstance?.Dispose();

                    if (executor != null)
                    {
                        await executor.DisposeAsync();
                    }

                    if (dbScopeInstance != null)
                    {
                        await dbScopeInstance.DisposeAsync();
                    }
                }

                validationResult.Add(context.ValidationResult);

                if (!context.ValidationResult.IsSuccessful())
                {
                    await this.CompleteWithErrorAsync(operationID.Value, request, validationResult, cancellationToken);
                    return false;
                }

                outputFilePath = context.OutputFilePath;

                // сохраняем файл в кеш с заданным идентификатором
                if (request.Flags.HasNot(FileConverterRequestFlags.DoNotCacheResult))
                {
                    byte[] requestHash = this.fileConverterComposer.CalculateHash(request);

                    ValidationResult conversionResult = await this.fileConverterCache
                        .StoreFileAsync(
                            versionID: request.VersionID,
                            requestHash: requestHash,
                            fileID: convertedFileID,
                            fileName: FileHelper.GetFileNameWithoutExtension(request.FileName, ignoreFolder: true) + "." + context.OutputExtension,
                            contentFilePath: outputFilePath,
                            responseInfo: context.ResponseInfo.GetStorage(),
                            cancellationToken: cancellationToken);

                    validationResult.Add(conversionResult);
                    if (!validationResult.IsSuccessful())
                    {
                        await this.CompleteWithErrorAsync(operationID.Value, request, validationResult, cancellationToken);
                        return false;
                    }
                }

                // завершаем операцию, указывая идентификатор файла в кэше
                await this.CompleteSuccessfulAsync(operationID.Value, request, convertedFileID, context, validationResult, cancellationToken);

                logger.Info("File has been converted '{0}': cardID='{1}', fileID='{2}'", request.FileName, request.CardID, request.FileID);
                return true;
            }
            catch (Exception ex)
            {
                await this.CompleteWithUnhandledExceptionAsync(operationID.Value, request, ex, cancellationToken);
                return false;
            }
            finally
            {
                DeleteFileSafe(inputFilePath);
                DeleteFileSafe(outputFilePath);
            }
        }


        private Task CompleteSuccessfulAsync(
            Guid operationID,
            IFileConverterRequest request,
            Guid convertedFileID,
            IFileConverterContext context,
            IValidationResultBuilder validationResult,
            CancellationToken cancellationToken = default)
        {
            if (request != null && request.Flags.Has(FileConverterRequestFlags.WithoutResponse))
            {
                return this.DeleteOperationSafeAsync(operationID, cancellationToken);
            }

            var response = new OperationResponse();
            response.ValidationResult.Add(validationResult);

            Dictionary<string, object> responseInfo = response.Info;
            responseInfo["FileID"] = convertedFileID;
            responseInfo["ResponseInfo"] = StorageHelper.Clone(context.ResponseInfo.GetStorage());

            return this.operationRepository.CompleteAsync(operationID, response, cancellationToken);
        }


        private async Task CompleteWithErrorAsync(
            Guid operationID,
            IFileConverterRequest request,
            IValidationResultBuilder validationResult,
            CancellationToken cancellationToken = default)
        {
            logger.Error(
                "Error converting file '{0}'. cardID='{1}', fileID='{2}'.{3}{4}",
                request.FileName,
                request.CardID,
                request.FileID,
                Environment.NewLine,
                validationResult.Build().ToString(ValidationLevel.Detailed));

            await this.ReportErrorSafeAsync(validationResult.Build(), request, cancellationToken);

            if (request.Flags.Has(FileConverterRequestFlags.WithoutResponse))
            {
                await this.DeleteOperationSafeAsync(operationID, cancellationToken);
            }

            var response = new OperationResponse();
            response.ValidationResult.Add(validationResult);

            await this.operationRepository.CompleteAsync(operationID, response, cancellationToken);
        }


        private async Task CompleteWithUnhandledExceptionAsync(
            Guid operationID,
            IFileConverterRequest request,
            Exception ex,
            CancellationToken cancellationToken = default)
        {
            // ErrorException уже записывает ошибку в лог, не будем её дублировать

            var response = new OperationResponse();
            response.ValidationResult.AddException(this, ex);

            await this.ReportErrorSafeAsync(response.ValidationResult.Build(), request, cancellationToken);

            if (request != null && request.Flags.Has(FileConverterRequestFlags.WithoutResponse))
            {
                await this.DeleteOperationSafeAsync(operationID, cancellationToken);
            }

            await this.operationRepository.CompleteAsync(operationID, response, cancellationToken);
        }


        private Task ReportErrorSafeAsync(
            ValidationResult result,
            IFileConverterRequest request,
            CancellationToken cancellationToken = default)
        {
            Guid cardID;
            string cardName;
            if (request != null)
            {
                cardID = request.CardID;
                cardName = request.EventName;
            }
            else
            {
                cardID = FileConverterHelper.CacheCardID;
                cardName = "Unhandled exception";
            }

            return this.errorManager.ReportErrorSafeAsync(
                CardHelper.FileConverterCacheTypeID,
                cardID,
                cardName,
                new ErrorDescription(result, ErrorCategories.FileConverterFailed),
                cancellationToken: cancellationToken);
        }


        private async Task DeleteOperationSafeAsync(Guid operationID, CancellationToken cancellationToken = default)
        {
            try
            {
                await this.operationRepository.DeleteAsync(operationID, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogException(
                    $"Error during removing file conversion operation ID={operationID:B}:",
                    ex, LogLevel.Warn);
            }
        }


        private static void DeleteFileSafe(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex, LogLevel.Warn);
            }
        }


        private static void CreateDirectorySafe(string folder)
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex, LogLevel.Warn);
            }
        }

        #endregion

        #region Base Overrides

        public override async Task EntryPointAsync(CancellationToken cancellationToken = default)
        {
            logger.Info("Starting plugin");

            try
            {
                TessaPlatform.InitializeFromConfiguration();

                // поскольку в расширениях может быть переопределён FileConverterFormat.Pdf, то здесь корректно сначала вызвать
                // RegisterServerForPluginAsync, и потом уже RegisterWorker
                IUnityContainer container = (await new UnityContainer().RegisterServerForPluginAsync())

                        // регистрацию всех известных в типовом решении Worker-ов и их зависимостей выполняем здесь;

                        // если необходимо создать Worker в рамках проекта, то его можно указать в библиотеке расширений,
                        // например, в Tessa.Extensions.Server.dll, и в методе Registrator.FinalizeRegistration
                        // выполнить такую же регистрацию;

                        // если в одном из расширений уже зарегистрированы Worker-ы для этих форматов,
                        // то регистрация ниже их не перезапишет, т.к. мы не указали overwrite
                        .RegisterWorker(
                            FileConverterFormat.Pdf,
                            c => new PdfFileConverterWorker(
                                c.Resolve<Func<IProcessManager>>(),
                                c.Resolve<IFileConverterWorker>(FileConverterWorkerNames.TiffToPdf)))
                    ;

                if (!container.IsRegistered<IFileConverterWorker>(FileConverterWorkerNames.TiffToPdf))
                {
                    container
                        .RegisterType<IFileConverterWorker, TiffToPdfFileConverterWorker>(
                            FileConverterWorkerNames.TiffToPdf,
                            new ContainerControlledLifetimeManager())
                        ;
                }

                this.fileConverterWorker = container.TryResolve<IFileConverterWorker>();
                if (this.fileConverterWorker != null)
                {
                    this.dbScope = container.Resolve<IDbScope>();
                    this.operationRepository = container.Resolve<IOperationRepository>();
                    this.errorManager = container.Resolve<IErrorManager>();
                    this.extensionContainer = container.Resolve<IExtensionContainer>();
                    this.fileConverterComposer = container.Resolve<IFileConverterComposer>();
                    this.fileConverterCache = container.Resolve<IFileConverterCache>();
                    this.cardStreamServerRepository = container.Resolve<ICardStreamServerRepository>();
                    this.permissionsProvider = container.Resolve<ICardServerPermissionsProvider>();

                    await this.fileConverterWorker.PreprocessAsync(cancellationToken);
                    await this.PerformOperationCycleAsync(cancellationToken);
                }
                else
                {
                    logger.Error(
                        "Can't find registration for {0}. Check if default extensions are registered.",
                        typeof(IFileConverterWorker).FullName);
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
            finally
            {
                switch (this.fileConverterWorker)
                {
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync();
                        break;

                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }

            logger.Info("File converter: shutdown completed");
        }


        public override Task StopAsync(IGracefulStopToken token)
        {
            logger.Info("File converter: shutting down");
            return base.StopAsync(token);
        }

        #endregion
    }
}