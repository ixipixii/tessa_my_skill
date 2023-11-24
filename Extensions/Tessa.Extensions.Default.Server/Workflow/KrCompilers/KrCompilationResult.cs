using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tessa.Compilation;
using Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI;
using Tessa.Platform;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers
{
    /// <summary>
    /// Результат компиляции KrCompiler'a
    /// </summary>
    [Serializable]
    public sealed class KrCompilationResult : IKrCompilationResult
    {
        #region fields

        [NonSerialized]
        private readonly Lazy<IDictionary<string, Func<IKrScript>>> typesFactories;

        #endregion

        #region constructor

        private KrCompilationResult()
        {
            this.typesFactories = new Lazy<IDictionary<string, Func<IKrScript>>>(
                this.BuildTypesCache, 
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public KrCompilationResult(
            ICompilationResult result,
            ValidationResult validationResult) : this()
        {
            this.Result = result;
            this.ValidationResult = validationResult;
        }
        
        #endregion

        #region implementation

        /// <inheritdoc />
        public ICompilationResult Result { get; }

        /// <inheritdoc />
        public ValidationResult ValidationResult { get; }

        /// <inheritdoc />
        public IKrScript CreateInstance(
            string prefix,
            string alias,
            Guid typeID)
        {
            var key = KrCompilersHelper.FormatClassName(prefix, alias, typeID);
            try
            {
                return this.typesFactories.Value[key]();
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"Type \"{key}\" wasn't compiled.");
            }
        }

        #endregion

        #region private

        private IDictionary<string, Func<IKrScript>> BuildTypesCache()
        {
            if (this.Result.Assembly == null)
            {
                return new Dictionary<string, Func<IKrScript>>();
            }
            var types = this.Result
                .Assembly
                .GetTypes()
                .Where(a => a.Implements<IKrScript>() && !a.IsAbstract);

            var typesCache = new Dictionary<string, Func<IKrScript>>();
            foreach (var type in types)
            {
                if (KrCompilersHelper.CorrectClassName(type.Name))
                {
                    typesCache.Add(
                        type.Name,
                        () => (IKrScript)Activator.CreateInstance(type));
                }
            }
            return typesCache;
        }

        #endregion
    }
}
