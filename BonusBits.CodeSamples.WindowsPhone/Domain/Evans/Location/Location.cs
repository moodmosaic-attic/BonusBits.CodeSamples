using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BonusBits.CodeSamples.WP7.Domain.Evans.Location
{
    /// <summary>
    /// A location is our model is stops on a journey, such as cargo
    /// origin or destination, or carrier movement endpoints.
    /// 
    /// It is uniquely identified by a UN Locode.
    /// http://dddsample.sourceforge.net/
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> class.
        /// </summary>
        public Location() { }

        /// <summary>
        /// Gets the <see cref="UnLocode"/> for this location.
        /// </summary>
        public UnLocode UnLocode { get; protected set; }

        /// <summary>
        /// Gets the name of this location, e.g. Krakow.
        /// </summary>
        public String Name { get; protected set; }

        /// <summary>
        /// Returns an instance indicating an unknown location.
        /// </summary>
        public static Location Unknown
        {
            get { return new Location(new UnLocode("XXXXX"), "Unknown location"); }
        }

        /// <summary>
        /// Creates new location.
        /// </summary>
        /// <param name="locode"><see cref="UnLocode"/> for this location.</param>
        /// <param name="name">Name.</param>
        public Location(UnLocode locode, String name)
        {
            UnLocode = locode;
            Name = name;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override String ToString()
        {
            return Name + " [" + UnLocode + "]";
        }
    }
}
