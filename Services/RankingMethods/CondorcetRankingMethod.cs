using System;
using System.Collections.Generic;
using System.Linq;
using Lab2.Models;

namespace Lab2.Services
{
    class CondorcetRankingMethod<TData> : IRankingMethod<TData>
    {
        private readonly IRelativeWeightCalculationApproach<TData> _relativeWeightCalculationApproach;
        public string ApproachName { get; }
        public CondorcetRankingMethod(IRelativeWeightCalculationApproach<TData> relativeWeightCalculationApproach)
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
            var resultRankingElements = rankings.SelectMany(r => r.Elements)
                                                   .Distinct()
                                                   .Select(_ => new RankingElement<TData>()
                                                   {
                                                       Value = _.Value
                                                   })
                                                   .ToArray();
            var preparedRankings = _relativeWeightCalculationApproach.CalculateRelativeWeightForRankings(rankings);

            foreach (var element in resultRankingElements)
            {
                var resultRank = 0m;
                foreach (var rival in resultRankingElements.Except(new[] { element }))
                {
                    var d = 0m;
                    foreach (var ranking in preparedRankings)
                    {
                        var elementRank = ranking.Elements.FirstOrDefault(e => e == element)?.Rank;
                        var rivalRank = ranking.Elements.FirstOrDefault(e => e == rival)?.Rank;
                        if ((!elementRank.HasValue && !rivalRank.HasValue) || (elementRank.HasValue && rivalRank.HasValue &&
                            elementRank.Value == rivalRank.Value))
                        {
                            continue;
                        }
                        else if (elementRank.HasValue && (!rivalRank.HasValue || elementRank.Value < rivalRank.Value))
                        {
                            d += ranking.NormalizedWeight;
                        }
                        else if ((!elementRank.HasValue && rivalRank.HasValue) || (elementRank.HasValue && elementRank.Value > rivalRank.Value))
                        {
                            d -= ranking.NormalizedWeight;
                        }
                    }
                    if (d > 0)
                    {
                        resultRank--;
                    }
                    else if (d < 0)
                    {
                        resultRank++;
                    }
                }
                element.Rank = resultRank;
            }

            var aggregatedRanking = new Ranking<TData>(resultRankingElements);
            return aggregatedRanking;
        }
    }
}


















//foreach (var rival in allUniqueElements)
//{
//    decimal elementResultingRank = 0m;
//    foreach (var elements in allElementsInRankings)
//    {
//        var elIndex = elements.IndexOf(element);
//        var rivIndex = elements.IndexOf(rival);
//        if (elIndex < rivIndex)
//            elementResultingRank++;
//        else if(elIndex > rivIndex)
//            elementResultingRank--;
//    }
//    if (elementResultingRank > 0)
//    {
//        resultRankingElements.First(el => el == element).Rank += 1;
//    } else if (elementResultingRank < 0) {
//        resultRankingElements.First(el => el == rival).Rank += 1;
//    }
//}