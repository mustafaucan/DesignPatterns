namespace Stok.API.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Count { get; set; }
    }
}
