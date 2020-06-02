using System;
using System.Linq;
using System.Collections.Generic;
using Lab2.Models;

namespace Lab2.Services
{
    class BordRankingMethod<TData> : IRankingMethod<TData>
    {
        private readonly IRelativeWeightCalculationApproach<TData> _relativeWeightCalculationApproach;
        public string ApproachName { get;  }

        public BordRankingMethod(IRelativeWeightCalculationApproach<TData> relativeWeightCalculationApproach)
        {
            ApproachName = relativeWeightCalculationApproach.GetType().Name.Replace("`1", "");
            _relativeWeightCalculationApproach = relativeWeightCalculationApproach ??
                throw new ArgumentNullException(nameof(relativeWeightCalculationApproach));
        }
        
        public Ranking<TData> GetAggregatedRanking(IEnumerable<Ranking<TData>> rankings)
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

            var allUniqueRankingElements = rankings.SelectMany(r => r.Elements)
                                                   .Distinct()
                                                   .Select(_ => new RankingElement<TData>()
                                                   {
                                                       Value = _.Value
                                                   })
                                                   .ToArray();
            if (allUniqueRankingElements.Length == default)
            {
                throw new InvalidOperationException("There are no elements in the given rankings");
            }

            var preparedRankings = _relativeWeightCalculationApproach.CalculateRelativeWeightForRankings(rankings);

            foreach (var element in allUniqueRankingElements)
            {
                foreach (var r in preparedRankings)
                {
                    var rank = r.Elements.FirstOrDefault(e => e.Equals(element))?.Rank ?? r.Elements.Length;
                    element.Rank += rank * r.NormalizedWeight;
                }
            }
            var aggregatedRanking = new Ranking<TData>(allUniqueRankingElements);
            return aggregatedRanking;
        }
    }
}