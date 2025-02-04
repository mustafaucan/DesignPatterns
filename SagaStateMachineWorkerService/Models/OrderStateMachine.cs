using MassTransit;
using Shared;
using Shared.Events;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaStateMachineWorkerService.Models
{
    // Management for distrubuted transaction
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; set; }
        public Event<IStockReservedEvent> StockReservedEvent { get; set; }
        public Event<IStockNotReservedEvent> StockNotReservedEvent { get; set; }

        public Event<IPaymentCompletedEvent> PaymentCompletedEvent { get; set; }
        public Event<IPaymentFailedEvent> PaymentFailedEvent { get; set; }



        public State OrderCreated { get; private set; }
        public State StockReserved { get; private set; }
        public State StockNotReserved { get; private set; }

        public State PaymentCompleted { get; private set; }
        public State PaymentFailed { get; private set; }



        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

            State(() => OrderCreated);

            Event(() => OrderCreatedRequestEvent, evt =>
            {
                evt.CorrelateBy<int>(m => m.OrderId, context => context.Message.OrderId);
                evt.SelectId(context => Guid.NewGuid());
            });

            Event(() => StockReservedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));
            Event(() => StockNotReservedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));
            Event(() => PaymentCompletedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));
            Event(() => PaymentFailedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));

            Initially(
                When(OrderCreatedRequestEvent)
                    .Then(context =>
                    {
                        context.Saga.BuyerId = context.Message.BuyerId;
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.CreatedDate = DateTime.Now;
                        context.Saga.CardName = context.Message.Payment.CardName;
                        context.Saga.CardNumber = context.Message.Payment.CardNumber;
                        context.Saga.CVV = context.Message.Payment.CVV;
                        context.Saga.Expiration = context.Message.Payment.Expiration;
                        context.Saga.TotalPrice = context.Message.Payment.TotalPrice;
                    })
                    .Then(context =>
                    {
                        Console.WriteLine($"OrderCreatedRequestEvent before : {context.Saga}");
                    })
                    .Publish(context => new OrderCreatedEvent(context.Saga.CorrelationId)
                    {
                        OrderItems = context.Message.OrderItems
                    })
                    .TransitionTo(OrderCreated)
                    .Then(context =>
                    {
                        Console.WriteLine($"OrderCreatedRequestEvent after : {context.Saga}");
                    })

            );

            During(OrderCreated,
                When(StockReservedEvent)
                .TransitionTo(StockReserved)
                .Send(new Uri($"queue:{RabbitMqSettingsConst.PaymentStockReservedRequestQueueName}"),
                context => new StockReservedRequestPayment(context.Saga.CorrelationId)
                {
                    OrderItems = context.Message.OrderItems,
                    Payment = new PaymentMessage
                    {
                        CardName = context.Saga.CardName,
                        CardNumber = context.Saga.CardNumber,
                        CVV = context.Saga.CVV,
                        Expiration = context.Saga.Expiration,
                        TotalPrice = context.Saga.TotalPrice
                    },
                    BuyerId = context.Saga.BuyerId
                })
                .Then(context =>
                {
                    Console.WriteLine($"StockReservedEvent after : {context.Saga}");
                }), When(StockNotReservedEvent)
                    .TransitionTo(StockNotReserved)
                    .Publish(context => new OrderRequestFailedEvent()
                    {
                        OrderId = context.Saga.OrderId,
                        Reason = context.Message.Reason
                    })
                     .Then(context =>
                     {
                         Console.WriteLine($"StockNotReservedEvent after : {context.Saga}");
                     })
                 );

            During(StockReserved,
                When(PaymentCompletedEvent)
                    .TransitionTo(PaymentCompleted)
                    .Publish(context => new OrderRequestCompletedEvent() { OrderId = context.Saga.OrderId })
                    .Then(context => { Console.WriteLine($"PaymentCompletedEvent After : {context.Saga}"); })
                    .Finalize()
            When(PaymentFailedEvent)
                .Publish(context => new OrderRequestFailedEvent()
                { OrderId = context.Saga.OrderId, Reason = context.Message.Reason })
                .Send(new Uri($"queue:{RabbitMqSettingsConst.StockRollBackMessageQueueName}"),
                    context => new StockRollbackMessage() { OrderItems = context.Message.OrderItems })
                .TransitionTo(PaymentFailed).Then(context =>
                {
                    Console.WriteLine($"PaymentFailedEvent After : {context.Saga}");
                })
            );
        }
    }
}
