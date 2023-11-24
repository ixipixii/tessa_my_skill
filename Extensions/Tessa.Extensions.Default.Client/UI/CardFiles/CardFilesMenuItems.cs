using System;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Files;
using Tessa.Platform.Runtime;
using Tessa.Properties.Resharper;
using Tessa.UI;
using Tessa.UI.Files;
using Tessa.UI.Menu;

namespace Tessa.Extensions.Default.Client.UI.CardFiles
{
    public static class CardFilesMenuItems
    {
        [PublicAPI]
        public static IMenuAction CreateUploadFileMenuItem(
            IFileControl fileControl,
            IIconContainer iconContainer,
            IUser user,
            ICardMetadata cardMetadata,
            IFileContainer fileContainer,
            Func<Task, Task> refreshAction)
        {
            var uploadFileMenuAction = new MenuAction(
                FileMenuActionNames.Upload,
                "$UI_Controls_FilesControl_UploadFiles",
                iconContainer.Get("Thin50"),
                new DelegateCommand(async o => await CardFilesActions.AddFiles(fileControl, user, cardMetadata, fileContainer, refreshAction)));
            
            return uploadFileMenuAction;
        }
        
        
        public static IMenuAction CreateOpenFileMenuItem(
            IFileControl fileControl,
            IIconContainer iconContainer,
            IUser user,
            ICardMetadata cardMetadata,
            IFileContainer fileContainer,
            Func<Task, Task> refreshAction)
        {
            var uploadFileMenuAction = new MenuAction(
                FileMenuActionNames.Upload,
                "$UI_Controls_FilesControl_UploadFiles",
                iconContainer.Get("Thin50"),
                new DelegateCommand(async o => await CardFilesActions.AddFiles(fileControl, user, cardMetadata, fileContainer, refreshAction)));
            
            return uploadFileMenuAction;
        }
        
    }
}