using System;
using System.Globalization;
using System.Windows.Data;

namespace BonusBits.CodeSamples.WP7.Design.Converters
{
    /// <summary>
    /// http://www.cookcomputing.com/blog/archives/000613.html
    /// </summary>
    public abstract class BaseValueConverter<V, T, P> : IValueConverter
    {
        protected virtual T Convert(V value, P parameter, CultureInfo culture)
        {
            throw new NotImplementedException(GetType().Name + "Convert not implemented");
        }

        protected virtual V ConvertBack(T value, P parameter, CultureInfo culture)
        {
            throw new NotImplementedException(GetType().Name + "ConvertBack not implemented");
        }

        #region IValueConverter Members
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target
        /// dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public Object Convert(Object value, Type targetType, Object parameter,
            CultureInfo culture)
        {
            if (value.GetType() != typeof(V))
            {
                throw new ArgumentException(GetType().Name
                    + ".Convert: value type not " + typeof(V).Name);
            }

            if (targetType != typeof(T))
            {
                throw new ArgumentException(GetType().Name
                  + ".Convert: target type not " + typeof(T).Name);
            }

            return Convert((V)value, (P)parameter, culture);
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called
        /// only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source
        /// object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public Object ConvertBack(Object value, Type targetType, Object parameter,
            CultureInfo culture)
        {
            if (value.GetType() != typeof(T))
            {
                throw new ArgumentException(GetType().Name
                  + ".ConvertBack: value type not " + typeof(T).Name);
            }

            if (targetType != typeof(V))
            {
                throw new ArgumentException(GetType().Name
                  + ".ConvertBack: target type not " + typeof(V).Name);
            }

            return ConvertBack((T)value, (P)parameter, culture);
        }
        #endregion
    }

    public abstract class BaseValueConverter<V, T> : BaseValueConverter<V, T, Object>
    {
        protected virtual T Convert(V value, CultureInfo culture)
        {
            throw new NotImplementedException(GetType().Name + "Convert not implemented");
        }

        protected virtual V ConvertBack(T value, CultureInfo culture)
        {
            throw new NotImplementedException(GetType().Name + "ConvertBack not implemented");
        }

        protected sealed override T Convert(V value, Object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                throw new ArgumentException(GetType().Name
                    + ".Convert: binding contains unexpected parameter");
            }

            return Convert(value, culture);
        }

        protected sealed override V ConvertBack(T value, Object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                throw new ArgumentException(GetType().Name
                  + ".ConvertBack: binding contains unexpected parameter");
            }

            return ConvertBack(value, culture);
        }
    }
}
