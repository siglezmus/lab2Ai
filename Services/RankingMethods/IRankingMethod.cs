using System.Collections.Generic;
using Lab2.Models;

namespace Lab2.Services
{
    interface IRankingMethod<TData>
    {
        /// <summary>
        /// Назва поточного підходу визначення відносної вагомості джерел інформації
        /// </summary>
        string ApproachName { get; }
        /// <summary>
        /// Метод ранжування
        /// </summary>
        /// <param name="rankings">колекція ранжувань (результатів пошуку) від пошукових систем</param>
        /// <returns>агреговане ранжування</returns>
        Ranking<TData> GetAggregatedRanking(IEnumerable<Ranking<TData>> rankings);
    }
}