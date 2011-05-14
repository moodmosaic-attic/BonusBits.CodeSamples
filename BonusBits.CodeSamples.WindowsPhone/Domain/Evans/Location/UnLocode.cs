using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BonusBits.CodeSamples.WP7.Domain.Evans.Location
{
    /// <summary>
    /// United nations location code.
    /// 
    /// http://www.unece.org/cefact/locode/
    /// http://www.unece.org/cefact/locode/DocColumnDescription.htm#LOCODE
    /// 
    /// http://dddsample.sourceforge.net/
    /// </summary>
#pragma warning disable 661,660 //Equals and GetHashCode are overridden in ValueObject class.
    public sealed class UnLocode : ValueObject
#pragma warning restore 661,660
    {
        private static readonly Regex m_codePattern = new Regex("[a-zA-Z]{2}[a-zA-Z2-9]{3}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly String m_code;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnLocode"/> class.
        /// </summary>
        public UnLocode() { }

        /// <summary>
        /// Creates new <see cref="UnLocode"/> object.
        /// </summary>
        /// <param name="code">String representation of location code.</param>
        public UnLocode(String code)
        {
            if (code == null)
            {
                throw new ArgumentNullException("code");
            }
            if (!m_codePattern.Match(code).Success)
            {
                throw new ArgumentException(string.Format("Provided code does not comply with a UnLocode pattern ({0})", m_codePattern), "code");
            }

            m_code = code;
        }

        /// <summary>
        /// Returns a string representation of this UnLocode consisting of 5 characters (all upper):
        /// 2 chars of ISO country code and 3 describing location.
        /// </summary>
        public String CodeString
        {
            get { return m_code; }
        }

        /// <summary>
        /// To be overridden in inheriting clesses for providing a collection of atomic values of
        /// this Value Object.
        /// </summary>
        /// <returns>Collection of atomic values.</returns>
        protected override IEnumerable<Object> GetAtomicValues()
        {
            yield return m_code;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Boolean operator ==(UnLocode left, UnLocode right)
        {
            return EqualOperator(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Boolean operator !=(UnLocode left, UnLocode right)
        {
            return NotEqualOperator(left, right);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override String ToString()
        {
            return m_code;
        }
    }
}
