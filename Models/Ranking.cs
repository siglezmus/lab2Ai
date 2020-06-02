using System;
using System.Linq;
namespace Lab2.Models
{
    class Ranking<T>
    {
        private RankingElement<T>[] _elements;
        public RankingElement<T>[] Elements
        {
            get => _elements; 
            set
            {
                if (value.Select(_ => _.Rank).Distinct().Count() != value.Length)
                {
                    throw new ArgumentException($"{nameof(RankingElement<T>.Rank)}s of {Elements} must be unique");
                }
                _elements = value;
            }
        }
        /// <summary>
        /// 'O' in formula
        /// </summary>
        public decimal ObjectiveAssessment { get; set; }
        /// <summary>
        /// 'V' in formula
        /// </summary>
        public decimal PeerAssessment { get; set; }
        /// <summary>
        /// 'E' in formula
        /// </summary>
        public decimal ExpertAssessment { get; set; }
        /// <summary>
        /// 'W*' in formula
        /// </summary>
        public decimal UnnormalizedWeight { get; set; }
        /// <summary>
        /// 'W' in formula
        /// </summary>
        public decimal NormalizedWeight { get; set; }

        public Ranking() => _elements = new RankingElement<T>[] { };

        public Ranking(RankingElement<T>[] elements,
                       decimal objectiveAssessment = default,
                       decimal peerAssessment = default,
                       decimal expertAssessment = 1m,
                       decimal unnormalizedWeight = default,
                       decimal normalizedWeight = default)
        {
            _elements = elements ?? throw new ArgumentNullException(nameof(elements));
            ObjectiveAssessment = objectiveAssessment;
            PeerAssessment = peerAssessment;
            ExpertAssessment = expertAssessment;
            UnnormalizedWeight = unnormalizedWeight;
            NormalizedWeight = normalizedWeight;
        }

        //public static implicit operator SearchResult<T>(Ranking<T> ranking) =>
        //    new SearchResult<T>(ranking.Elements
        //                               .Select(e => new FoundItem<T>(e.Value)));
        public static explicit operator Ranking<T>(SearchResult<T> searchResult) =>
            new Ranking<T>(searchResult.FoundItems
                                       .Select((item, index) => new RankingElement<T>()
                                       {
                                           Value = item.Data,
                                           Rank = index
                                       })
                                       .ToArray());
    }
}