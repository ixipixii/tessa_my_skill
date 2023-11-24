using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Server.Workflow.KrProcess.Scope;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.SqlProcessing
{
    public sealed class KrSqlExecutor: IKrSqlExecutor
    {
        #region fields

        private readonly Func<IKrSqlPreprocessor> getSqlPreprocessor;

        private readonly IDbScope dbScope;

        private readonly IKrScope krScope;

        #endregion

        #region constructor

        public KrSqlExecutor(
            Func<IKrSqlPreprocessor> getSqlPreprocessor,
            IDbScope dbScope,
            IKrScope krScope)
        {
            this.getSqlPreprocessor = getSqlPreprocessor;
            this.dbScope = dbScope;
            this.krScope = krScope;
        }

        #endregion

        #region implementation

        /// <inheritdoc />
        public bool ExecuteCondition(IKrSqlExecutorContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Query))
            {
                return true;
            }

            var sqlPreprocessorResult = this.getSqlPreprocessor().Preprocess(context);

            try
            {
                using (this.dbScope.Create())
                {
                    var db = this.dbScope.Db;
                    db.SetCommand(
                        sqlPreprocessorResult.Query,
                        sqlPreprocessorResult.Parameters.Select(p => db.Parameter(p.Key, p.Value)).ToArray());

                    bool result = false;
                    using (var reader = db.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.FieldCount != 1)
                            {
                                ThrowException(
                                    null,
                                    "$KrProcess_ErrorMessage_SqlConditionTooManyColumns",
                                    sqlPreprocessorResult,
                                    context);
                            }

                            var value = reader.GetValue<object>(0);
                            if (value == null || value.Equals(0) || value.Equals(false))
                            {
                                // ReSharper disable once RedundantAssignment
                                result = false;
                            }
                            else if (value.Equals(1) || value.Equals(true))
                            {
                                result = true;
                            }
                            else
                            {
                                ThrowException(
                                    null,
                                    "$KrProcess_ErrorMessage_SqlConditionTooManyRows",
                                    sqlPreprocessorResult,
                                    context);
                            }
                        }
                        else
                        {
                            // ReSharper disable once RedundantAssignment
                            result = false;
                        }

                        if (reader.Read())
                        {
                            ThrowException(
                                null,
                                "$KrProcess_ErrorMessage_SqlPerformersError",
                                sqlPreprocessorResult,
                                context);
                        }
                    }

                    return result;
                }
            }
            catch (Exception e)
            {
                if (e is QueryExecutionException)
                {
                    throw;
                }
                ThrowException(
                    e,
                    "$KrProcess_ErrorMessage_SqlPerformersError",
                    sqlPreprocessorResult,
                    context);
            }

            return false;
        }

        /// <inheritdoc />
        public List<Performer> ExecutePerformers(IKrSqlExecutorContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Query))
            {
                return new List<Performer>();
            }
            var sqlPreprocessorResult = this.getSqlPreprocessor().Preprocess(context);

            var roles = new List<Performer>();
            try
            {
                using (this.dbScope.Create())
                {
                    var db = this.dbScope.Db;
                    db.SetCommand(
                        sqlPreprocessorResult.Query,
                        sqlPreprocessorResult.Parameters.Select(p => db.Parameter(p.Key, p.Value)).ToArray());
                    using var reader = db.ExecuteReader();
                    roles.AddRange(ReadPerformers(reader, context.StageRowID, context));
                }
            }
            catch (Exception e)
            {
                if (e is QueryExecutionException)
                {
                    throw;
                }
                ThrowException(
                    e,
                    "$KrProcess_ErrorMessage_SqlPerformersError",
                    sqlPreprocessorResult,
                    context);
            }

            return roles;
        }

        #endregion

        #region private

        private static IEnumerable<Performer> ReadPerformers(IDataReader reader, Guid stageRowID, IKrSqlExecutorContext context)
        {
            // на разных СУБД разные типы, поэтому мы не можем их проверить по GetDataTypeName;
            // читаем первую строку и смотрим

            if (!reader.Read())
            {
                return EmptyHolder<Performer>.Array;
            }

            if (reader.FieldCount != 2
                || !(reader[0] is Guid firstID)
                || !(reader[1] is string firstName))
            {
                var errorText = context.GetErrorTextFunc(
                    context,
                    "$KrProcess_ErrorMessage_IncorrectRoleResultSet",
                    new object[] { });
                throw new QueryExecutionException(errorText, context.Query);
            }

            var result = new List<Performer> { new MultiPerformer(firstID, firstName, stageRowID, isSql: true) };

            while (reader.Read())
            {
                result.Add(new MultiPerformer(reader.GetGuid(0), reader.GetNullableString(1), stageRowID, isSql: true));
            }

            // Проверка есть ли ещё запросы, в т.ч. содержащими ошибки.
            if (reader.NextResult())
            {
                var errorText = context.GetErrorTextFunc(
                    context,
                    "$KrProcess_ErrorMessage_SeveralQueries",
                    new object[0]);
                throw new QueryExecutionException(errorText, context.Query);
            }

            return result;
        }

        /// <summary>
        /// Преобразовать список SQL параметров в одну строку.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string SqlParametersToString(
            IEnumerable<KeyValuePair<string, object>> parameters)
        {
            return string.Join(
                Environment.NewLine,
                parameters.Select(x => $"{x.Key} = {x.Value}"));
        }

        private static void ThrowException(
            Exception e,
            string text,
            IKrSqlPreprocessorResult sqlPreprocessorResult,
            IKrSqlExecutorContext context)
        {
            var query =
                SqlParametersToString(sqlPreprocessorResult.Parameters) + Environment.NewLine +
                sqlPreprocessorResult.Query;

            var errorText = context.GetErrorTextFunc(
                context,
                text,
                new object[] { });

            throw new QueryExecutionException(errorText, query, e);
        }

        #endregion
    }
}