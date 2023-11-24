using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using Tessa.Cards;
using Tessa.Files;
using Tessa.Platform.Runtime;
using Tessa.Properties.Resharper;
using Tessa.UI.Files;
using Tessa.UI.Files.Controls;
using Tessa.Views.Metadata;

namespace Tessa.Extensions.Default.Client.UI.CardFiles
{
    public static class CardFilesActions
    {

        
        
        public static async Task AddFiles([NotNull] IFileControl fileControl,
            IUser user,
            ICardMetadata cardMetadata,
            IFileContainer fileContainer,
            Func<Task, Task> refreshAction)
        {
            if (fileControl == null)
            {
                throw new ArgumentNullException(nameof(fileControl));
            }

            var fileDialog = new OpenFileDialog { Multiselect = true };
            var result = fileDialog.ShowDialog();

            if (result != true || fileDialog.FileNames.Length <= 0)
            {
                return;
            }

            // Считываем пути
            var filePaths = new List<string>();
            foreach (var filePath in fileDialog.FileNames)
            {
                if (Directory.Exists(filePath))
                {
                    filePaths.AddRange(Directory.GetFiles(filePath));
                }
                else
                {
                    filePaths.Add(filePath);
                }
            }

            // Добавляем файлы
            await FileControlHelper.AddFilesAsync(
                fileControl,
                CardHelper.GetCardFileTypes(
                    await CardHelper.GetFileCardTypesAsync(
                        cardMetadata,
                        user.IsAdministrator())),
                fileContainer,
                fileContainer.Source,
                user,
                filePaths);
            await refreshAction(null);
        }
    }
}