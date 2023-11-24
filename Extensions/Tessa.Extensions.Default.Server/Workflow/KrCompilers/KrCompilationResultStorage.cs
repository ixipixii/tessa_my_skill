using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using LinqToDB.Data;
using Tessa.Compilation;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants.KrStageBuildOutput;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public sealed class KrCompilationResultStorage : IKrCompilationResultStorage
    {
        private readonly IDbScope dbScope;

        private readonly ISession session;

        public KrCompilationResultStorage(
            IDbScope dbScope,
            ISession session)
        {
            this.dbScope = dbScope;
            this.session = session;
        }

        /// <inheritdoc />
        public void Upsert(
            Guid cardID,
            IKrCompilationResult compilationResult,
            bool withCompilationResult = false)
        {
            var assemblyName = compilationResult.Result.Assembly != null
                ? compilationResult.Result.Assembly.FullName + Environment.NewLine
                : string.Empty;

            byte[] assemblyBytes = null;
            byte[] resultBytes = null;
            if (withCompilationResult)
            {
                if (!string.IsNullOrEmpty(assemblyName))
                {
                    using (var stream = new MemoryStream(8096))
                    {
                        var formatter = new BinaryFormatter();
                        formatter.Serialize(stream, compilationResult);
                        resultBytes = stream.ToArray();
                    }

                    assemblyBytes = compilationResult.Result.AssemblyBytes;
                }
                else
                {
                    // компиляция завершена с ошибками, байты от сборки нельзя поместить в базу
                    withCompilationResult = false;
                }
            }

            // Отдельное соединение, т.к. нужно записать результат, даже если внешняя транзакция будет откатана
            using (this.dbScope.CreateNew())
            {
                var db = this.dbScope.Db;
                var builder = this.dbScope.BuilderFactory;
                string[] parameterNames;
                DataParameter[] parameters;
                if (withCompilationResult)
                {
                    parameterNames = new[]
                    {
                        KrConstants.ID,
                        BuildDateTime,
                        Output,
                        KrConstants.KrStageBuildOutput.Assembly,
                        KrConstants.KrStageBuildOutput.CompilationResult
                    };
                    parameters = new[]
                    {
                        db.Parameter(KrConstants.ID, cardID),
                        db.Parameter(BuildDateTime, compilationResult.Result.CompilationDateTime.ToUniversalTime()),
                        db.Parameter(Output, assemblyName + compilationResult.Result.RawOutput),
                        db.Parameter(KrConstants.KrStageBuildOutput.Assembly, assemblyBytes),
                        db.Parameter(KrConstants.KrStageBuildOutput.CompilationResult, resultBytes),
                    };
                }
                else
                {
                    parameterNames = new[]
                    {
                        KrConstants.ID,
                        BuildDateTime,
                        Output,
                    };
                    parameters = new[]
                    {
                        db.Parameter(KrConstants.ID, cardID),
                        db.Parameter(BuildDateTime, compilationResult.Result.CompilationDateTime.ToUniversalTime()),
                        db.Parameter(Output, assemblyName + compilationResult.Result.RawOutput),
                    };
                }

                if (this.dbScope.Dbms == Dbms.SqlServer)
                {
                    var updateQuery = builder
                        .Update(Name)
                        .C(KrConstants.ID).Assign().P(KrConstants.ID)
                        .C(BuildDateTime).Assign().P(BuildDateTime)
                        .C(Output).Assign().P(Output);
                    if (withCompilationResult)
                    {
                        updateQuery
                            .C(KrConstants.KrStageBuildOutput.Assembly).Assign().P(KrConstants.KrStageBuildOutput.Assembly)
                            .C(KrConstants.KrStageBuildOutput.CompilationResult).Assign().P(KrConstants.KrStageBuildOutput.CompilationResult);
                    }

                    updateQuery.Where().C(KrConstants.ID).Equals().P(KrConstants.ID);

                    var rowsAffected = db
                        .SetCommand(updateQuery.Build(), parameters)
                        .LogCommand()
                        .ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        var insertQuery = builder
                            .InsertInto(Name, parameterNames)
                            .Values(v => v.P(parameterNames)).N()
                            .Build();
                        db
                            .SetCommand(insertQuery, parameters)
                            .LogCommand()
                            .ExecuteNonQuery();
                    }
                }
                else if (this.dbScope.Dbms == Dbms.PostgreSql)
                {
                    var query = builder
                        .InsertInto(Name, parameterNames)
                        .Values(v => v.P(parameterNames)).N()
                        .Q("ON CONFLICT (").C(KrConstants.ID).Q(") DO UPDATE SET ")
                        .C(BuildDateTime).Assign().Q($" EXCLUDED.\"{BuildDateTime}\"").RequireComma()
                        .C(Output).Assign().Q($" EXCLUDED.\"{Output}\"").RequireComma();

                    if (withCompilationResult)
                    {
                        query
                            .C(KrConstants.KrStageBuildOutput.Assembly).Assign().Q($" EXCLUDED.\"{KrConstants.KrStageBuildOutput.Assembly}\"").RequireComma()
                            .C(KrConstants.KrStageBuildOutput.CompilationResult).Assign().Q($" EXCLUDED.\"{KrConstants.KrStageBuildOutput.CompilationResult}\"");
                    }

                    db
                        .SetCommand(query.Build(), parameters)
                        .LogCommand()
                        .ExecuteNonQuery();
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        /// <inheritdoc />
        public IKrCompilationResult GetCompilationResult(
            Guid cardID)
        {
            byte[] rawAssembly;
            byte[] rawResult;

            using (this.dbScope.Create())
            {
                var db = this.dbScope.Db;
                var builder = this.dbScope.BuilderFactory
                    .Select().C(
                        null,
                        KrConstants.KrStageBuildOutput.Assembly,
                        KrConstants.KrStageBuildOutput.CompilationResult)
                    .From(Name)
                    .Where().C(KrConstants.ID).Equals().P("ID");
                db
                    .SetCommand(
                        builder.Build(),
                        db.Parameter("ID", cardID))
                    .LogCommand();
                using var reader = db.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }

                rawAssembly = reader.GetNullableBytes(0);
                rawResult = reader.GetNullableBytes(1);
            }

            if (rawResult is null)
            {
                return null;
            }

            IKrCompilationResult krDeserializedResult;
            try
            {
                var formatter = new BinaryFormatter();
                using var stream = new MemoryStream(rawResult);
                krDeserializedResult = (IKrCompilationResult) formatter.Deserialize(stream);
            }
            catch
            {
                return null;
            }

            Assembly deserializedAssembly = null;
            try
            {
                if (rawAssembly != null)
                {
                    deserializedAssembly = System.Reflection.Assembly.Load(rawAssembly);
                }
            }
            catch
            {
                return null;
            }

            var deserializedResult = krDeserializedResult.Result;
            var compilationResult = new CompilationResult(
                deserializedResult.AssemblyID,
                null,
                deserializedResult.BuildVersion,
                deserializedResult.BuildDate,
                deserializedAssembly,
                deserializedResult.ValidationResult,
                deserializedResult.CompilerOutput,
                deserializedResult.RawOutput,
                deserializedResult.CompilerReturnValue,
                deserializedResult.Info.GetStorage()
            );
            return new KrCompilationResult(
                compilationResult,
                krDeserializedResult.ValidationResult);
        }

        /// <inheritdoc />
        public KrCompilationOutput GetCompilationOutput(
            Guid cardID)
        {
            var localBuildOutput = string.Empty;
            var globalBuildOutput = string.Empty;
            using (this.dbScope.Create())
            {
                var db = this.dbScope.Db;
                var builder = this.dbScope.BuilderFactory
                    .Select()
                    .C(null,
                        KrConstants.ID,
                        BuildDateTime,
                        Output)
                    .From(Name)
                    .Where().C(KrConstants.ID).Equals().P("ID")
                    .Or().C(KrConstants.ID).Equals().V(Guid.Empty);

                using var reader = db
                    .SetCommand(
                        builder.Build(),
                        db.Parameter("id", cardID))
                    .LogCommand()
                    .ExecuteReader();
                while (reader.Read())
                {
                    var utcDateTime = reader.GetDateTimeUtc(1);
                    var compilerOutput = reader.GetString(2);
                    var text = FormattingHelper.FormatDateTimeWithoutSeconds(utcDateTime + this.session.ClientUtcOffset, false)
                        + Environment.NewLine
                        + compilerOutput;

                    if (reader.GetGuid(0) == Guid.Empty)
                    {
                        globalBuildOutput = text;
                    }
                    else
                    {
                        localBuildOutput = text;
                    }
                }
            }

            return new KrCompilationOutput { Local = localBuildOutput, Global = globalBuildOutput };
        }

        /// <inheritdoc />
        public void DeleteCompilationResult(
            Guid cardID)
        {
            using (this.dbScope.CreateNew())
            {
                var query = this.dbScope.BuilderFactory
                    .Update(Name)
                    .C(KrConstants.KrStageBuildOutput.Assembly).Assign().V(null)
                    .C(KrConstants.KrStageBuildOutput.CompilationResult).Assign().V(null)
                    .Where().C(KrConstants.ID).Equals().P("ID")
                    .Build();
                var db = this.dbScope.Db;
                db
                    .SetCommand(query, db.Parameter("ID", cardID))
                    .LogCommand()
                    .ExecuteNonQuery();
            }
        }

        /// <inheritdoc />
        public void Delete(
            Guid cardID)
        {
            using (this.dbScope.Create())
            {
                var query = this.dbScope.BuilderFactory
                    .DeleteFrom(Name)
                    .Where().C(KrConstants.ID).Equals().P("ID")
                    .Build();
                var db = this.dbScope.Db;
                db
                    .SetCommand(query, db.Parameter("ID", cardID))
                    .LogCommand()
                    .ExecuteNonQuery();
            }
        }
    }
}