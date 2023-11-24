using System;
using System.IO;
using Tessa.Applications;

namespace Tessa.Extensions.Default.Console.PackageApp
{
    public sealed class ApplicationFile
    {
        #region Constructors

        public ApplicationFile(string filePath, string appFolder)
        {
            this.Name = Path.GetFileName(filePath)
                ?? throw new ArgumentException($"Invalid file path: \"{filePath}\"", nameof(filePath));

            if (filePath.StartsWith(appFolder, StringComparison.OrdinalIgnoreCase) && filePath.Length > appFolder.Length)
            {
                // filePath = C:\TessaClient\x86\subfolder\file.dll
                // appFolder = C:\TessaClient

                // relativePath = x86\subfolder\file.dll (длина appFolder - это положение "\x86...", а нам нужно на один символ дальше)
                // category = x86\subfolder

                string relativePath = filePath.Substring(appFolder.Length + 1);

                if (relativePath.EndsWith(this.Name, StringComparison.Ordinal)
                    && relativePath.Length > this.Name.Length + 1)
                {
                    string category = relativePath.Substring(0, relativePath.Length - this.Name.Length - 1);
                    this.Category = category.NormalizePathForApplications();
                }
            }

            // если относительный путь к папке не нашли сверху, то свойство Category = null

            // размер файла нужен сразу, а содержимое всё равно требуется и для расчёта хэшей,
            // и для записи в JSON (в виде того же массива байт для всех файлов разом), поэтому проще загрузить в память
            this.Content = File.ReadAllBytes(filePath);
            this.Size = this.Content.Length;
        }

        #endregion

        #region Properties

        public Guid RowID { get; } = Guid.NewGuid();

        public string Name { get; }

        public string Category { get; }

        public byte[] Content { get; }

        public long Size { get; }

        #endregion
    }
}