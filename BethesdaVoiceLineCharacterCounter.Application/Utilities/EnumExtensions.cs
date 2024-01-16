using System.ComponentModel;

namespace BethesdaVoiceLineCharacterCounter.Application.Utilities
{
    /// <summary>
    /// This class will provide functionality for extended Enum functionality.
    /// </summary>
    internal static class EnumExtensions
    {
        /// <summary>
        /// This method will return the Enum description, if present.
        /// </summary>
        /// <param name="value">The passed in Enum</param>
        /// <returns>A description if present, otherwise the enum itself as a string.</returns>
        public static string GetEnumDescription(Enum value)
        {
            DescriptionAttribute[] customAttributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return customAttributes != null && customAttributes.Length > 0 ? customAttributes[0].Description : value.ToString();
        }
    }
}
