using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Server.Workflow.KrProcess;
using Tessa.FileConverters;
using Tessa.Files;
using Tessa.Platform.Storage;
using Unity;
using Unity.Lifetime;
using NLog;

namespace Tessa.Extensions.Server.Requests
{
    public sealed class DocxToPdfFileExtension : CardRequestExtension
    {
        private readonly IUnityContainer unityContainer;
        public DocxToPdfFileExtension(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }

        public override async Task AfterRequest(ICardRequestExtensionContext context)
        {
            var cardRepository = unityContainer.Resolve<ICardRepository>(CardRepositoryNames.ExtendedWithoutTransaction);
            var fileManager = unityContainer.Resolve<ICardFileManager>();
            var krTokenProvider = unityContainer.Resolve<IKrTokenProvider>();
            Guid cardID = context.Request.Info.TryGet<Guid>("cardID");
            // если тип карточки включён в типовое решение, то необходимо указать токен с правами
            var token = krTokenProvider.CreateToken(cardID);
            // загружаем карточку
            var getRequest = new CardGetRequest { CardID = cardID };
            token.Set(getRequest.Info);
            CardGetResponse getResponse = await cardRepository.GetAsync(getRequest);
            if (!getResponse.ValidationResult.IsSuccessful())
            {
                return;
            }
            Card card = getResponse.Card;

            Guid versionRowID = context.Request.Info.TryGet<Guid>("versionRowID");
            string fileName = context.Request.Info.TryGet<string>("fileName");

            // обычно запрос из Unity выполняют через конструктор
            var fileConverter = unityContainer.Resolve<IFileConverter>();

            var request = await fileConverter.GetRequestOrThrowAsync(FileConverterEventNames.Unknown, FileConverterFormat.Pdf, versionRowID);
            var response = await fileConverter.ConvertFileAsync(request);

            if (response.ValidationResult.HasErrors)
            {
                context.Response.Info.Add("ConvertFileResult", false);
            }
            else
            {
                using (Stream stream = await response.GetStreamOrThrowAsync())
                {
                    // stream содержит сконвертированный в PDF файл, копируем его в карточку, выгружаем и др.

                    // добавляем или заменяем файл, а затем сохраняем карточку
                    using (ICardFileContainer container = await fileManager.CreateContainerAsync(card))
                    {
                        await container.FileContainer
                            .BuildFile(fileName.Replace(".docx", ".pdf"))
                            .SetContent(stream)
                            .AddWithNotificationAsync(cancellationToken: context.CancellationToken);

                        // сохраняем карточку с файлами
                        CardStoreResponse storeResponse = await container.StoreAsync(
                            (c, request, ct) =>
                            {
                            // для типового решения надо указать токен с правами
                            token.Set(request.Card.Info);
                                return Task.CompletedTask;
                            },
                            cancellationToken: context.CancellationToken);

                        context.Response.Info.Add("ConvertFileResult", storeResponse.ValidationResult.IsSuccessful());
                    }
                }
            }
        }
    }
}