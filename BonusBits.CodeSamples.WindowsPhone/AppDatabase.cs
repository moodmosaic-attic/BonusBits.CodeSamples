using System;
using System.Collections.Generic;
using BonusBits.CodeSamples.WP7.Domain.Evans.Cargo;
using BonusBits.CodeSamples.WP7.Domain.Evans.Location;
using Wintellect.Sterling.Database;

namespace BonusBits.CodeSamples.WP7
{
    internal sealed class AppDatabase : BaseDatabaseInstance
    {
        public override String Name
        {
            get { return this.GetType().Name; }
        }

        protected override List<ITableDefinition> _RegisterTables()
        {
            return new List<ITableDefinition>
            {
                CreateTableDefinition<Cargo, String>(x=>x.TrackingId.IdString),
                CreateTableDefinition<Location, String>(x=>x.Name),
                CreateTableDefinition<RouteSpecification,  DateTime>(x=>x.ArrivalDeadline),
                CreateTableDefinition<TrackingId, String>(x=>x.IdString),
                CreateTableDefinition<UnLocode, String>(x=>x.CodeString)
            };
        }
    }
}
