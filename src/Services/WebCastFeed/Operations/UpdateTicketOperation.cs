using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Enums;
using WebCastFeed.Models.Requests;
using Xiugou.Entities.Entities;
using Xiugou.Entities.Enums;

namespace WebCastFeed.Operations
{
    public class UpdateTicketOperation : IAsyncOperation<UpdateTicketRequest, bool>
    {
        private readonly IXiugouRepository _XiugouRepository;
        private readonly Dictionary<TicketState, TicketState> _TicketStateTransferMap 
            = new Dictionary<TicketState, TicketState>
        {
            {TicketState.Initial, TicketState.Distributed},
            {TicketState.Distributed, TicketState.Activated},
            {TicketState.Activated, TicketState.Activated},
        };

        public UpdateTicketOperation(IXiugouRepository xiugouRepository)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
        }

        public async ValueTask<bool> ExecuteAsync(UpdateTicketRequest input, CancellationToken cancellationToken = default)
        {
            var targetTicket = await _XiugouRepository.GetTicketByCode(input.TicketCode);
            if (!IsValidTicket(targetTicket))
            {
                return false;
            }

            var fromState = GetTicketCurrentState(targetTicket);
            var toTicket = new Ticket()
            {
                Id = targetTicket.Id,
                Code = targetTicket.Code,
                IsDistributed = targetTicket.IsDistributed || input.IsDistributed,
                IsClaimed = targetTicket.IsClaimed || input.IsClaimed,
                IsActivated = targetTicket.IsActivated || input.IsActivated,
                CreatedUtc = targetTicket.CreatedUtc
            };
            var toState = GetTicketCurrentState(toTicket);

            if (_TicketStateTransferMap[fromState] != toState)
            {
                return false;
            }

            await _XiugouRepository.UpdateTicket(toTicket);

            var user = await _XiugouRepository.GetUserByUserIdAndPlatform(input.UserId, (Platform)input.Platform);
            if (user == null)
            {
                // First create a user no matter what
                var now = DateTime.UtcNow;
                user = new User()
                {
                    UserId = input.UserId,
                    Platform = (Platform)input.Platform,
                    NickName = input.Nickname,
                    MessageCount = 0,
                    TotalPay = 0,
                    TotalPayGuest = 0,
                    JoinTimestamp = now,
                    LastTimestamp = now,
                    CreatedUtc = now,
                    UpdatedUtc = now,
                };
            }

            // ticket code comes in with danmu
            if (fromState != TicketState.Activated)
            {
                user.TicketId = (int)targetTicket.Id;
            }

            _XiugouRepository.Save(user);

            if (user.TicketId.HasValue &&
                user.TicketId == (int)targetTicket.Id)
            {
                return true;
            }

            return false;
        }

        private static bool IsValidTicket(Ticket ticket)
        {
            if(ticket == null)
            {
                return false;
            }

            // distributed -> activated
            // valid states:
            //     false   ->  false
            //     true    ->  false
            //     true    ->  true
            return (!ticket.IsDistributed && !ticket.IsActivated)
                || (ticket.IsDistributed && !ticket.IsActivated)
                || (ticket.IsDistributed && ticket.IsActivated);
        }

        private static TicketState GetTicketCurrentState(Ticket ticket)
        {
            if (ticket.IsDistributed && !ticket.IsActivated)
            {
                return TicketState.Distributed;
            }

            if (ticket.IsDistributed && ticket.IsActivated)
            {
                return TicketState.Activated;
            }

            return TicketState.Initial;
        }
    }
}
