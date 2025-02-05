using EventSourcing.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcing.Shared.Events
{
    public class ProductPriceChangedEvent : IEvent
    {
        public Guid Id { get; set; }
        [Column(TypeName =("decimal(18,2)"))]
        public decimal ChangedPrice { get; set; }
    }
}
