using EventSourcing.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcing.Shared.Events
{
    public class ProductCreatedEvent : IEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int UserId { get; set; }
    }
}
