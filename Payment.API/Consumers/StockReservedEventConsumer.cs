﻿using MassTransit;
using Shared;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<StockReservedEventConsumer> _logger;

        public StockReservedEventConsumer(ILogger<StockReservedEventConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            var balance = 3000m;

            if (balance > context.Message.Payment.TotalPrice)
            {

                _logger.LogInformation($"{context.Message.Payment.TotalPrice} " +
                    $"TL was withdrawn from credit card for buyerId:{context.Message.BuyerId}");

                await _publishEndpoint.Publish(new PaymentCompletedEvent
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId
                });
            }
            else
            {
                _logger.LogInformation($"{context.Message.Payment.TotalPrice} " +
                    $"TL was not withdrawn from credit card for buyerId:{context.Message.BuyerId}");

                await _publishEndpoint.Publish(new PaymentFailedEvent
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    Message = "Not enough balance."
                });
            }
        }
    }
}
