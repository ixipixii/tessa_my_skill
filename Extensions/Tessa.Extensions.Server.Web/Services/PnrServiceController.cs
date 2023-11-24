using LinqToDB.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Server.DataHelpers;
using Tessa.Extensions.Shared.Helpers;
using Tessa.Extensions.Server.Web.Models;
using Tessa.Extensions.Shared;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Web;
using Tessa.Web.Services;
using Unity;
using Tessa.Extensions.Shared.Models;
using Tessa.Platform.Validation;
using System.Xml.Serialization;
using Tessa.Cards.Caching;
using System.Text.Json;
using Tessa.Files;
using Tessa.Extensions.Platform.Server.Roles;
using Tessa.Platform.Storage;
using System.Text.Unicode;

namespace Tessa.Extensions.Server.Web.Services
{
    [Route("PnrService"), AllowAnonymous]
    [Produces("application/json")]
    public sealed class PnrServiceController : TessaControllerBase
    {
        private readonly ITessaWebScope scope;
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IOptions<WebOptions> options;
        private const string LogSeparator = "====================================================";

        public PnrServiceController(
            ITessaWebScope scope,
            IWebHostEnvironment hostEnvironment,
            IOptions<WebOptions> options)
        {
            this.scope = scope;
            this.hostEnvironment = hostEnvironment;
            this.options = options;
        }

        private Guid ToGuid(Guid? source)
        {
            return source ?? Guid.Empty;
        }

        private T Resolve<T>(string name = null) => this.scope.UnityContainer.Resolve<T>(name);

        /// <summary>
        /// Универсальный метод для создания/обновления карточек в Тессе
        /// </summary>
        /// <param name="getRequestFunc"></param>
        /// <param name="logger"></param>
        /// <param name="logInputJson">Нужно ли выводить в лог входной запрос в виде JSON</param>
        /// <param name="isCanCreate">Возможно ли создание новой карточки (иначе только обновление)</param>
        /// <returns></returns>
        private async Task<PnrBaseResponse> CreateOrUpdateCard<T>(Func<PnrBaseRequest> getRequestFunc, Logger logger, bool logInputJson = false, bool isCanCreate = true)
        {
            PnrBaseRequest pnrBaseRequest = null;
            try
            {
                logger.Info(LogSeparator);
                logger.Info($"Новый запрос.");

                // тут парсим объект в ручную, т.к. нет возможности настроить XML десериализацию на уровне настроек веб-сервиса
                pnrBaseRequest = getRequestFunc();
                if (pnrBaseRequest == null)
                {
                    string modelErrors = null;
                    if (this.ModelState.Values.Any())
                    {
                        modelErrors = $" Ошибки: {string.Join("; ", this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))}.";
                    }

                    string modelErrorsResult = $"Не удалось считать входные данные.{modelErrors}";

                    logger.Error(modelErrorsResult);
                    return new PnrBaseResponse(PnrBaseResponseStatusCode.Error, modelErrorsResult);
                }

                // XML тут не будем выводить, чтобы не засорять логи, т.к. он выводится до этого при парсинге XML
                if (logInputJson)
                {
                    logger.Info($"Полученные входные данные:{Environment.NewLine}{JsonConvert.SerializeObject(pnrBaseRequest, Formatting.Indented)}");
                }

                var serverSettings = this.Resolve<ITessaServerSettings>();

                // код внутри будет выполняться от имени пользователя System
                await using (SessionContext.Create(Session.CreateSystemToken(SessionType.Server, serverSettings)))
                {
                    var dbScope = this.Resolve<IDbScope>();
                    var cardRepository = this.Resolve<ICardRepository>();
                    var cardFileManager = this.Resolve<ICardFileManager>();
                    var cardMetadata = this.Resolve<ICardMetadata>();
                    var session = this.Resolve<ISession>();


                    // ID карточки
                    var cardID = await pnrBaseRequest.GetCardIDAsync(dbScope);

                    // ID типа карточки
                    var cardTypeID = await pnrBaseRequest.GetCardTypeID(dbScope);
                    
                    Card card = null;

                    // создание новой карточки
                    if (isCanCreate && cardID == null)
                    {
                        if (cardTypeID == null)
                        {
                            return pnrBaseRequest.GetErrorResult(logger, "Не удалось определить тип карточки!", $"Ошибка создания карточки: ");
                        }

                        // создадим карточку
                        var newResponse = await cardRepository.NewAsync(new CardNewRequest()
                        {
                            CardTypeID = cardTypeID
                        });

                        if (!newResponse.ValidationResult.IsSuccessful())
                        {
                            var errorMessage = newResponse.ValidationResult.Build().ToString();

                            return pnrBaseRequest.GetErrorResult(logger, errorMessage, "Ошибка создания карточки: ");
                        }

                        card = newResponse.Card;

                        card.ID = Guid.NewGuid();
                    }
                    // обновление существующей
                    else if (cardID != null)
                    {
                        // загрузим карточку
                        var getResponse = await cardRepository.GetAsync(new CardGetRequest()
                        {
                            CardID = cardID.Value,
                            CardTypeID = cardTypeID
                        });

                        if (!getResponse.ValidationResult.IsSuccessful())
                        {
                            var errorMessage = getResponse.ValidationResult.Build().ToString();

                            return pnrBaseRequest.GetErrorResult(logger, errorMessage, "Ошибка загрузки карточки: ");
                        }

                        card = getResponse.Card;
                    }

                    if (card == null)
                    {
                        return pnrBaseRequest.GetErrorResult(logger, "Не удалось получить объект карточки для создания/обновления!");
                    }

                    ValidationResultBuilder fillCardDataValidationResult = new ValidationResultBuilder();

                    await pnrBaseRequest.FillCardDataAsync(dbScope, card, fillCardDataValidationResult);

                    if (!fillCardDataValidationResult.IsSuccessful())
                    {
                        return pnrBaseRequest.GetErrorResult(logger, fillCardDataValidationResult.Build().ToString(), "Ошибка заполнения карточки: ");
                    }

                    // выведем все сообщения в лог (там могут быть предупреждения)
                    string fillCardDataWarnings = fillCardDataValidationResult.Any()
                        ? "ВНИМАНИЕ! При заполнении полей карточки возникли следующие предупреждения: " + fillCardDataValidationResult.Build().ToString()
                        : null;

                    if (fillCardDataWarnings != null)
                    {
                        logger.Warn(fillCardDataWarnings);
                    }

                    if (card.StoreMode == CardStoreMode.Update)
                    {
                        card.RemoveAllButChanged();
                    }

                    // сохраним карточку
                    var storeResponse = await cardRepository.StoreAsync(new CardStoreRequest()
                    {
                        Card = card,
                        // передадим флаг, по которому будем пропускать проверку заполненности некоторых полей
                        Info = new Dictionary<string, object>()
                        {
                            { PnrInfoKeys.PnrSkipCardFieldsCustomValidation, true }
                        }
                    });

                    if (!storeResponse.ValidationResult.IsSuccessful())
                    {
                        var errorMessage = storeResponse.ValidationResult.Build().ToString();

                        return pnrBaseRequest.GetErrorResult(logger, errorMessage, "Ошибка сохранения карточки: ");
                    }

                    var successMessage = @$"Карточка {pnrBaseRequest.GetDescription()} успешно {(card.StoreMode == CardStoreMode.Insert
                        ? "создана"
                        : "обновлена")}.";

                    // если были предупреждения, добавим их в ответ
                    if (fillCardDataWarnings != null)
                    {
                        successMessage += $" {fillCardDataWarnings}";
                    }

                    // после сохранения основной карточки выполним постобработку (если нужно)
                    ValidationResultBuilder afterCardStoreActionValidationResult = new ValidationResultBuilder();
                    await pnrBaseRequest.AfterCardStoreActionAsync(logger, dbScope, cardRepository, card, afterCardStoreActionValidationResult);
                    if (!afterCardStoreActionValidationResult.IsSuccessful())
                    {
                        var afterCardStoreActionErrors = afterCardStoreActionValidationResult.Build().ToString();

                        // ошибку добавим к сообщению ответа
                        successMessage += $"{Environment.NewLine}Ошибка при постобработке: {afterCardStoreActionErrors}";
                    }

                    var result = pnrBaseRequest.GetSuccessResult(logger, successMessage, card, session);

                    logger.Info($"Ответ:{Environment.NewLine}{JsonConvert.SerializeObject(result, Formatting.Indented)}");

                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new PnrBaseResponse(PnrBaseResponseStatusCode.Error, ex.ToString());
            }
            finally
            {
                logger.Info($"Завершение обработки запроса.");
                logger.Info(LogSeparator);
            }

        }

        private async Task<PnrFileResponse> CreateOrUpdateFileInternal(PnrFileRequest fileRequest, Logger logger)
        {
            try
            {
                logger.Info(LogSeparator);


                logger.Info($"Новый запрос.");

                if (fileRequest == null)
                {
                    string modelErrors = null;
                    if (this.ModelState.Values.Any())
                    {
                        modelErrors = $" Ошибки: {string.Join("; ", this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))}.";
                    }

                    string modelErrorsResult = $"Не удалось считать входные данные.{modelErrors}";

                    logger.Error(modelErrorsResult);
                    return new PnrFileResponse(PnrBaseResponseStatusCode.Error, modelErrorsResult, new PnrFileResponseValue());
                }

                // в лог выведем запрос без содержимого файла
                var fileRequestCopy = new PnrFileRequest()
                {
                    Id = fileRequest?.Id,
                    FileName = fileRequest?.FileName,
                    FolderUId = fileRequest?.FolderUId,
                    UniqueId = fileRequest?.UniqueId,
                    User = fileRequest?.User
                };
                logger.Info($"Полученные входные данные:{Environment.NewLine}{JsonConvert.SerializeObject(fileRequestCopy, Formatting.Indented)}");


                var serverSettings = this.Resolve<ITessaServerSettings>();

                // код внутри будет выполняться от имени пользователя System
                await using (SessionContext.Create(Session.CreateSystemToken(SessionType.Server, serverSettings)))
                {
                    var dbScope = this.Resolve<IDbScope>();
                    var cardRepository = this.Resolve<ICardRepository>();
                    var cardFileManager = this.Resolve<ICardFileManager>();
                    var cardMetadata = this.Resolve<ICardMetadata>();
                    var cardCache = this.Resolve<ICardCache>();
                    var permissionsProvider = this.Resolve<ICardServerPermissionsProvider>();

                    // адрес веб-клиента
                    var webAddress = await CardHelper.TryGetWebAddressAsync(cardCache);

                    // ID карточки
                    Guid? cardID = fileRequest.GetCardID();

                    if (cardID == null)
                    {
                        return fileRequest.GetErrorResult(logger, $"Не удалось определить ID карточки.");
                    }

                    if (string.IsNullOrEmpty(fileRequest.FileName))
                    {
                        return fileRequest.GetErrorResult(logger, $"Не удалось определить название файла.");
                    }

                    // внешний ID файла
                    Guid? externalFileID = fileRequest.GetExternalFileID();

                    // загрузим карточку
                    var getResponse = await cardRepository.GetAsync(new CardGetRequest()
                    {
                        CardID = cardID.Value
                    });

                    if (!getResponse.ValidationResult.IsSuccessful())
                    {
                        var errorMessage = getResponse.ValidationResult.Build().ToString();

                        return fileRequest.GetErrorResult(logger, errorMessage, $"Ошибка загрузки карточки {{{cardID.Value}}}: ");
                    }

                    var card = getResponse.Card;

                    // файл закодирован в формате base64
                    Byte[] bytes = Convert.FromBase64String(fileRequest.DocumentBody);

                    using ICardFileContainer container = await cardFileManager
                        .CreateContainerAsync(getResponse.Card);

                    // логика такая
                    // если externalFileID задан и найден в ТЕССА, то обновляем файл
                    // иначе создаем новый

                    // ищем существующий файл
                    IFile existingFile = externalFileID != null
                        ? container.FileContainer.Files.TryGet(externalFileID.Value)
                        : null;

                    // если задан внешний ID, то он будет результирующим
                    Guid resultFileID = externalFileID ?? Guid.NewGuid();

                    // добавление нового файла (даже в том случае, когда был передан внешний ID, но не был найден)
                    if (existingFile == null)
                    {
                        // ID версии сгенерим заранее сразу
                        Guid versionRowID = Guid.NewGuid();

                        var createFileResult = await container.FileContainer
                            .BuildFile(fileRequest.FileName)
                            .SetFileToken(async (token, ct) =>
                            {
                                // укажем свой ID файла для удобства при обновлении карточки
                                token.ID = resultFileID;
                            })
                            .SetVersionToken(async (token, ct) =>
                            {
                                token.ID = versionRowID;
                            })
                            .SetContent(bytes)
                            .AddWithNotificationAsync();

                        if (!createFileResult.result.IsSuccessful)
                        {
                            return fileRequest.GetErrorResult(logger, createFileResult.result.ToString(), $"Ошибка создания нового файла: ");
                        }

                        // достанем сгенерированный новый файл и запомним в нем первую версию файла
                        var creatingFile = card.Files.First(x => x.RowID == resultFileID);
                        creatingFile.OriginalVersionRowID = versionRowID;
                    }
                    // обновление существующего файла
                    else
                    {
                        await existingFile.ReplaceAsync(bytes);
                        if (fileRequest.FileName != existingFile.Name)
                        {
                            await existingFile.SetNameAsync(fileRequest.FileName);
                            await existingFile.NotifyAsync(FileNotificationType.NameModified);
                        }
                    }

                    CardStoreResponse storeResponse = await container.StoreAsync(
                        (fileContainer, storeRequest, ct) =>
                        {
                            // передадим флаг, по которому будем пропускать проверку заполненности некоторых полей
                            storeRequest.Info = new Dictionary<string, object>()
                            {
                                { PnrInfoKeys.PnrSkipCardFieldsCustomValidation, true }
                            };
                            permissionsProvider.SetFullPermissions(storeRequest);
                            return Task.CompletedTask;
                        });

                    if (!storeResponse.ValidationResult.IsSuccessful())
                    {
                        var errorMessage = storeResponse.ValidationResult.Build().ToString();

                        return fileRequest.GetErrorResult(logger, errorMessage, $"Ошибка сохранения файла [{fileRequest.FileName}]: ");
                    }

                    var successMessage = @$"Файл [{fileRequest.FileName}] успешно {(existingFile == null
                        ? "создан"
                        : "обновлен")}. ID файла в ТЕССА: {{{resultFileID}.}}";

                    var actualFile = container.FileContainer.TryGetFile(resultFileID);

                    // сформируем ссылку на файл в веб-клиенте
                    var fileUrl = actualFile != null && actualFile.TryGetActualVersion() != null
                        ? CardHelper.GetWebFileLink(webAddress, cardID.Value, actualFile.ID, actualFile.TryGetActualVersion().ID, card.TypeID, fileName: actualFile.Name, asHtml: false)
                        : null;
                    var result = fileRequest.GetSuccessResult(logger, successMessage, resultFileID, fileUrl);

                    logger.Info($"Ответ:{Environment.NewLine}{JsonConvert.SerializeObject(result, Formatting.Indented)}");

                    return result;
                }
            }
            catch (Exception ex)
            {
                return fileRequest.GetErrorResult(logger, ex.ToString());
            }
            finally
            {
                logger.Info($"Завершение обработки запроса.");
                logger.Info(LogSeparator);
            }
        }

        /// <summary>
        /// Десериализация объекта из тела запроса с XML данными
        /// </summary>
        private T GetObjectFromRequestBodyXML<T>(Logger logger) where T : PnrBaseRequest
        {
            string bodyText = null;
            try
            {
                using (var reader = new StreamReader(this.Request.Body, Encoding.UTF8))
                {
                    bodyText = reader.ReadToEnd();

                    // выведем входную XML
                    logger.Info($"Полученные входные данные:{Environment.NewLine}{bodyText}");

                    using (var textReadear = new MemoryStream(Encoding.UTF8.GetBytes(bodyText)))
                    {
                        var serializer = new XmlSerializer(typeof(T));
                        T result = (T)serializer.Deserialize(textReadear);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Ошибка парсинга XML.");
                throw;
            }
        }

        [HttpGet, Produces(MediaTypeNames.Text.Plain)]
        public string Get()
        {
            return "Веб-сервис работает.";
        }

        /// <summary>
        /// Договор (интеграция с CRM)
        /// </summary>
        [HttpPost(nameof(CreateOrUpdateContract)+ "/createProject")]
        public async Task<JsonResult> CreateOrUpdateContract([FromBody] PnrCrmContractRequest contract)
        {
            var logger = LogManager.GetLogger(nameof(CreateOrUpdateContract));

            // тут сразу получаем объект из JSON
            // по сути это будет объект типа PnrContractResponse
            var result = await CreateOrUpdateCard<PnrCrmContractRequest>(() => contract, logger, logInputJson: true);

            return Json(result, new JsonSerializerOptions()
            {
                // попросили возвращать поля с заглавной буквы
                PropertyNamingPolicy = null
            });
        }

        /// <summary>
        /// Создание файла
        /// </summary>
        [HttpPost(nameof(CreateOrUpdateContract) + "/submitProjectFile")]
        public async Task<JsonResult> CreateOrUpdateFile([FromBody] PnrFileRequest file)
        {
            var logger = LogManager.GetLogger(nameof(CreateOrUpdateFile));

            var result = await CreateOrUpdateFileInternal(file, logger);

            return Json(result, new JsonSerializerOptions()
            {
                // попросили возвращать поля с заглавной буквы
                PropertyNamingPolicy = null
            });
        }

        /// <summary>
        /// Обновление договора (интеграция с НСИ)
        /// </summary>
        [HttpPost(nameof(MdmUpdateContract))]
        public async Task<PnrBaseResponse> MdmUpdateContract()
        {
            var logger = LogManager.GetLogger(nameof(MdmUpdateContract));

            // тут парсим объект в ручную, т.к. нет возможности настроить XML десериализацию на уровне настроек веб-сервиса
            var result = await CreateOrUpdateCard<PnrMdmUpdateContractRequest>(() => GetObjectFromRequestBodyXML<PnrMdmUpdateContractRequest>(logger), logger,
                isCanCreate: false); // не будем создавать новую карточку

            return result;
        }

        /// <summary>
        /// Проект
        /// </summary>
        [HttpPost(nameof(CreateOrUpdateProject))]
        public async Task<PnrBaseResponse> CreateOrUpdateProject()
        {
            var logger = LogManager.GetLogger(nameof(CreateOrUpdateProject));

            // тут парсим объект в ручную, т.к. нет возможности настроить XML десериализацию на уровне настроек веб-сервиса
            var result = await CreateOrUpdateCard<PnrProjectRequest>(() => GetObjectFromRequestBodyXML<PnrProjectRequest>(logger), logger);

            return result;
        }

        /// <summary>
        /// ЦФО
        /// </summary>
        [HttpPost(nameof(CreateOrUpdateCFO))]
        public async Task<PnrBaseResponse> CreateOrUpdateCFO()
        {
            var logger = LogManager.GetLogger(nameof(CreateOrUpdateCFO));

            var result = await CreateOrUpdateCard<PnrCFORequest>(() => GetObjectFromRequestBodyXML<PnrCFORequest>(logger), logger);

            return result;
        }

        /// <summary>
        /// Статья затрат
        /// </summary>
        [HttpPost(nameof(CreateOrUpdateCostItem))]
        public async Task<PnrBaseResponse> CreateOrUpdateCostItem()
        {
            var logger = LogManager.GetLogger(nameof(CreateOrUpdateCostItem));

            var result = await CreateOrUpdateCard<PnrCostItemRequest>(() => GetObjectFromRequestBodyXML<PnrCostItemRequest>(logger), logger);

            return result;
        }

        /// <summary>
        /// Организация
        /// </summary>
        [HttpPost(nameof(CreateOrUpdateOrganization))]
        public async Task<PnrBaseResponse> CreateOrUpdateOrganization()
        {
            var logger = LogManager.GetLogger(nameof(CreateOrUpdateOrganization));

            var result = await CreateOrUpdateCard<PnrOrganizationRequest>(() => GetObjectFromRequestBodyXML<PnrOrganizationRequest>(logger), logger);

            return result;
        }

        /// <summary>
        /// Контрагент
        /// </summary>
        [HttpPost(nameof(CreateOrUpdatePartner))]
        public async Task<PnrBaseResponse> CreateOrUpdatePartner()
        {
            var logger = LogManager.GetLogger(nameof(CreateOrUpdatePartner));

            var result = await CreateOrUpdateCard<PnrServicePartnerRequest>(() => GetObjectFromRequestBodyXML<PnrServicePartnerRequest>(logger), logger);

            return result;
        }
    }
}
