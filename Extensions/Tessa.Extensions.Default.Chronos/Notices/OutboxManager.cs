using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Chronos.Notices
{
    public class OutboxManager : IOutboxManager
    {
        #region Fields

        private readonly IDbScope dbScope;

        #endregion

        #region Constructors

        public OutboxManager(IDbScope dbScope)
        {
            this.dbScope = dbScope;
        }

        #endregion

        #region Private Methods

        private static OutboxMessage ToOutboxMessage(IDataRecord record)
        {
            return new OutboxMessage
            {
                ID = record.GetGuid(0),
                Email = record.GetValue<string>(2),
                Subject = record.GetValue<string>(3),
                Body = record.GetValue<string>(4),
                Attempts = record.GetInt32(5),
                Info = record.GetValue<byte[]>(6)
            };
        }

        #endregion

        #region IOutboxManager Items

        public async Task<ConcurrentQueue<OutboxMessage>> GetTopMessagesAsync(
            int topCount,
            int retryIntervalMinutes,
            CancellationToken cancellationToken = default)
        {
            await using (this.dbScope.Create())
            {
                var db = this.dbScope.Db;
                var builderFactory = this.dbScope.BuilderFactory;
                var maxLastAttemptDateToProcess = DateTime.UtcNow - TimeSpan.FromMinutes(retryIntervalMinutes);


                var result = new ConcurrentQueue<OutboxMessage>();
                await using (DbDataReader reader = await db
                    .SetCommand(
                        builderFactory
                            .Select().Top(topCount).C(null,
                                "ID", "AddDateUTC", "Email", "Subject",
                                "Body", "Attempts", "Info")
                            .From("Outbox").NoLock()
                            .Where().C("Attempts").Equals().V(0)
                            .Or().C("LastAttemptDateUTC").LessOrEquals().P("MaxLastAttemptDateToProcess")
                            .OrderBy("AddDateUTC")
                            .Limit(topCount)
                            .Build(),
                        db.Parameter("MaxLastAttemptDateToProcess", maxLastAttemptDateToProcess))
                    .ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        OutboxMessage message = ToOutboxMessage(reader);
                        result.Enqueue(message);
                    }
                }

                return result;
            }
        }

        public async Task MarkAsBadMessageAsync(
            Guid id,
            long attemptNum,
            string exceptionMessage,
            CancellationToken cancellationToken = default)
        {
            await using (this.dbScope.Create())
            {
                var db = this.dbScope.Db;
                var builderFactory = this.dbScope.BuilderFactory;

                await db
                    .SetCommand(
                        builderFactory
                            .Update("Outbox")
                            .C("Attempts").Assign().P("Attempts")
                            .C("LastAttemptDateUTC").Assign().P("LastAttemptDateUTC")
                            .C("LastExceptionMessage").Assign().P("LastExceptionMessage")
                            .Where().C("ID").Equals().P("ID")
                            .Build(),
                        db.Parameter("Attempts", attemptNum),
                        db.Parameter("LastAttemptDateUTC", DateTime.UtcNow),
                        db.Parameter("LastExceptionMessage", SqlHelper.Nullable(exceptionMessage, 254)),
                        db.Parameter("ID", id))
                    .LogCommand()
                    .ExecuteNonQueryAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await using (this.dbScope.Create())
            {
                var db = this.dbScope.Db;
                var builderFactory = this.dbScope.BuilderFactory;

                await db
                    .SetCommand(
                        builderFactory
                            .DeleteFrom("Outbox")
                            .Where().C("ID").Equals().P("ID")
                            .Build(),
                        db.Parameter("ID", id))
                    .LogCommand()
                    .ExecuteNonQueryAsync(cancellationToken);
            }
        }

        #endregion
    }
}