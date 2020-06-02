using System.Collections.Generic;
using System.Linq;
using Lab2.Models;

namespace Lab2.Services
{
    class QualityAndQuantityRelativeWeightCalculationApproach<TData> : BaseRelativeWeightCalculationApproach<TData>
    {
        protected override (decimal x1, decimal x2) GetRelativeImportanceCoefficients(IEnumerable<Ranking<TData>> rankings)
        {
            if (rankings is null)
            {
                throw new System.ArgumentNullException(nameof(rankings));
            }

            var allUniqueRankingElements = rankings.SelectMany(r => r.Elements)
                                                   .Distinct().ToArray();
            var sumOfCountOfRankingsByAlternative = allUniqueRankingElements
                                                   .Sum(element => rankings.Count(r => r.Elements.Contains(element)));
            var resultsIntersectionDensity = sumOfCountOfRankingsByAlternative/(rankings.Count()*allUniqueRankingElements.Length);

            return (1 - resultsIntersectionDensity, resultsIntersectionDensity);
        }
    }
}