using System;

namespace Xiugou.Entities.Entities
{
    public class Session
    {
        public long Id { get; set; }

        public string AnchorId { get; set; }
        
        public string SessionId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }
    }
}
