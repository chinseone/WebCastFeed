using System;
using Xiugou.Entities.Enums;

namespace Xiugou.Entities.Entities
{
    public class Ticket
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public Platform? Platform { get; set; }
        public Event? Event { get; set; }
        public TicketType TicketType { get; set; }
        public bool IsDistributed { get; set; }
        public bool IsClaimed { get; set; }
        public bool IsActivated { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
    }
}
