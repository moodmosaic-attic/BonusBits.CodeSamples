using System;
using BonusBits.CodeSamples.WP7.Domain.Evans.Cargo;
using BonusBits.CodeSamples.WP7.Domain.Evans.Location;

namespace BonusBits.CodeSamples.WP7.Infrastructure.Factories
{
    internal static class CargoFactory
    {
        public static Cargo CreateNew(String from, String to)
        {
            if (String.IsNullOrEmpty(from) || from.Length < 5)
            {
                throw new ArgumentException("from");
            }

            if (String.IsNullOrEmpty(to) || to.Length < 5)
            {
                throw new ArgumentException("to");
            }

            var origin      = new Location(new UnLocode(from.ToUpper().Substring(0,5)), from);
            var destination = new Location(new UnLocode(to.ToUpper().Substring(0, 5)), to);
            var trackingId  = NextTrackingId();
            var route       = new RouteSpecification(origin, destination, DateTime.Now);
            
            return new Cargo(trackingId, route);
        }

        private static TrackingId NextTrackingId()
        {
            Guid uniqueId = Guid.NewGuid();
            return new TrackingId(uniqueId.ToString("N"));
        }
    }
}
