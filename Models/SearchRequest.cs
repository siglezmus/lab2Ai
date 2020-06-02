namespace Lab2.Models
{
    class SearchRequest<TValue> 
    {
        public SearchRequest(TValue searchValue)
        {
            SearchValue = searchValue ?? throw new System.ArgumentNullException(nameof(searchValue));
        }

        public TValue SearchValue { get; set; }
    }
}