using System;
using System.Collections.Generic;
using System.Linq;
using Lab2.Models;

namespace Lab2.Services
{
    class StatisticRelativeWeightCalculationApproach<TData> : BaseRelativeWeightCalculationApproach<TData>
    {
        protected override (decimal x1, decimal x2) GetRelativeImportanceCoefficients(IEnumerable<Ranking<TData>> rankings)
        {
            #region Checks
            if (rankings is null)
            {
                throw new ArgumentNullException(nameof(rankings));
            }
            var peerAssessments = rankings.Select(r => r.PeerAssessment).ToArray();
            if (peerAssessments is null || peerAssessments.Length == default ||peerAssessments.Any(pa => pa == default))
            {
                throw new ArgumentException(nameof(peerAssessments));
            }
            #endregion
            decimal x1, x2;
            var poweredAveragePeerAssessment = (decimal)Math.Pow((double)(peerAssessments.Sum() / peerAssessments.Length), 2);
            var averagePoweredPeerAssessment = ((decimal)peerAssessments.Sum(pa => Math.Pow((double)pa, 2)))
                                               / peerAssessments.Length;
            x1 = averagePoweredPeerAssessment - poweredAveragePeerAssessment;
            x2 = 1 - x1;
            return (x1, x2);
        }
    }
}