using System;
using System.Globalization;
using System.Windows;

namespace BonusBits.CodeSamples.WP7.Design.Converters {
    /// <summary>
    /// http://www.cookcomputing.com/blog/archives/000613.html
    /// </summary>
    public sealed class BooleanToVisibilityConverter 
        : BaseValueConverter<Boolean, Visibility, String> {
        protected override Visibility Convert(Boolean value, String parameter, 
            CultureInfo culture) {
            if (parameter != null && parameter == "!") {
                value = !value;
            }

            return value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
