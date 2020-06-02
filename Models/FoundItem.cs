namespace Lab2.Models
{
    class FoundItem<TData>
    {
        public TData Data { get; set; }
        public FoundItem(TData data) => Data = data;

        public override string ToString()
        {
            return Data.ToString();
        }
    }
}