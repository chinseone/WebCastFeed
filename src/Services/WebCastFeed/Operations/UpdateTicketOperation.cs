﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Enums;
using WebCastFeed.Models.Requests;
using WebCastFeed.Models.Response;
using Xiugou.Entities.Entities;
using Xiugou.Entities.Enums;

namespace WebCastFeed.Operations
{
    public class UpdateTicketOperation : IAsyncOperation<UpdateTicketRequest, UpdateTicketResponse>
    {
        private readonly IXiugouRepository _XiugouRepository;
        private readonly Dictionary<TicketState, TicketState> _TicketStateTransferMap 
            = new Dictionary<TicketState, TicketState>
        {
            {TicketState.Initial, TicketState.Activated},
            {TicketState.Distributed, TicketState.Activated},
            {TicketState.Activated, TicketState.Activated},
        };

        public UpdateTicketOperation(IXiugouRepository xiugouRepository)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
        }

        public async ValueTask<UpdateTicketResponse> ExecuteAsync(UpdateTicketRequest input, CancellationToken cancellationToken = default)
        {
            var targetTicket = await _XiugouRepository.GetTicketByCode(input.TicketCode);
            if (!IsValidTicket(targetTicket))
            {
                return new UpdateTicketResponse()
                {
                    Success = false,
                    Platform = input.Platform,
                    TicketCode = "",
                    UserId = input.UserId,
                    Nickname = input.Nickname
                };
            }
            
            var fromState = GetTicketCurrentState(targetTicket);
            var currentUser = await _XiugouRepository.GetUserByUserIdAndPlatform(input.UserId, (Platform)input.Platform);

            if (fromState == TicketState.Activated)
            {
                if (currentUser != null)
                {
                    if (currentUser.UserId.Equals(targetTicket.OwnerId))
                    {
                        return new UpdateTicketResponse()
                        {
                            Success = true,
                            Platform = input.Platform,
                            TicketCode = targetTicket.Code,
                            UserId = input.UserId,
                            Nickname = input.Nickname
                        };
                    }
                    else
                    {
                        return new UpdateTicketResponse()
                        {
                            Success = false,
                            Platform = input.Platform,
                            TicketCode = "",
                            UserId = input.UserId,
                            Nickname = input.Nickname
                        };
                    }
                }
                else
                {
                    var now = DateTime.UtcNow;
                    currentUser = new User()
                    {
                        UserId = input.UserId,
                        Platform = (Platform)input.Platform,
                        NickName = input.Nickname,
                        TicketCode = string.Empty,
                        MessageCount = 0,
                        TotalPay = 0,
                        TotalPayGuest = 0,
                        JoinTimestamp = now,
                        LastTimestamp = now,
                        CreatedUtc = now
                    };

                    await _XiugouRepository.Save(currentUser);

                    return new UpdateTicketResponse()
                    {
                        Success = false,
                        Platform = input.Platform,
                        TicketCode = "",
                        UserId = input.UserId,
                        Nickname = input.Nickname
                    };
                }
            }
            else
            {
                if (currentUser == null)
                {
                    var now = DateTime.UtcNow;
                    currentUser = new User()
                    {
                        UserId = input.UserId,
                        Platform = (Platform)input.Platform,
                        NickName = input.Nickname,
                        TicketCode = targetTicket.Code,
                        MessageCount = 0,
                        TotalPay = 0,
                        TotalPayGuest = 0,
                        JoinTimestamp = now,
                        LastTimestamp = now,
                        CreatedUtc = now
                    };

                    await _XiugouRepository.Save(currentUser);

                    targetTicket.OwnerId = currentUser.UserId;

                    var toTicket = new Ticket()
                    {
                        Id = targetTicket.Id,
                        Code = targetTicket.Code,
                        Platform = (Platform)input.Platform,
                        Event = (Event)input.Event,
                        IsDistributed = targetTicket.IsDistributed || input.IsDistributed,
                        IsClaimed = targetTicket.IsClaimed || input.IsClaimed,
                        IsActivated = targetTicket.IsActivated || input.IsActivated,
                        CreatedUtc = targetTicket.CreatedUtc,
                        OwnerId = currentUser.UserId,
                        UpdatedUtc = now
                    };

                    await _XiugouRepository.UpdateTicket(toTicket);

                    return new UpdateTicketResponse()
                    {
                        Success = true,
                        Platform = input.Platform,
                        TicketCode = targetTicket.Code,
                        UserId = input.UserId,
                        Nickname = input.Nickname
                    };
                }
                else
                {
                    if (targetTicket.OwnerId.Equals(currentUser.UserId))
                    {
                        var now  = DateTime.UtcNow;
                        var toTicket = new Ticket()
                        {
                            Id = targetTicket.Id,
                            Code = targetTicket.Code,
                            Platform = (Platform)input.Platform,
                            Event = (Event)input.Event,
                            IsDistributed = targetTicket.IsDistributed || input.IsDistributed,
                            IsClaimed = targetTicket.IsClaimed || input.IsClaimed,
                            IsActivated = true,
                            CreatedUtc = targetTicket.CreatedUtc,
                            OwnerId = currentUser.UserId,
                            UpdatedUtc = now
                        };

                        await _XiugouRepository.UpdateTicket(toTicket);

                        return new UpdateTicketResponse()
                        {
                            Success = true,
                            Platform = input.Platform,
                            TicketCode = targetTicket.Code,
                            UserId = input.UserId,
                            Nickname = input.Nickname
                        };
                    }

                    return new UpdateTicketResponse()
                    {
                        Success = false,
                        Platform = input.Platform,
                        TicketCode = "",
                        UserId = input.UserId,
                        Nickname = input.Nickname
                    };
                }
            }



                /////

            // if (fromState == TicketState.Activated)
            // {
            //     // Check if the ticket belongs to same user
            //     var currentUser = await _XiugouRepository.GetUserByUserIdAndPlatform(input.UserId, (Platform)input.Platform);
            //     if (currentUser == null || currentUser.TicketCode == null || !currentUser.TicketCode.Equals(input.TicketCode))
            //     {
            //         return new UpdateTicketResponse()
            //         {
            //             Success = false,
            //             Platform = input.Platform,
            //             TicketCode = "",
            //             UserId = input.UserId,
            //             Nickname = input.Nickname
            //         };
            //     }
            // }
            //
            // var toTicket = new Ticket()
            // {
            //     Id = targetTicket.Id,
            //     Code = targetTicket.Code,
            //     Platform = (Platform)input.Platform,
            //     Event = (Event)input.Event,
            //     IsDistributed = targetTicket.IsDistributed || input.IsDistributed,
            //     IsClaimed = targetTicket.IsClaimed || input.IsClaimed,
            //     IsActivated = targetTicket.IsActivated || input.IsActivated,
            //     CreatedUtc = targetTicket.CreatedUtc
            // };
            // var toState = GetTicketCurrentState(toTicket);
            //
            // if (_TicketStateTransferMap[fromState] != toState)
            // {
            //     return new UpdateTicketResponse()
            //     {
            //         Success = false,
            //         Platform = input.Platform,
            //         TicketCode = "",
            //         UserId = input.UserId,
            //         Nickname = input.Nickname
            //     };
            // }
            //
            // await _XiugouRepository.UpdateTicket(toTicket);
            //
            // var user = await _XiugouRepository.GetUserByUserIdAndPlatform(input.UserId, (Platform)input.Platform);
            // if (user == null)
            // {
            //     // First create a user no matter what
            //     var now = DateTime.UtcNow;
            //     user = new User()
            //     {
            //         UserId = input.UserId,
            //         Platform = (Platform)input.Platform,
            //         NickName = input.Nickname,
            //         TicketCode = targetTicket.Code,
            //         MessageCount = 0,
            //         TotalPay = 0,
            //         TotalPayGuest = 0,
            //         JoinTimestamp = now,
            //         LastTimestamp = now,
            //         CreatedUtc = now
            //     };
            // }
            //
            // // ticket code comes in with danmu
            // if (fromState != TicketState.Activated)
            // {
            //     user.TicketCode = targetTicket.Code;
            // }
            //
            // await _XiugouRepository.Save(user);
            //
            // if (!string.IsNullOrEmpty(user.TicketCode) &&
            //     user.TicketCode.Equals(targetTicket.Code))
            // {
            //     return new UpdateTicketResponse()
            //     {
            //         Success = true,
            //         Platform = input.Platform,
            //         TicketCode = targetTicket.Code,
            //         UserId = input.UserId,
            //         Nickname = input.Nickname
            //     };
            // }
            //
            // return new UpdateTicketResponse()
            // {
            //     Success = false,
            //     Platform = input.Platform,
            //     TicketCode = "",
            //     UserId = input.UserId,
            //     Nickname = input.Nickname
            // };
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
            //     false    ->  true
            return (!ticket.IsDistributed && !ticket.IsActivated)
                || (ticket.IsDistributed && !ticket.IsActivated)
                || (ticket.IsDistributed && ticket.IsActivated)
                || (!ticket.IsDistributed && ticket.IsActivated);
        }

        private static TicketState GetTicketCurrentState(Ticket ticket)
        {
            if (ticket.IsDistributed && !ticket.IsActivated)
            {
                return TicketState.Distributed;
            }

            if (ticket.IsActivated)
            {
                return TicketState.Activated;
            }

            return TicketState.Initial;
        }
    }
}
