using Lab2.Models;
using System.Collections.Generic;

namespace Lab2.Services
{
    interface IRelativeWeightCalculationApproach<TData>
    {
        List<Ranking<TData>> CalculateRelativeWeightForRankings(IEnumerable<Ranking<TData>> rankings);
    }
}
