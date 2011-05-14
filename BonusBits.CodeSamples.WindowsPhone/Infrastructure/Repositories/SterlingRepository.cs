using System.Collections.Generic;
using System.Linq;

namespace BonusBits.CodeSamples.WP7.Infrastructure.Repositories
{
    internal class SterlingRepository<T> where T : class, new()
    {
        /// <summary>
        /// Saves the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void Save(T instance)
        {
            App.Database.Save<T>(instance);
        }

        /// <summary>
        /// Loads the by id.
        /// </summary>
        /// <param name="trackingId">The tracking id.</param>
        /// <returns></returns>
        public T LoadById<TKey>(TKey id) where TKey : class
        {
            var query = App.Database.Query<T, TKey>()
                                    .Where((table) => table.Key == id)
                                    .FirstOrDefault();

            return query.LazyValue.Value ??
                default(T);
        }

        /// <summary>
        /// Finds all.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ICollection<T> FindAll<TKey>()
        {
            var items = App.Database.Query<T, TKey>()
                                    .Select((table) => table.LazyValue.Value)
                                    .ToList<T>();
            return items;
        }
    }
}
