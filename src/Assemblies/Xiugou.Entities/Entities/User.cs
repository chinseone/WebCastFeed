using System;
using System.Collections.Generic;
using Xiugou.Entities.Enums;

namespace Xiugou.Entities.Entities
{
    public class User
    {
        // 数据库自增Id
        public long Id { get; set; }

        // 直播平台给的uid。不同直播平台的uid可能会串
        public string UserId { get; set; }

        // 平台
        public Platform Platform { get; set; }

        // 昵称
        public string NickName { get; set; }

        // 对应的门票ID，可以为空
        public int? TicketId { get; set; }

        // 发送的弹幕总数
        public int MessageCount { get; set; }

        // 总付费，单位毛
        public int TotalPay { get; set; }

        // 嘉宾礼物的总付费
        public int TotalPayGuest { get; set; }

        // 初次加入的时间戳
        public DateTime JoinTimestamp { get; set; }

        // 最后活跃的时间戳
        public DateTime LastTimestamp { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }
    }
}
