using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.DTOs;
using Order.API.Models;
using Shared;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderController(AppDbContext appDbContext, IPublishEndpoint publishEndpoint)
        {
            _appDbContext = appDbContext;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDto orderCreateDto)
        {
            var newOrder = new Models.Order
            {
                BuyerId = orderCreateDto.BuyerId,
                Status = OrderStatus.Suspend,
                Address = new Address { Line = orderCreateDto.Adress.Line,
                Province = orderCreateDto.Adress.Province,
                District = orderCreateDto.Adress.District
                },
                CreatedTime = DateTime.Now
            };

            orderCreateDto.OrderItems.ForEach(_ =>
            {
                newOrder.Items.Add(new OrderItem()
                {
                    Price = _.Price,
                    ProductId = _.ProductId,
                    Count = _.Count
                });
            });


            await _appDbContext.AddAsync(newOrder);
            await _appDbContext.SaveChangesAsync();

            var orderCreatedEvent = new OrderCreatedEvent()
            {
                BuyerId= orderCreateDto.BuyerId,
                OrderId = newOrder.Id,
                Payment = new PaymentMessage {
                    CardName = orderCreateDto.Payment.CardName,
                    CardNumber = orderCreateDto.Payment.CardNumber,
                    Expiration = orderCreateDto.Payment.Expiration,
                    CVV = orderCreateDto.Payment.CVV,
                    TotalPrice = orderCreateDto.OrderItems.Sum(item => item.Price * item.Count)                
                }                                
            };

            orderCreateDto.OrderItems.ForEach(_ => {
                orderCreatedEvent.OrderItems.Add(new OrderItemMessage
                {
                    Count = _.Count,
                    ProductId = _.ProductId
                });
            });
            
            await _publishEndpoint.Publish(orderCreatedEvent);

            return Ok();
        }
    }
}
