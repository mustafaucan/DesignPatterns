using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class PaymentFailedEvent : IPaymentFailedEvent
    {
        public List<OrderItemMessage> OrderItems { get; set; }
        public string Reason { get; set; }

        public Guid CorrelationId { get; }

        public PaymentFailedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}
