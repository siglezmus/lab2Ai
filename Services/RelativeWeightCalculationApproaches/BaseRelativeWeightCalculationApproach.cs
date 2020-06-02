using Lab2.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab2.Services
{
    abstract class BaseRelativeWeightCalculationApproach<TData> : IRelativeWeightCalculationApproach<TData>
    {
        public List<Ranking<TData>> CalculateRelativeWeightForRankings(IEnumerable<Ranking<TData>> rankings)
        {
            #region Checks
            if (rankings is null || !rankings.Any())
            {
                throw new ArgumentNullException(nameof(rankings));
            }
            if (rankings.Any(r => r.Elements.Length == default))
            {
                throw new InvalidOperationException("Every ranking must have "
                                                    + $" {nameof(Ranking<TData>.Elements)}.");
            }
            if (rankings.Any(r => r.ExpertAssessment == default))
            {
                throw new InvalidOperationException("Every ranking must have"
                                                    + $" {nameof(Ranking<TData>.ExpertAssessment)}.");
            }
            #endregion
            var resultingRankings = new List<Ranking<TData>>(rankings);
            var allUniqueRankingElementsCount = resultingRankings.SelectMany(r => r.Elements)
                                       .Distinct().Count();
            if (allUniqueRankingElementsCount == default)
            {
                throw new InvalidOperationException("There are no elements in the given rankings");
            }

            var generalCountOfFoundItems = resultingRankings.Sum(r => r.Elements.Length);

            foreach (var ranking in resultingRankings)
            {
                ranking.ObjectiveAssessment = (decimal)ranking.Elements.Length / allUniqueRankingElementsCount;
                ranking.PeerAssessment = (decimal)ranking.Elements.Length / generalCountOfFoundItems;
            }
            var (x1, x2) = GetCheckedRelativeImportanceCoefficients(resultingRankings);
            var unnormalizedWeightsSum = 0M;
            foreach (var ranking in resultingRankings)
            {
                ranking.UnnormalizedWeight = ranking.ExpertAssessment *
                                            (ranking.ObjectiveAssessment * x1 +
                                            ranking.PeerAssessment * x2);
                unnormalizedWeightsSum += ranking.UnnormalizedWeight;
            }
            foreach (var ranking in resultingRankings)
            {
                ranking.NormalizedWeight = ranking.UnnormalizedWeight / unnormalizedWeightsSum;
            }

            return resultingRankings;
        }

        private (decimal x1, decimal x2) GetCheckedRelativeImportanceCoefficients(IEnumerable<Ranking<TData>> rankings)
        {
            var (x1, x2) = GetRelativeImportanceCoefficients(rankings);
            if (x1 + x2 == 1)
            {
                return (x1, x2);
            }
            throw new InvalidOperationException("Relative Importance Coefficients don't meet the rule x1 + x2 = 1");
        }
        protected abstract (decimal x1, decimal x2) GetRelativeImportanceCoefficients(IEnumerable<Ranking<TData>> rankings);
    }
}
