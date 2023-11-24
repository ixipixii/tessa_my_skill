using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using LinqToDB.DataProvider;
using Tessa.Cards;
using Tessa.Localization;
using Tessa.Platform;
using Tessa.Platform.ConsoleApps;
using Tessa.Platform.Data;
using Tessa.Platform.Validation;
using CsvWriter = Tessa.Platform.IO.CsvWriter;

namespace Tessa.Extensions.Default.Console
{
    public static class DefaultConsoleHelper
    {
        #region Static Methods

        public static List<string> GetSourceFiles(
            IEnumerable<string> source,
            string patternMatch,
            bool throwIfNotFound = true,
            bool checkPatternMatch = false,
            string displayPatternMatch = null)
        {
            var result = new List<string>();

            foreach (string sourceItem in source ?? Enumerable.Empty<string>())
            {
                // если указан throwIfNotFound, то при наличии хотя бы одного параметра без скриптов будет ошибка с указанием имени папки
                result.AddRange(GetSourceFiles(sourceItem, patternMatch, throwIfNotFound, checkPatternMatch, displayPatternMatch));
            }

            // если параметров не указано, то здесь возвращается пустой массив
            return result;
        }


        public static string NormalizeFolderAndCreateIfNotExists(string sourceFolder)
        {
            sourceFolder = sourceFolder.NormalizePathOnCurrentPlatform() ?? string.Empty;

            if (sourceFolder == ".")
            {
                sourceFolder = Directory.GetCurrentDirectory();
            }

            if (sourceFolder.Length > 0 && !Directory.Exists(sourceFolder))
            {
                Directory.CreateDirectory(sourceFolder);
            }

            return sourceFolder;
        }


        public static List<string> GetSourceFiles(
            string source,
            string patternMatch,
            bool throwIfNotFound = true,
            bool checkPatternMatch = false,
            string displayPatternMatch = null)
        {
            // в качестве patternMatch мы ожидаем паттерн только для файла, не для части пути,
            // иначе checkPatternMatch будет работать некорректно; пример: *.card

            source = source.NormalizePathOnCurrentPlatform();

            if (source == ".")
            {
                source = Directory.GetCurrentDirectory();
            }

            // можем вернуть все файлы в заданной папке или в её подпапках (чаще всего)
            if (Directory.Exists(source))
            {
                // выводим сначала отсортированный список для первого расширения, потом для второго,
                // но между собой их не перемешиваем (если надо - вызывающий код сам перемешает)
                List<string> filePathList = patternMatch
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .SelectMany(pattern => Directory
                        .GetFiles(source, pattern.Trim(), SearchOption.AllDirectories)
                        .OrderBy(x => x))
                    .ToList();

                if (filePathList.Count > 0)
                {
                    return filePathList;
                }
            }

            // или заданный файл
            if (File.Exists(source))
            {
                bool addSourceToResult;
                if (checkPatternMatch && !string.Equals(source, patternMatch, StringComparison.OrdinalIgnoreCase))
                {
                    // требуется определить, действительно ли указанный файл совпадает с patternMatch-ем;
                    // например, для *.cardlib надо проверить, что расширение у source равно .cardlib
                    string sourceFolder = Path.GetDirectoryName(source);
                    if (string.IsNullOrEmpty(sourceFolder))
                    {
                        sourceFolder = Directory.GetCurrentDirectory();
                    }

                    // ReSharper disable once AssignNullToNotNullAttribute
                    string rootedSource = Path.Combine(sourceFolder, Path.GetFileName(source));

                    // для всех файлов в папке вместе с source проверяем, что хотя бы один из файлов,
                    // найденных по patternMatch, совпадает с указанным source
                    addSourceToResult = false;

                    foreach (string fileMatch in Directory.EnumerateFiles(sourceFolder, patternMatch))
                    {
                        if (string.Equals(fileMatch, rootedSource, StringComparison.OrdinalIgnoreCase))
                        {
                            addSourceToResult = true;
                            break;
                        }
                    }
                }
                else
                {
                    addSourceToResult = true;
                }

                var result = new List<string>();
                if (addSourceToResult)
                {
                    result.Add(source);
                }

                return result;
            }

            // или все подходящие файлы, например, "Configuration\Localization\*.tll"
            string folder = Path.GetDirectoryName(source);
            if (string.IsNullOrEmpty(folder))
            {
                folder = Directory.GetCurrentDirectory();
            }

            if (Directory.Exists(folder))
            {
                string matchPattern = Path.GetFileName(source);
                if (!string.IsNullOrEmpty(matchPattern))
                {
                    List<string> filePathList = Directory
                        .GetFiles(folder, matchPattern, SearchOption.TopDirectoryOnly)
                        .OrderBy(x => x)
                        .ToList();

                    if (filePathList.Count > 0)
                    {
                        return filePathList;
                    }
                }
            }

            if (!throwIfNotFound)
            {
                return new List<string>();
            }

            throw new FileNotFoundException(
                $"Couldn't locate {displayPatternMatch ?? patternMatch} file in \"{source}\"",
                source);
        }


        public static async Task DropAndCreateDatabaseAsync(
            IConsoleLogger logger,
            string configurationString,
            string databaseName,
            bool dropOld,
            bool createNew,
            CancellationToken cancellationToken = default)
        {
            (_, ConfigurationConnection configurationConnection) =
                ConfigurationManager.Default.Configuration
                    .GetConfigurationDataProvider(configurationString);

            DbProviderFactory factory = ConfigurationManager
                .GetConfigurationDataProviderFromType(configurationConnection.DataProvider)
                .GetDbProviderFactory();

            DbConnectionStringBuilder connectionBuilder = factory.CreateConnectionStringBuilder();
            if (connectionBuilder is null)
            {
                throw new InvalidOperationException($"Connection builder is null in method {nameof(DefaultConsoleHelper)}.{nameof(DropAndCreateDatabaseAsync)}");
            }

            connectionBuilder.ConnectionString = configurationConnection.ConnectionString;

            Dbms dbms = factory.GetDbms();
            switch (dbms)
            {
                case Dbms.SqlServer:
                    if (string.IsNullOrEmpty(databaseName))
                    {
                        databaseName = (string) connectionBuilder["Initial Catalog"];
                    }

                    connectionBuilder["Database"] = "master";
                    break;

                case Dbms.PostgreSql:
                    if (string.IsNullOrEmpty(databaseName))
                    {
                        databaseName = (string) connectionBuilder["Database"];
                    }

                    connectionBuilder["Database"] = "postgres";
                    break;

                default:
                    throw new NotSupportedException();
            }

            string masterConnectionString = connectionBuilder.ConnectionString;

            DbConnection connection = null;
            DbCommand command = null;

            try
            {
                connection = factory.CreateConnection();
                if (connection is null)
                {
                    throw new InvalidOperationException($"Connection is null in method {nameof(DefaultConsoleHelper)}.{nameof(CheckConnectionAsync)}");
                }

                connection.ConnectionString = masterConnectionString;
                command = connection.CreateCommand();

                await connection.OpenAsync(cancellationToken);

                if (dropOld)
                {
                    await logger.InfoAsync("Dropping database \"{0}\" if it exists", databaseName);

                    await DropDatabaseAsync(command, dbms, databaseName, cancellationToken);
                }

                if (createNew)
                {
                    await CreateDatabaseAsync(command, dbms, databaseName, cancellationToken);
                }
            }
            finally
            {
                if (command != null)
                {
                    await command.DisposeAsync();
                }

                if (connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }

        private static Task CreateDatabaseAsync(
            DbCommand command,
            Dbms dbms,
            string databaseName,
            CancellationToken cancellationToken = default)
        {
            var builder = StringBuilderHelper.Acquire();
            switch (dbms)
            {
                case Dbms.SqlServer:
                    builder
                        .Append("CREATE DATABASE ")
                        .AppendSqlIdentifier(databaseName)
                        .Append(';');
                    break;

                case Dbms.PostgreSql:
                    builder
                        .Append("CREATE DATABASE ")
                        .AppendPostgresIdentifier(databaseName)
                        .Append(" WITH ENCODING='UTF8' TEMPLATE=template0;");
                    break;

                default:
                    throw new NotSupportedException();
            }

            command.CommandText = builder.ToStringAndRelease();
            return command.ExecuteNonQueryAsync(cancellationToken);
        }

        private static Task DropDatabaseAsync(
            DbCommand command,
            Dbms dbms,
            string databaseName,
            CancellationToken cancellationToken = default)
        {
            var builder = StringBuilderHelper.Acquire();
            switch (dbms)
            {
                case Dbms.SqlServer:
                    builder
                        .Append("IF EXISTS (SELECT * FROM [sys].[databases] WHERE [name] = '")
                        .AppendEscaped(databaseName, '\'', '\'')
                        .Append("')")
                        .AppendLine()
                        .Append("BEGIN")
                        .AppendLine()
                        .Append("\tALTER DATABASE ")
                        .AppendSqlIdentifier(databaseName)
                        .Append(" SET OFFLINE WITH ROLLBACK IMMEDIATE;")
                        .AppendLine()
                        .Append("\tALTER DATABASE ")
                        .AppendSqlIdentifier(databaseName)
                        .Append(" SET ONLINE;")
                        .AppendLine()
                        .Append("\tDROP DATABASE ")
                        .AppendSqlIdentifier(databaseName)
                        .Append(';')
                        .AppendLine()
                        .Append("END;");
                    break;

                case Dbms.PostgreSql:
                    builder
                        .Append("SELECT pg_terminate_backend(pid)")
                        .AppendLine()
                        .Append("FROM pg_stat_activity")
                        .AppendLine()
                        .Append("WHERE pg_stat_activity.datname = '")
                        .AppendEscaped(databaseName, '\'', '\'')
                        .Append("';")
                        .AppendLine()
                        .Append("DROP DATABASE IF EXISTS ")
                        .AppendPostgresIdentifier(databaseName)
                        .Append(';');
                    break;

                default:
                    throw new NotSupportedException();
            }

            command.CommandText = builder.ToStringAndRelease();
            return command.ExecuteNonQueryAsync(cancellationToken);
        }


        public static async Task<Dbms> CheckConnectionAsync(
            IConsoleLogger logger,
            string configurationString,
            int timeoutSecondsOverride,
            bool outputConnectionString,
            CancellationToken cancellationToken = default)
        {
            (_, ConfigurationConnection configurationConnection) =
                ConfigurationManager.Default.Configuration
                    .GetConfigurationDataProvider(configurationString);

            DbProviderFactory factory = ConfigurationManager
                .GetConfigurationDataProviderFromType(configurationConnection.DataProvider)
                .GetDbProviderFactory();

            Dbms dbms = factory.GetDbms();

            string databaseValue;
            string timeoutKey;
            switch (dbms)
            {
                case Dbms.SqlServer:
                    databaseValue = "master";
                    timeoutKey = "Connect Timeout";
                    break;

                case Dbms.PostgreSql:
                    databaseValue = "postgres";
                    timeoutKey = "Timeout";
                    break;

                default:
                    throw new NotSupportedException();
            }

            DbConnectionStringBuilder connectionBuilder = factory.CreateConnectionStringBuilder();
            if (connectionBuilder is null)
            {
                throw new InvalidOperationException($"Connection builder is null in method {nameof(DefaultConsoleHelper)}.{nameof(CheckConnectionAsync)}");
            }

            connectionBuilder.ConnectionString = configurationConnection.ConnectionString;
            connectionBuilder["Database"] = databaseValue;

            if (timeoutSecondsOverride >= 0)
            {
                connectionBuilder[timeoutKey] = timeoutSecondsOverride;
            }

            string connectionString = connectionBuilder.ToString();
            await logger.InfoAsync("Connection string is:{0}{1}", Environment.NewLine, connectionString);

            if (outputConnectionString)
            {
                await logger.WriteLineAsync(connectionString);
            }

            await using (DbConnection connection = factory.CreateConnection())
            {
                if (connection is null)
                {
                    throw new InvalidOperationException($"Connection is null in method {nameof(DefaultConsoleHelper)}.{nameof(CheckConnectionAsync)}");
                }

                connection.ConnectionString = connectionString;
                await connection.OpenAsync(cancellationToken);
            }

            return dbms;
        }


        public static async Task<DbManager> CreateDbManagerAsync(
            IConsoleLogger logger,
            string configurationString,
            string databaseNameOverride,
            CancellationToken cancellationToken = default)
        {
            (ConfigurationDataProvider configurationDataProvider, ConfigurationConnection configurationConnection) =
                ConfigurationManager.Default.Configuration
                    .GetConfigurationDataProvider(configurationString);

            DbProviderFactory factory = ConfigurationManager
                .GetConfigurationDataProviderFromType(configurationConnection.DataProvider)
                .GetDbProviderFactory();

            DbConnectionStringBuilder connectionBuilder = factory.CreateConnectionStringBuilder();
            if (connectionBuilder is null)
            {
                throw new InvalidOperationException($"Connection builder is null in method {nameof(DefaultConsoleHelper)}.{nameof(CreateDbManagerAsync)}");
            }

            connectionBuilder.ConnectionString = configurationConnection.ConnectionString;

            if (!string.IsNullOrEmpty(databaseNameOverride))
            {
                connectionBuilder["Database"] = databaseNameOverride;
            }

            string connectionString = connectionBuilder.ToString();
            IDataProvider dataProvider = configurationDataProvider.GetDataProvider(connectionString);

            await logger.InfoAsync("Connection string is:{0}{1}", Environment.NewLine, connectionString);

            DbConnection connection = null;

            try
            {
                connection = factory.CreateConnection();
                if (connection is null)
                {
                    throw new InvalidOperationException($"Connection is null in method {nameof(DefaultConsoleHelper)}.{nameof(CreateDbManagerAsync)}");
                }

                connection.ConnectionString = connectionString;

                await connection.OpenAsync(cancellationToken);

                var db = new DbManager(dataProvider, connection);

                connection = null;
                return db;
            }
            finally
            {
                if (connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }


        public static Func<CancellationToken, Task<DbManager>> CreateAsyncDbManagerFunc(
            string configurationString,
            string databaseNameOverride)
        {
            (ConfigurationDataProvider configurationDataProvider, ConfigurationConnection configurationConnection) =
                ConfigurationManager.Default.Configuration
                    .GetConfigurationDataProvider(configurationString);

            DbProviderFactory factory = ConfigurationManager
                .GetConfigurationDataProviderFromType(configurationConnection.DataProvider)
                .GetDbProviderFactory();

            DbConnectionStringBuilder connectionBuilder = factory.CreateConnectionStringBuilder();
            if (connectionBuilder is null)
            {
                throw new InvalidOperationException($"Connection builder is null in method {nameof(DefaultConsoleHelper)}.{nameof(CreateAsyncDbManagerFunc)}");
            }

            connectionBuilder.ConnectionString = configurationConnection.ConnectionString;

            if (!string.IsNullOrEmpty(databaseNameOverride))
            {
                connectionBuilder["Database"] = databaseNameOverride;
            }

            string connectionString = connectionBuilder.ToString();
            IDataProvider dataProvider = configurationDataProvider.GetDataProvider(connectionString);

            return async cancellationToken =>
            {
                DbConnection connection = null;

                try
                {
                    connection = factory.CreateConnection();
                    if (connection is null)
                    {
                        throw new InvalidOperationException($"Connection is null in method {nameof(DefaultConsoleHelper)}.{nameof(CreateAsyncDbManagerFunc)}");
                    }

                    connection.ConnectionString = connectionString;

                    await connection.OpenAsync(cancellationToken);

                    var db = new DbManager(dataProvider, connection);

                    connection = null;
                    return db;
                }
                finally
                {
                    if (connection != null)
                    {
                        await connection.DisposeAsync();
                    }
                }
            };
        }


        public static string EscapeStringFromConsole(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            StringBuilder sb = StringBuilderHelper.Acquire(s.Length);
            char[] hexNumber = new char[4];
            int hexIndex = 0;
            bool escapeMode = false;

            foreach (char c in s)
            {
                if (escapeMode)
                {
                    bool continueEscaping = false;

                    switch (c)
                    {
                        case '\\':
                            sb.Append('\\');
                            break;

                        case 'n':
                            sb.Append('\n');
                            break;

                        case 'r':
                            sb.Append('\r');
                            break;

                        case 't':
                            sb.Append('\t');
                            break;

                        case 'q':
                            sb.Append('\"');
                            break;

                        default:
                            if (char.IsDigit(c) || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F')
                            {
                                hexNumber[hexIndex++] = char.ToLowerInvariant(c);

                                if (hexIndex == hexNumber.Length)
                                {
                                    // парсинг 16-ричного числа без ведущего 0x
                                    try
                                    {
                                        uint code = uint.Parse(new string(hexNumber), NumberStyles.HexNumber);
                                        sb.Append((char) code);
                                    }
                                    catch (Exception)
                                    {
                                        // значит не число, а просто какие-то числобуквы
                                        for (int i = 0; i < hexIndex; i++)
                                        {
                                            sb.Append(hexNumber[i]);
                                        }
                                    }

                                    // ниже выход из эскейпинга со сбросом hexIndex
                                }
                                else
                                {
                                    // ждём следующих цифр
                                    continueEscaping = true;
                                }
                            }
                            else
                            {
                                // непонятное записываем as-is вместе с "задержанными" символами из hexNumber, потом выходим из режима
                                for (int i = 0; i < hexIndex; i++)
                                {
                                    sb.Append(hexNumber[i]);
                                }

                                sb.Append(c);
                            }

                            break;
                    }

                    if (!continueEscaping)
                    {
                        escapeMode = false;
                        hexIndex = 0;
                    }
                }
                else if (c == '\\')
                {
                    escapeMode = true;
                }
                else
                {
                    sb.Append(c);
                }
            }

            // "задержанные" цифры, которых не дождались, и строка закончилась
            for (int i = 0; i < hexIndex; i++)
            {
                sb.Append(hexNumber[i]);
            }

            return sb.ToStringAndRelease();
        }


        public static async Task<string> ExecuteReaderAndReturnCsvAsync(
            DbManager db,
            char separator = ';',
            int rowCount = 0,
            bool showHeaders = false,
            CancellationToken cancellationToken = default)
        {
            MemoryStream memoryStream = null;
            CsvWriter csvWriter = null;
            DbDataReader reader = null;
            StreamReader streamReader = null;

            try
            {
                memoryStream = new MemoryStream();
                csvWriter = new CsvWriter(memoryStream, Encoding.UTF8, 65535, leaveOpen: true) { NewLine = "\n", Separator = separator };

                reader = await db.ExecuteReaderAsync(cancellationToken);
                int rowIndex = 0;

                while (await reader.ReadAsync(cancellationToken))
                {
                    if (rowCount > 0 && rowIndex >= rowCount)
                    {
                        break;
                    }

                    int fieldCount = reader.FieldCount;
                    if (showHeaders && rowIndex == 0)
                    {
                        for (int i = 0; i < fieldCount; i++)
                        {
                            if (i > 0)
                            {
                                await csvWriter.WriteSeparatorAsync();
                            }

                            await csvWriter.WriteAsync(reader.GetName(i));
                        }

                        await csvWriter.WriteLineAsync();
                    }

                    if (rowIndex++ > 0)
                    {
                        await csvWriter.WriteLineAsync();
                    }

                    for (int i = 0; i < fieldCount; i++)
                    {
                        if (i > 0)
                        {
                            await csvWriter.WriteSeparatorAsync();
                        }

                        string value;
                        if (reader.IsDBNull(i))
                        {
                            value = null;
                        }
                        else
                        {
                            object obj = reader[i];
                            value = obj is DateTime dateTime
                                ? FormattingHelper.FormatDateTime(dateTime, convertToLocal: false)
                                : FormattingHelper.FormatToString(obj);
                        }

                        csvWriter.Write(value);
                    }
                }

                await reader.DisposeAsync();
                reader = null;

                await csvWriter.DisposeAsync();
                csvWriter = null;

                memoryStream.Position = 0L;

                streamReader = new StreamReader(memoryStream, Encoding.UTF8);
                return await streamReader.ReadToEndAsync();
            }
            finally
            {
                streamReader?.Dispose();

                if (reader != null)
                {
                    await reader.DisposeAsync();
                }

                if (csvWriter != null)
                {
                    await csvWriter.DisposeAsync();
                }

                if (memoryStream != null)
                {
                    await memoryStream.DisposeAsync();
                }
            }
        }


        public static void AddOperationToValidationResult(
            string operationPrefix,
            string cardName,
            ValidationResult result,
            IValidationResultBuilder validationResult,
            bool ignoreExistentCards = false,
            bool ignoreRepairMessages = false)
        {
            if (result.Items.Count == 0)
            {
                return;
            }

            if (ignoreExistentCards
                && result.Items.Any(x => CardValidationKeys.IsCardExists(x.Key)))
            {
                return;
            }

            ValidationResult actualResult = result;

            if (ignoreRepairMessages && result.IsSuccessful)
            {
                ValidationResultItem[] newItems = result.Items
                    .Where(x => !CardValidationKeys.IsCardRepair(x.Key))
                    .ToArray();

                if (newItems.Length == 0)
                {
                    actualResult = ValidationResult.Empty;
                }
                else if (newItems.Length < result.Items.Count)
                {
                    actualResult = new ValidationResult(newItems);
                }

                if (actualResult.Items.Count == 0)
                {
                    return;
                }
            }

            string details = actualResult.ToString(ValidationLevel.Detailed);

            if (actualResult.HasErrors)
            {
                ValidationSequence
                    .Begin(validationResult)
                    .SetObjectName(typeof(DefaultConsoleHelper))
                    .ErrorDetails(
                        string.Format(
                            LocalizationManager.GetString(operationPrefix)
                            + " \"{0}\" "
                            + LocalizationManager.GetString("UI_Cards_HasErrorsSuffix"),
                            cardName),
                        details)
                    .End();
            }
            else if (actualResult.HasWarnings)
            {
                ValidationSequence
                    .Begin(validationResult)
                    .SetObjectName(typeof(DefaultConsoleHelper))
                    .WarningDetails(
                        string.Format(
                            LocalizationManager.GetString(operationPrefix)
                            + " \"{0}\" "
                            + LocalizationManager.GetString("UI_Cards_HasWarningsSuffix"),
                            cardName),
                        details)
                    .End();
            }
            else
            {
                ValidationSequence
                    .Begin(validationResult)
                    .SetObjectName(typeof(DefaultConsoleHelper))
                    .InfoDetails(
                        string.Format(
                            LocalizationManager.GetString(operationPrefix)
                            + " \"{0}\" "
                            + LocalizationManager.GetString("UI_Cards_HasInfoSuffix"),
                            cardName),
                        details)
                    .End();
            }
        }


        public static StringBuilder GetQuotedItemsText(
            StringBuilder builder,
            string singleMessage,
            string multipleMessage,
            ICollection<string> items)
        {
            if (items.Count == 0)
            {
                return builder;
            }

            if (items.Count == 1)
            {
                return builder
                    .Append(LocalizationManager.GetString(singleMessage))
                    .Append(" \"")
                    .Append(items.First())
                    .Append("\".");
            }

            builder
                .Append(LocalizationManager.GetString(multipleMessage))
                .Append(" (")
                .Append(items.Count)
                .Append("):");

            foreach (string item in items)
            {
                builder
                    .AppendLine()
                    .Append('"')
                    .Append(item)
                    .Append('"');
            }

            return builder;
        }


        public static void RemoveCardInfoListDuplicates(List<CardInfo> cardInfoList)
        {
            for (int lastIndex = cardInfoList.Count - 1; lastIndex > 0; lastIndex--)
            {
                Guid cardID = cardInfoList[lastIndex].CardID;

                int firstIndex = -1;
                for (int i = 0; i < cardInfoList.Count; i++)
                {
                    if (cardInfoList[i].CardID == cardID)
                    {
                        firstIndex = i;
                        break;
                    }
                }

                if (firstIndex >= 0 && firstIndex != lastIndex)
                {
                    // lastIndex является дублем, удаляем
                    cardInfoList.RemoveAt(lastIndex);
                }
            }
        }


        public static async Task<List<CardInfo>> TryParseCardInfoListAsync(
            IEnumerable<string> identifiers,
            IConsoleLogger logger,
            string separatorChar = ";",
            CultureInfo localizationCulture = null,
            CancellationToken cancellationToken = default)
        {
            string allIdentifiers = string.Join(" ", identifiers ?? Enumerable.Empty<string>());

            try
            {
                if (!string.IsNullOrWhiteSpace(allIdentifiers))
                {
                    return allIdentifiers
                        .Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => new CardInfo(Guid.Parse(x.Trim())))
                        .ToList();
                }

                if (string.IsNullOrEmpty(separatorChar))
                {
                    separatorChar = ";";
                }
                else if (separatorChar.Length > 1)
                {
                    separatorChar = separatorChar[0].ToString();
                }

                await logger.InfoAsync(
                    "Cards identifiers are expected from console input or previous chained command.{0}" +
                    "Place each id on a new line, use new line with EOF ({1}) to complete input.{0}" +
                    "Provide card name after separator \"{2}\" (optional). Ctrl+C to cancel.",
                    Environment.NewLine,
                    EnvironmentHelper.IsLinux ? "Enter, Ctrl+D, Ctrl+D" : "Enter, Ctrl+Z, Enter",
                    separatorChar);

                var cardList = new List<CardInfo>();
                string input = (await System.Console.In.ReadToEndAsync()).Trim();

                if (!string.IsNullOrEmpty(input))
                {
                    // разделитель между колонками, по умолчанию ";"

                    var configuration = new Configuration(CultureInfo.InvariantCulture) { Delimiter = separatorChar };

                    using var stringReader = new StringReader(input);
                    using var csvReader = new CsvReader(stringReader, configuration);
                    int length = 0;

                    while (await csvReader.ReadAsync())
                    {
                        if (length == 0)
                        {
                            length = csvReader.Context.Record.Length;
                            if (length == 0)
                            {
                                break;
                            }
                        }

                        if (csvReader.TryGetField(0, out string id)
                            && !string.IsNullOrEmpty(id = id?.Trim())
                            && Guid.TryParse(id, out Guid cardID))
                        {
                            cardList.Add(
                                new CardInfo(
                                    cardID,
                                    length > 1 && csvReader.TryGetField(1, out string cardName)
                                        ? cardName
                                        : null));
                        }
                    }
                }

                return cardList;
            }
            catch (Exception e)
            {
                await logger.LogExceptionAsync("Error parsing identifiers", e);
                return null;
            }
        }

        #endregion
    }
}