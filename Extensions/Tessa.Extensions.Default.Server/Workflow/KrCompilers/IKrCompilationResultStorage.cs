using System;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    public interface IKrCompilationResultStorage
    {
        void Upsert(
            Guid cardID,
            IKrCompilationResult compilationResult,
            bool withCompilationResult = false);

        IKrCompilationResult GetCompilationResult(
            Guid cardID);

        KrCompilationOutput GetCompilationOutput(
            Guid cardID);

        void DeleteCompilationResult(
            Guid cardID);

        void Delete(
            Guid cardID);
    }
}