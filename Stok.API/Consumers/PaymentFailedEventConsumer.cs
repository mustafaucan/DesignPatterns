using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Stok.API.Models;

namespace Stok.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<PaymentFailedEventConsumer> _logger;

        public PaymentFailedEventConsumer(AppDbContext appDbContext, ILogger<PaymentFailedEventConsumer> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            foreach (var orderItem in context.Message.OrderItems)
            {
                var stock = await _appDbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == orderItem.ProductId);
                if (stock != null)
                {
                    stock.Count += orderItem.Count;
                    await _appDbContext.SaveChangesAsync();
                }

            }
            _logger.LogInformation($"Stock was released for Order Id : {context.Message.OrderId}");
        }
    }
}
