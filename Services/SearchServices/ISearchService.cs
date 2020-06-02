using Lab2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

interface ISearchService<TSearchValue,TSearchData>
{
    Task<List<SearchResult<TSearchData>>> ProcessSearchRequestAsync(SearchRequest<TSearchValue> request);
}