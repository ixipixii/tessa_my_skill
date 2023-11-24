﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Tessa.FileConverters;
using Tessa.Platform;
using Tessa.Platform.IO;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Properties.Resharper;

namespace Tessa.Extensions.Default.Chronos.FileConverters
{
    /// <summary>
    /// Объект, ответственный за преобразование файла в формат <see cref="FileConverterFormat.Pdf"/>
    /// посредством внешних программ, таких как OpenOffice или LibreOffice.
    /// </summary>
    /// <remarks>
    /// Наследники класса могут переопределять методы интерфейса, например, добавив к ним обработку файлов других форматов.
    /// Класс может также реализовывать <see cref="IAsyncDisposable"/> для очистки ресурсов,
    /// для этого в наследнике переопределяется метод <see cref="DisposeAsync"/> и вызывается сначала его базовая реализация.
    /// </remarks>
    public class PdfFileConverterWorker :
        IFileConverterWorker,
        IAsyncDisposable
    {
        #region Constructors

        /// <summary>
        /// Создаёт экземпляр класса с указанием его зависимостей.
        /// </summary>
        /// <param name="createProcessManagerFunc">Объект для запуска дочерних процессов.</param>
        /// <param name="tiffToPdfWorker">
        /// Объект для конвертации из TIFF в PDF или <c>null</c>, если такая конвертация не поддерживается.
        /// </param>
        public PdfFileConverterWorker(
            Func<IProcessManager> createProcessManagerFunc,
            IFileConverterWorker tiffToPdfWorker = null)
        {
            Check.ArgumentNotNull(createProcessManagerFunc, "createProcessManagerFunc");

            this.processManagerLazy = new Lazy<IProcessManager>(
                createProcessManagerFunc,
                LazyThreadSafetyMode.ExecutionAndPublication);

            this.tiffToPdfWorker = tiffToPdfWorker;
        }

        #endregion

        #region Fields

        [CanBeNull]
        private readonly IFileConverterWorker tiffToPdfWorker;

        [CanBeNull]
        private Lazy<IProcessManager> processManagerLazy;

        /// <summary>
        /// Текст ошибки, которая должна выбрасываться при попытке конвертировать файл PDF через OpenOffice/LibreOffice,
        /// или <c>null</c>, если ошибок нет и конвертацию можно выполнять.
        /// </summary>
        private string openOfficeErrorText; // = null

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Environment variables

        private const string UnoconvExternalCommandSettingName = "UnoconvExternalCommand";


        private static string unoconvExternalCommand;

        /// <summary>
        /// Путь к внешнему исполняемому файлу "unoconv", который будет использоваться вместо OpenOffice.
        /// </summary>
        private static string UnoconvExternalCommand =>
            unoconvExternalCommand ??= Environment.ExpandEnvironmentVariables(
                    ConfigurationManager.Settings.TryGet<string>(UnoconvExternalCommandSettingName) ??
                    string.Empty)
                .Trim();


        private const string OpenOfficePythonSettingName = "OpenOfficePython";


        private static string openOfficePython;

        /// <summary>
        /// Путь к исполняемому файлу "OpenOffice".
        /// </summary>
        private static string OpenOfficePython
        {
            get
            {
                InitializeOpenOfficePythonPath();
                return openOfficePython;
            }
        }


        private static string openOfficePythonParamsPrefix;

        /// <summary>
        /// Дополнительные параметры, передаваемые в команду "OpenOffice" перед параметрами unoconv.
        /// Равны либо пустой строке (нет параметров), либо параметрам плюс завершающий пробел.
        /// </summary>
        private static string OpenOfficePythonParamsPrefix
        {
            get
            {
                InitializeOpenOfficePythonPath();
                return openOfficePythonParamsPrefix;
            }
        }


        private static bool openOfficePythonPathInitialized; // = false

        private static void InitializeOpenOfficePythonPath()
        {
            if (openOfficePythonPathInitialized)
            {
                return;
            }

            openOfficePythonPathInitialized = true;

            string command = Environment.ExpandEnvironmentVariables(
                    ConfigurationManager.Settings.TryGet<string>(OpenOfficePythonSettingName) ?? string.Empty)
                .Trim();

            int separatorIndex = command.IndexOf('|');
            if (separatorIndex <= 0 || separatorIndex == command.Length - 1)
            {
                openOfficePython = command;
                openOfficePythonParamsPrefix = string.Empty;
            }
            else
            {
                openOfficePython = command.Substring(0, separatorIndex).Trim();
                openOfficePythonParamsPrefix = command.Substring(separatorIndex + 1).Trim() + " ";
            }
        }

        #endregion

        #region UnoconvPath Private Property

        private static string unoconvPath;

        private static string UnoconvPath => unoconvPath ??= GetUnoconvPath();

        private static string GetUnoconvPath()
        {
            // location - путь до сборки Tessa.dll, в этой же папке должен лежать unoconv
            string location = typeof(BuildInfo).Assembly.GetActualLocationFolder();
            return string.IsNullOrEmpty(location) ? string.Empty : Path.Combine(location, "unoconv");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Завершаем экземпляр OpenOffice или LibreOffice в этой сессии.
        /// </summary>
        private static void KillSessionOfficeInstance()
        {
            Process process = GetOfficeProcess();
            if (process != null && process.SessionId == Process.GetCurrentProcess().SessionId)
            {
                logger.Trace(
                    "There is an Office process found PID={0} in current session {1}. It would be terminated to avoid problems.",
                    process.Id,
                    process.SessionId);

                try
                {
                    process.Kill();
                    logger.Trace("Process is terminated PID={0}.", process.Id);
                }
                catch
                {
                    // ignored
                }
            }
        }


        /// <summary>
        /// Возвращает рабочий процесс OpenOffice или LibreOffice.
        /// </summary>
        /// <returns>Рабочий процесс OpenOffice или LibreOffice.</returns>
        private static Process GetOfficeProcess()
        {
            Process[] processesList = Process.GetProcessesByName("soffice.bin");
            return processesList.Length != 0 ? processesList[0] : null;
        }


        /// <summary>
        /// Возвращает запускающий процесс OpenOffice или LibreOffice, который относится к текущей сессии
        /// и будет закрыт при завершении работы.
        ///
        /// Если процессы запускаются на .NET Framework в пределах того же WinAPI Job,
        /// то такой процесс будет закрыт автоматически, но для .NET Core необходимо закрыть его вручную.
        /// </summary>
        /// <returns>Запускающий процесс OpenOffice или LibreOffice.</returns>
        private static Process GetCurrentSessionOfficeDisposableProcess()
        {
            int sessionId = Process.GetCurrentProcess().SessionId;
            return Process.GetProcessesByName("soffice.exe").FirstOrDefault(x => x.SessionId == sessionId)
                ?? Process.GetProcessesByName("soffice.bin").FirstOrDefault(x => x.SessionId == sessionId);
        }


        /// <summary>
        /// Завершает запускающий процесс OpenOffice или LibreOffice,
        /// поиск которого выполняется методом <see cref="GetCurrentSessionOfficeDisposableProcess"/>.
        /// </summary>
        private static void KillOfficeDisposableProcess()
        {
            try
            {
                Process officeDisposableProcess = GetCurrentSessionOfficeDisposableProcess();
                if (officeDisposableProcess != null)
                {
                    logger.Trace(
                        "Terminating Office process PID={0} in current session {1}.",
                        officeDisposableProcess.Id,
                        officeDisposableProcess.SessionId);

                    try
                    {
                        officeDisposableProcess.Kill();
                        logger.Trace("Process is terminated PID={0}.", officeDisposableProcess.Id);
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("Error terminating Office process.");
                        logger.LogException(ex, LogLevel.Warn);
                    }
                }
            }
            catch
            {
                // ignore errors
            }
        }


        /// <summary>
        /// Получает текст с сообщениями об ошибках в выходном файле.
        /// </summary>
        /// <param name="errorFile">Выходной файл с ошибками.</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Текст с сообщениями об ошибках.</returns>
        private static async Task<string> GetErrorTextAsync(ITempFile errorFile, CancellationToken cancellationToken = default)
        {
            string errorText = null;

            string path = errorFile.Path;
            if (File.Exists(path))
            {
                try
                {
                    errorText = await File.ReadAllTextAsync(path, cancellationToken);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            if (errorText != null)
            {
                errorText = errorText.Trim();
            }

            if (string.IsNullOrEmpty(errorText))
            {
                errorText = "Unknown error";
            }

            return errorText;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Выполняется в методах <see cref="PreprocessAsync"/> или <see cref="PerformMaintenanceAsync"/>.
        /// </summary>
        protected virtual async Task PreprocessOrPerformMaintenanceCoreAsync(CancellationToken cancellationToken = default)
        {
            // завершаем предыдущий инстанс. это нестрашно ибо в одной сессии может быть только один инстанс
            KillSessionOfficeInstance();

            var startInfo = new ProcessStartInfo()
                .SetSilentExecution();

            string externalCommand = UnoconvExternalCommand;
            if (externalCommand.Length == 0)
            {
                startInfo
                    .SetApplication(
                        OpenOfficePython,
                        OpenOfficePythonParamsPrefix + $"\"{UnoconvPath}\" -l");
            }
            else
            {
                startInfo
                    .SetApplication(externalCommand, "-l");
            }

            logger.Trace(
                "Starting Office process via command line:{0}\"{1}\" {2}",
                Environment.NewLine,
                startInfo.FileName,
                startInfo.Arguments);

            if (this.processManagerLazy != null)
            {
                this.processManagerLazy.Value.StartProcess(startInfo);
            }

            // запускается небыстро, даем возможность
            int count = 10;

            Process process;
            while ((process = GetOfficeProcess()) == null && count-- != 0)
            {
                await Task.Delay(500, cancellationToken);
            }

            if (process != null)
            {
                logger.Trace("Office process has been started, PID={0}", process.Id);
            }
            else
            {
                logger.Trace("Office process can't be found, may be there is a problem with its configuration. Continuing anyway.");
            }
        }

        #endregion

        #region IFileConverterWorker Members

        /// <summary>
        /// Преобразует файл в заданный формат.
        /// </summary>
        /// <param name="context">Контекст, содержащий информацию по выполняемому преобразованию.</param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        public virtual async Task ConvertFileAsync(IFileConverterContext context, CancellationToken cancellationToken = default)
        {
            switch (context.InputExtension)
            {
                case "pdf":
                    // файл PDF можно "конвертировать" в pdf, просто скопировав
                    await FileHelper.CopyAsync(context.InputFilePath, context.OutputFilePath, cancellationToken);
                    return;

                case "tif":
                case "tiff":
                    // файлы TIFF конвертируются отдельным worker-ом, если он задан
                    if (this.tiffToPdfWorker != null)
                    {
                        await this.tiffToPdfWorker.ConvertFileAsync(context, cancellationToken);
                    }
                    else
                    {
                        context.ValidationResult.AddError(this, "Can't convert to TIFF without registered worker.");
                    }

                    return;
            }

            // все остальные файлы конвертируются средствами OpenOffice/LibreOffice

            if (this.openOfficeErrorText != null)
            {
                // какая-то ошибка возникла при инициализации PDF
                context.ValidationResult.AddError(this, this.openOfficeErrorText);
                return;
            }

            Lazy<IProcessManager> processManagerLazy = this.processManagerLazy;
            if (processManagerLazy == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            string externalCommand = UnoconvExternalCommand;
            using (ITempFile errorFile = TempFile.Acquire("error.txt"))
            {
                var startInfo = new ProcessStartInfo()
                    .SetSilentExecution();

                if (externalCommand.Length == 0)
                {
                    startInfo
                        .SetCommandLine(
                            OpenOfficePython,
                            OpenOfficePythonParamsPrefix
                            + $"\"{UnoconvPath}\" -f {context.OutputExtension} -o \"{context.OutputFilePath}\" \"{context.InputFilePath}\"",
                            outputFile: errorFile.Path,
                            errorFile: errorFile.Path);
                }
                else
                {
                    startInfo
                        .SetCommandLine(
                            externalCommand,
                            $"-f {context.OutputExtension} -o \"{context.OutputFilePath}\" \"{context.InputFilePath}\"",
                            outputFile: errorFile.Path,
                            errorFile: errorFile.Path);
                }

                logger.Trace("Converting file via command line:{0}\"{1}\" {2}", Environment.NewLine, startInfo.FileName, startInfo.Arguments);

                using Process process = processManagerLazy.Value.StartProcess(startInfo);
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    context.ValidationResult.AddError(this,
                        "Conversion failed. Exit code: {0}. Output:{1}{2}",
                        process.ExitCode, Environment.NewLine, await GetErrorTextAsync(errorFile, cancellationToken));

                    return;
                }
            }

            if (externalCommand.Length > 0)
            {
                string filePath = context.OutputFilePath;
                if (!File.Exists(filePath))
                {
                    context.OutputFilePath += "." + context.OutputExtension;
                }
            }
            else
            {
                // unoconv добавляет от себя расширения в выходной папке
                context.OutputFilePath += "." + context.OutputExtension;
            }

            logger.Trace("File has been converted successfully via command line.");

            // пишем ключ, через который вызывающая сторона поймёт, что конвертация была выполнена посредством unoconv
            context.ResponseInfo["unoconv"] = BooleanBoxes.True;
        }


        /// <summary>
        /// Выполняет обработку перед запуском цикла обслуживания для очереди на конвертацию файлов.
        /// Метод запускается единственный раз при старте сервиса конвертации.
        /// </summary>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        public virtual async Task PreprocessAsync(CancellationToken cancellationToken = default)
        {
            string openOfficePython = OpenOfficePython;
            string unoconvPath = UnoconvPath;

            // вместо пути к python может быть указана глобальная команда из %path%, например, "python3", которая сама подтягивается из окружения;
            // в этом случае нельзя проверять наличие файла, просто "доверимся" настройкам
            if (string.IsNullOrEmpty(openOfficePython)
                || Path.GetFileName(openOfficePython) != openOfficePython && !File.Exists(openOfficePython))
            {
                this.openOfficeErrorText = $"OpenOffice or LibreOffice \"{Path.GetFileName(openOfficePython)}\" isn't found: "
                    + (string.IsNullOrEmpty(openOfficePython) ? "<value is empty>" : openOfficePython);
            }
            else if (string.IsNullOrEmpty(unoconvPath) || !File.Exists(unoconvPath))
            {
                this.openOfficeErrorText = "Conversion script \"unoconv\" isn't found: "
                    + (string.IsNullOrEmpty(unoconvPath) ? "<value is empty>" : unoconvPath);
            }

            if (this.openOfficeErrorText == null)
            {
                await this.PreprocessOrPerformMaintenanceCoreAsync(cancellationToken);
            }
            else
            {
                logger.Warn(this.openOfficeErrorText);
            }

            if (this.tiffToPdfWorker != null)
            {
                await this.tiffToPdfWorker.PreprocessAsync(cancellationToken);
            }
        }


        /// <summary>
        /// Выполняет обработку в процессе выполнения цикла обслуживания для очереди на конвертацию файлов.
        /// Метод запускается множество раз внутри цикла с переидичностью, заданной в конфигурационном файле.
        /// </summary>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        public virtual async Task PerformMaintenanceAsync(CancellationToken cancellationToken = default)
        {
            if (this.openOfficeErrorText == null)
            {
                await this.PreprocessOrPerformMaintenanceCoreAsync(cancellationToken);
            }

            if (this.tiffToPdfWorker != null)
            {
                await this.tiffToPdfWorker.PerformMaintenanceAsync(cancellationToken);
            }
        }

        #endregion

        #region IAsyncDisposable Members

        /// <summary>
        /// Освобождение занятых ресурсов.
        /// Метод принудительно закрывает все запущенные процессы.
        /// </summary>
        /// <returns>Асинхронная задача.</returns>
        public virtual async ValueTask DisposeAsync()
        {
            logger.Trace("Shutting down file converter. All child processes will be terminated.");

            if (this.processManagerLazy != null
                && this.processManagerLazy.IsValueCreated)
            {
                this.processManagerLazy.Value.Dispose();
                this.processManagerLazy = null;
            }

            // процесс Office уже может быть остановлен здесь, но для .NET Core убиваем его явно
            KillOfficeDisposableProcess();

            switch (this.tiffToPdfWorker)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync();
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }

        #endregion
    }
}