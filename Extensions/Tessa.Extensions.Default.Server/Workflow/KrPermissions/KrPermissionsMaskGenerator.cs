using Tessa.Cards;
using Tessa.Cards.Metadata;
using Tessa.Views;

namespace Tessa.Extensions.Default.Server.Workflow.KrPermissions
{
    /// <inheritdoc />
    public sealed class KrPermissionsMaskGenerator : IKrPermissionsMaskGenerator
    {
        #region IKrPermissionsMaskGenerator Implementation

        /// <inheritdoc />
        public object GenerateMaskValue(
            Card card,
            CardSection section,
            CardRow row,
            CardMetadataColumn columnMeta,
            object originalValue,
            string defaultMask)
        {
            if (!string.IsNullOrWhiteSpace(defaultMask))
            {
                var type = columnMeta.MetadataType.RuntimeType;
                if (type == typeof(string))
                {
                    return defaultMask;
                }
                else if (columnMeta.IsReference)
                {
                    return DefaultValues.TryGetDefaultValue(type).Item2;
                }
            }
            return null;
        }

        #endregion
    }
}
