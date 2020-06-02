using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Lab2.Models
{
    class SearchResult<TData>
    {
        public string SearchEngineId { get; set; }
        public HashSet<FoundItem<TData>> FoundItems { get; set; }

        public SearchResult(IEnumerable<FoundItem<TData>> foundItems, string searchServiceId)
        {
            if (string.IsNullOrEmpty(searchServiceId))
            {
                throw new System.ArgumentException($"{nameof(searchServiceId)} must be provided", nameof(searchServiceId));
            }

            SearchEngineId = searchServiceId;
            FoundItems = foundItems?.ToHashSet()
                                    ?? new HashSet<FoundItem<TData>>();
        }

        public SearchResult() => FoundItems = new HashSet<FoundItem<TData>>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Search Engine: {SearchEngineId}");
            sb.AppendLine();
            sb.Append("Found items:");
            sb.AppendLine();
            sb.AppendJoin(",\n", FoundItems);
            return sb.ToString();
        }
    }
}