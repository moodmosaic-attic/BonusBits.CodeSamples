using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BonusBits.CodeSamples.WP7.Domain.Evans.Location
{
    /// <summary>
    /// Provides access to location store.
    /// http://dddsample.sourceforge.net/
    /// </summary>
    public interface ILocationRepository
    {
        /// <summary>
        /// Finds a location using given <see cref="UnLocode"/>.
        /// </summary>
        /// <param name="locode"></param>
        /// <returns>A location identified by a given <see cref="UnLocode"/></returns>
        Location Find(UnLocode locode);
        /// <summary>
        /// Finds all locations.
        /// </summary>
        /// <returns>A collection of all defined locations.</returns>
        IList<Location> FindAll();
    }
}
