using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Platform.ConsoleApps;
using Tessa.Roles;

namespace Tessa.Extensions.Default.Console.ImportTypes
{
    public sealed class Operation : ConsoleOperation<OperationContext>
    {
        private readonly ICardTypeClientRepository cardTypeClientRepository;

        public Operation(
            ConsoleSessionManager sessionManager,
            IConsoleLogger logger,
            ICardTypeClientRepository cardTypeClientRepository)
            : base(logger, sessionManager)
        {
            this.cardTypeClientRepository = cardTypeClientRepository;
        }

        /// <inheritdoc />
        public override async Task<int> ExecuteAsync(OperationContext context, CancellationToken cancellationToken = default)
        {
            if (!this.SessionManager.IsOpened)
            {
                return -1;
            }

            try
            {
                await this.Logger.InfoAsync("Importing types from: \"{0}\"", context.Source);

                var typesToImport = new List<CardType>();

                foreach (string typeFile in DefaultConsoleHelper.GetSourceFiles(context.Source, "*.jtype", throwIfNotFound: false))
                {
                    await this.Logger.InfoAsync("Reading type from: \"{0}\"", typeFile);

                    string text = await File.ReadAllTextAsync(typeFile, cancellationToken);

                    var cardType = new CardType();
                    cardType.DeserializeFromJson(text);

                    typesToImport.Add(cardType);
                }

                foreach (string typeFile in DefaultConsoleHelper.GetSourceFiles(context.Source, "*.tct", throwIfNotFound: false))
                {
                    await this.Logger.InfoAsync("Reading type from: \"{0}\"", typeFile);

                    await using FileStream fileStream = File.OpenRead(typeFile);
                    var cardType = new CardType();
                    cardType.DeserializeFromXml(fileStream);

                    typesToImport.Add(cardType);
                }

                if (typesToImport.Count == 0)
                {
                    throw new FileNotFoundException($"Couldn't locate *.jtype or *.tct file in \"{context.Source}\"", context.Source);
                }

                var cardTypesToDelete = new List<CardType>();
                if (context.ClearTypes)
                {
                    // тип карточки "Сотрудник" никогда не будем удалять
                    var doNotRemoveTypes = new HashSet<Guid> { RoleHelper.PersonalRoleTypeID };

                    // также не удаляем все типы, которые импортируются (они будут заменены)
                    foreach (CardType type in typesToImport)
                    {
                        doNotRemoveTypes.Add(type.ID);
                    }

                    var allTypes = (await this.cardTypeClientRepository.GetAllCardTypesAsync(cancellationToken)).ToArray();
                    foreach (CardType type in allTypes)
                    {
                        if (!doNotRemoveTypes.Contains(type.ID))
                        {
                            cardTypesToDelete.Add(type);
                        }
                    }
                }

                if (typesToImport.Count > 0 || cardTypesToDelete.Count > 0)
                {
                    await this.Logger.InfoAsync("Importing types ({0})", typesToImport.Count);

                    if (cardTypesToDelete.Count > 0)
                    {
                        await this.Logger.InfoAsync(
                            "Removing types ({0}): {1}",
                            cardTypesToDelete.Count,
                            string.Join(", ", cardTypesToDelete.Select(x => "\"" + x.Name + "\"")));
                    }

                    await this.cardTypeClientRepository.StoreManyAsync(
                        typesToImport,
                        cardTypesToDelete.Select(type => type.ID).ToList(),
                        cancellationToken);
                }
            }
            catch (Exception e)
            {
                await this.Logger.LogExceptionAsync("Error importing types", e);
                return -1;
            }

            await this.Logger.InfoAsync("Types are imported successfully");
            return 0;
        }
    }
}
