using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Default.Shared;
using Tessa.Files;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Files;
using Tessa.UI.Notifications;

namespace Tessa.Extensions.Default.Client.UI
{
    public static class Get1CHelper
    {
        public static async Task RequestAndAddFileAsync(
            IUIHost uiHost,
            ICardRepository cardRepository,
            INotificationUIManager notificationUIManager)
        {
            ICardEditorModel editor = UIContext.Current.CardEditor;
            if (editor is null)
            {
                return;
            }

            ICardModel model = editor.CardModel;
            if (model is null)
            {
                return;
            }

            CardTypeForm dialogForm = model.CardType.Forms.First(x => x.Name == "Dialog_1C");

            bool ok = false;
            await uiHost.ShowFormDialogAsync(
                "$CardTypes_Tabs_Dialog1C",
                dialogForm,
                model,
                buttons: new[]
                {
                    new UIButton(
                        "$UI_Common_OK",
                        btn =>
                        {
                            ok = true;
                            btn.Close();
                        },
                        isDefault: true),

                    new UIButton("$UI_Common_Cancel", isCancel: true),
                });

            if (!ok)
            {
                return;
            }

            var request = new CardRequest { RequestType = DefaultRequestTypes.GetFake1C };
            request.DynamicInfo.Name = model.Card.DynamicEntries.TEST_CarMainInfo.Name;
            request.DynamicInfo.Driver = model.Card.DynamicEntries.TEST_CarMainInfo.DriverName;

            // показываем сплэш и задаём блокирующую операцию для карточки:
            // пользователь не сможет закрыть вкладку или отрефрешить карточку, пока она не закончится

            CardResponse response;
            using (TessaSplash.Create("$KrTest_Splash_LoadingFrom1C"))
            using (editor.SetOperationInProgress(blocking: true))
            {
                await Task.Delay(2000);    // для примера добавим задержки
                response = await cardRepository.RequestAsync(request);
            }

            ValidationResult result = response.ValidationResult.Build();
            await TessaDialog.ShowNotEmptyAsync(result);

            if (!result.IsSuccessful)
            {
                return;
            }

            string content = response.Info.Get<string>("Xml");

            const string fileName = "1C.xml";

            IFile file = model.FileContainer.Files.FirstOrDefault(x => x.Name == fileName);
            if (file != null)
            {
                await DispatcherHelper.InvokeInUIAsync(() => model.FileControlManager.ResetIfInPreview(file));
                await file.ReplaceTextAsync(content);
            }
            else
            {
                await model.FileContainer
                    .BuildFile(fileName)
                    .SetContentText(content, isLocal: true)
                    .AddWithNotificationAsync();
            }

            await notificationUIManager.ShowTextOrMessageBoxAsync("$KrTest_LoadedFrom1C");
        }
    }
}
