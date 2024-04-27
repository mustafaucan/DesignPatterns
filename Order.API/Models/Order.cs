namespace Order.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public string BuyerId { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public OrderStatus Status { get; set; }
        public string FailMessage { get; set; } = string.Empty;
        public Address Address { get; set; }

    }

    public enum OrderStatus
    {
        Suspend,
        Complete,
        Fail
    }
}
