using System;
using System.Collections.Generic;
using BonusBits.CodeSamples.WP7.Domain.Evans.Cargo;

namespace BonusBits.CodeSamples.WP7.Infrastructure.Repositories
{
    /// <summary>
    /// Cargo repository implementation based on Sterling.
    /// </summary>
    internal sealed class CargoRepository : SterlingRepository<Cargo>, ICargoRepository
    {
        #region ICargoRepository Members

        public void Store(Cargo cargo)
        {
            Save(cargo);
        }

        public Cargo Find(TrackingId trackingId)
        {
            return LoadById(trackingId.IdString);
        }

        public ICollection<Cargo> FindAll()
        {
            return FindAll<String>(); // Cargo.Id is a String.
        }

        #endregion
    }
}
