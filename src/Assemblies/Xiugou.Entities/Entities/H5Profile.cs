using System;
using Xiugou.Entities.Enums;

namespace Xiugou.Entities.Entities
{
    public class H5Profile
    {
        public long Id { get; set; }

        public string Role { get; set; }

        public string Items { get; set; }

        public string Title { get; set; }

        public Platform Platform { get; set; }

        public string Nickname { get; set; }

        public long TicketId { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }
    }
}
