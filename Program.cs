using System.Linq;
using System;
using Lab2.Models;
using Lab2.Services;
using Lab2.Services.SearchServices;
using System.Collections.Generic;

namespace Lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            var ss = new RealSearchService();
            Console.WriteLine("Enter search keyword:");
            var searchKeyword = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(searchKeyword) || searchKeyword.Length > 25)
            {
                var results = ss.ProcessSearchRequestAsync(new SearchRequest<string>(searchKeyword)).GetAwaiter().GetResult();
                if (results != null && results.Count > 0)
                {
                    Console.WriteLine("Searching results: ");
                    Console.WriteLine(string.Join("\n\n", results));
                    Console.WriteLine(("").PadRight(100, '-'));
                    Console.WriteLine("Would you like to change the expert assessment (default 1) of each search engine(true, false)?");
                    if (bool.TryParse(Console.ReadLine(), out bool changeExpertAssessment))
                    {
                        Console.WriteLine(("").PadRight(100, '-'));
                        var rankings = new List<Ranking<string>>() { };
                        if (changeExpertAssessment)
                        {
                            foreach (var result in results)
                            {
                                Console.Write($"Enter expert assessment of {result.SearchEngineId}: ");
                                if (decimal.TryParse(Console.ReadLine(), out decimal ea))
                                {
                                    Console.WriteLine(("").PadRight(100, '-'));
                                    rankings.Add(new Ranking<string>(result.FoundItems
                                           .Select((item, index) => new RankingElement<string>()
                                           {
                                               Value = item.Data,
                                               Rank = index
                                           })
                                           .ToArray(), expertAssessment: ea));
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Default value was not changed.");
                                }
                            }
                        }
                        else
                        {
                            rankings.AddRange(results.Select(result => (Ranking<string>)result));
                        }
                        var qqRelativeWeightCalculationApproach = new QualityAndQuantityRelativeWeightCalculationApproach<string>();
                        var statisticRelativeWeightCalculationApproach = new StatisticRelativeWeightCalculationApproach<string>();
                        var rankingMethods = new IRankingMethod<string>[]
                        {
                        new BordRankingMethod<string>(qqRelativeWeightCalculationApproach),
                        new BordRankingMethod<string>(statisticRelativeWeightCalculationApproach),
                        new CondorcetRankingMethod<string>(qqRelativeWeightCalculationApproach),
                        new CondorcetRankingMethod<string>(statisticRelativeWeightCalculationApproach),
                        };
                        foreach (var rankingMethod in rankingMethods)
                        {
                            var aggregatedRanking = rankingMethod.GetAggregatedRanking(rankings);
                            DisplayAggregatedRanking(aggregatedRanking, rankingMethod.GetType().Name.Replace("`1", ""), rankingMethod.ApproachName);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                    }
                }
                else
                {
                    Console.WriteLine("Unable to get results");
                }
            }
            else
            {
                Console.WriteLine("Search keyword cannot be empty or longest than 25 char-s");
            }
        }

        private static void DisplayAggregatedRanking(Ranking<string> aggregatedRanking, string rankingMethodName, string approachName)
        {
            Console.WriteLine($"Ranking method: {rankingMethodName}\nRelative Weight Calculation Approach: {approachName}\nAggregated ranking:");
            Console.WriteLine(string.Join("\n", aggregatedRanking.Elements.OrderBy(e => e.Rank).Select(e => $"Value: {e.Value, -80}\tRank: {Math.Round(e.Rank, 4)}")));
            Console.WriteLine(("").PadRight(100, '-'));
        }
    }
}
