using System;
using System.Collections.Generic;
using System.Text;
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
        private readonly Dictionary<TicketState, TicketState> TicketStateTransferMap = new Dictionary<TicketState, TicketState>
        {
            {TicketState.Initial, TicketState.Distributed},
            {TicketState.Distributed, TicketState.Claimed},
            {TicketState.Claimed, TicketState.Activated}
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

            if(TicketStateTransferMap[fromState] != toState)
            {
                return false;
            }

            await _XiugouRepository.UpdateTicketState(toTicket);
            
            return true;
        }

        private bool IsValidTicket(Ticket ticket)
        {
            if(ticket == null)
            {
                return false;
            }

            // distributed -> claimed -> activated
            // valid states:
            //     false   ->  false  -> false
            //     true    ->  false  -> false
            //     true    ->  true   -> false
            //     true    ->  true   -> true
            return (!ticket.IsDistributed && !ticket.IsClaimed && !ticket.IsActivated)
                || (ticket.IsDistributed && !ticket.IsClaimed && !ticket.IsActivated)
                || (ticket.IsDistributed && ticket.IsClaimed && !ticket.IsActivated)
                || (ticket.IsDistributed && ticket.IsClaimed && ticket.IsActivated);
        }

        private TicketState GetTicketCurrentState(Ticket ticket)
        {
            if (ticket.IsDistributed && !ticket.IsClaimed && !ticket.IsActivated)
            {
                return TicketState.Distributed;
            }

            if (ticket.IsDistributed && ticket.IsClaimed && !ticket.IsActivated)
            {
                return TicketState.Claimed;
            }

            if (ticket.IsDistributed && ticket.IsClaimed && ticket.IsActivated)
            {
                return TicketState.Activated;
            }

            return TicketState.Initial;
        }
    }
}
