using System;
using System.Runtime.Serialization;
using System.Security;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    [Serializable]
    public class StageTypeDescriptorNotInitializedException : Exception
    {
        #region constructors

        public StageTypeDescriptorNotInitializedException(
                Guid descriptorID,
                string caption,
                string notInitializedField)
            : base($"Descriptor ID = {descriptorID}, caption = {caption} has not initialized field {notInitializedField}.")
        {
        }

        public StageTypeDescriptorNotInitializedException(
            string message)
            : base(message)
        {

        }

        public StageTypeDescriptorNotInitializedException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {

        }

        [SecuritySafeCritical]
        protected StageTypeDescriptorNotInitializedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {

        }

        #endregion

    }
}