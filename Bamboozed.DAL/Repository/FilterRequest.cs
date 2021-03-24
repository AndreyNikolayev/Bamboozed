namespace Bamboozed.DAL.Repository
{
    public class FilterRequest
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public FilterRequest(string partitionKey = null, string rowKey = null)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
    }
}
