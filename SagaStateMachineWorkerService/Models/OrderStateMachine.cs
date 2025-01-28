using MassTransit;
using Shared;
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
        public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; private set; }

        public State OrderCreated { get; private set; }

        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

            State(() => OrderCreated);

            Event(() => OrderCreatedRequestEvent, evt =>
            {
                evt.CorrelateBy<int>(m => m.OrderId, context => context.Message.OrderId);
                evt.SelectId(context => Guid.NewGuid());
            });

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
        }
    }

}
