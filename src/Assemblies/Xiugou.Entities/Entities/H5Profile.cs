using System;
using System.ComponentModel.DataAnnotations;
using Xiugou.Entities.Enums;

namespace Xiugou.Entities.Entities
{
    public class H5Profile
    {
        [Required]
        public string Id { get; set; } = $"h5profile:{Guid.NewGuid().ToString()}";

        public int Role { get; set; }

        public string Items { get; set; }

        public string Title { get; set; }

        public Platform Platform { get; set; }

        public string Nickname { get; set; }

        public long TicketId { get; set; }
    }
}
