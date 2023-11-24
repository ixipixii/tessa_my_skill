using System;
using System.IO;
using System.Text;
using Tessa.Extensions.Default.Shared.VirtualFiles;
using Tessa.Notices;
using Tessa.Platform;

namespace Tessa.Extensions.Default.Shared.Notices
{
    public static class MailFileFactory
    {
        #region Static Methods

        /// <summary>
        /// Создаёт файл, который может прикладываться к письму и который доступен по заданному пути на сервере,
        /// на котором установлен Chronos. После успешной отправки файл может быть удалён в зависимости от настройки <see cref="MailFileRemovalType"/>.
        /// </summary>
        /// <param name="filePath">Путь к файлу на сервере (полный путь на диске или UNC-путь).</param>
        /// <param name="removalType">Тип удаления файла после успешной отправки письма.</param>
        /// <returns>
        /// Файл, который может прикладываться к письму и который доступен по заданному пути на сервере,
        /// на котором установлен Chronos.
        /// </returns>
        public static MailFile CreateFromServerPath(
            string filePath,
            MailFileRemovalType removalType = MailFileRemovalType.KeepFile)
        {
            Check.ArgumentNotNullOrEmpty(filePath, "filePath");

            var file = new MailFile
            {
                FileID = Guid.Empty,
                FileName = Path.GetFileName(filePath),
                VersionID = Guid.Empty,
                FileTypeName = DefaultFileTypes.ServerPath,
                IsVirtual = true,
            };

            file.Info["ServerFilePath"] = filePath;

            bool removeFile = removalType != MailFileRemovalType.KeepFile;
            file.Info["RemoveFile"] = BooleanBoxes.Box(removeFile);

            bool removeFolder = removalType == MailFileRemovalType.RemoveFileAndFolder;
            file.Info["RemoveFolder"] = BooleanBoxes.Box(removeFolder);

            return file;
        }


        /// <summary>
        /// Создаёт файл, который может прикладываться к письму и данные которого встроены в структуру объекта <see cref="MailFile"/>.
        /// Рекомендуется использовать только для небольших файлов (желательно не более 100 Кб).
        /// </summary>
        /// <param name="fileName">Имя прикладываемого файла.</param>
        /// <param name="data">Данные прикладываемого файла.</param>
        /// <returns>Файл, приложенный к письму.</returns>
        public static MailFile Create(string fileName, byte[] data)
        {
            Check.ArgumentNotNullOrEmpty(fileName, "fileName");
            Check.ArgumentNotNull(data, "data");

            var file = new MailFile
            {
                FileID = Guid.Empty,
                FileName = fileName,
                VersionID = Guid.Empty,
                FileTypeName = DefaultFileTypes.EmbeddedData,
                IsVirtual = true,
            };

            file.Info["Data"] = data;

            return file;
        }


        /// <summary>
        /// Создаёт файл, который может прикладываться к письму и текстовые данные которого встроены в структуру объекта <see cref="MailFile"/>
        /// в кодировке <paramref name="encoding"/>. Рекомендуется использовать только для небольших файлов (желательно не более 100 Кб).
        /// </summary>
        /// <param name="fileName">Имя прикладываемого файла.</param>
        /// <param name="content">Содержимое файла в виде текста. Значение <c>null</c> аналогично пустой строке.</param>
        /// <param name="encoding">Кодировка текста в файле или <c>null</c>, если используется кодировка <see cref="Encoding.UTF8"/>.</param>
        /// <returns>Файл, приложенный к письму.</returns>
        public static MailFile Create(string fileName, string content, Encoding encoding = null)
        {
            Check.ArgumentNotNullOrEmpty(fileName, "fileName");

            Encoding actualEncoding = encoding ?? Encoding.UTF8;
            byte[] bytes = actualEncoding.GetBytes(content ?? string.Empty);

            return Create(fileName, bytes);
        }


        /// <summary>
        /// Создаёт файл листа согласования, который может прикладываться к письму.
        /// При локализации имени файла учитывается текущая культура.
        /// </summary>
        /// <param name="type">Тип создаваемого листа согласования.</param>
        /// <returns>Созданный файл.</returns>
        public static MailFile CreateApprovalList(bool isPrintable = false)
        {
            return
                new MailFile
                {                    
                    FileID = KrVirtualFilesHelper.ApprovalListFileID,
                    VersionID = isPrintable ? KrVirtualFilesHelper.ApprovalListPrintableVersionID : KrVirtualFilesHelper.ApprovalListDefaultVersionID,
                    FileName = isPrintable ? "$ApprovalHistory_FileName_Printable" : "$ApprovalHistory_FileName_Default",
                    FileTypeName = DefaultFileTypes.KrVirtualFile,
                    IsVirtual = true,
                };
        }

        #endregion
    }
}
