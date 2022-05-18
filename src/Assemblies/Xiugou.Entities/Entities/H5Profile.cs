using System;
using System.ComponentModel.DataAnnotations;
using Xiugou.Entities.Enums;

namespace Xiugou.Entities.Entities
{
    public class H5Profile
    {
        [Required]
        public string Id { get; set; }

        public int Role { get; set; }

        public string Items { get; set; }

        public int Status { get; set; }

        public string OpenId { get; set; }
    }
}
