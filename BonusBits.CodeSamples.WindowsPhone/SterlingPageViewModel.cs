using System.Diagnostics;
using BonusBits.CodeSamples.WP7.Domain.Evans.Cargo;
using BonusBits.CodeSamples.WP7.Infrastructure.Factories;
using Caliburn.Micro;
using System;

namespace BonusBits.CodeSamples.WP7
{
    public sealed class SterlingPageViewModel : PropertyChangedBase
    {
        private readonly ICargoRepository m_repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SterlingPageViewModel"/> class.
        /// </summary>
        /// <param name="navigator">The navigator.</param>
        public SterlingPageViewModel(ICargoRepository repository)
        {
            // ICargoRepository is available to us using Constructor Injection.
            m_repository = repository;
        }

        public void StoreAndLoad()
        {
            Cargo cargo = CargoFactory.CreateNew("Glyfada", "Perachora");
            m_repository.Store(cargo);

            Cargo saved = m_repository.Find(cargo.TrackingId);

            Debug.Assert(cargo.RouteSpecification.Equals(saved.RouteSpecification));
            Debug.Assert(cargo.Delivery.Equals(saved.Delivery));
            Debug.Assert(cargo.Equals(saved));
        }

        public String TestSyncElapsed { get; set; }

        public void TestSync()
        {
            TestSyncElapsed = "Nikos";
            NotifyOfPropertyChange(() => TestSyncElapsed);
        }
    }
}
