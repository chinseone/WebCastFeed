using WebCastFeed.Models.Requests;
using Xiugou.Entities.Entities;
using Xiugou.Entities.Enums;

namespace XiugouChatHub.Operations;

public class StoreChatInformationOperation : IAsyncOperation<MessageBody, bool>
{
    private readonly IXiugouRepository _XiugouRepository;

    public StoreChatInformationOperation(IXiugouRepository xiugouRepository)
    {
        _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
    }

    public async ValueTask<bool> ExecuteAsync(MessageBody chatMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _XiugouRepository.GetUserByUserIdAndPlatform(chatMessage.UserId, (Platform)chatMessage.Platform);

            if (user == null)
            {
                await CreateNewUser(chatMessage);
                return true;
            }

            await UpdateUserInformation(user, chatMessage);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private async Task CreateNewUser(MessageBody chatMessage)
    {
        var now = DateTime.UtcNow;
        var user = new User()
        {
            Id = new Guid(),
            Platform = (Platform)chatMessage.Platform,
            UserId = chatMessage.UserId,
            MessageCount = 1,
            NickName = chatMessage.Nickname,
            TicketCode = chatMessage.TicketCode,
            TotalPay = chatMessage.Pay,
            TotalPayGuest = chatMessage.PayGuest,
            Items = chatMessage.Items,
            CreatedUtc = now,
            JoinTimestamp = now,
            LastTimestamp = now
        };

        await _XiugouRepository.Save(user);
    }

    private async Task UpdateUserInformation(User user, MessageBody chatMessage)
    {
        var now = DateTime.UtcNow;
        var allItems = user.Items ?? new List<int>();
        allItems.AddRange(chatMessage.Items);
        var userToUpdate = new User()
        {
            Id = user.Id,
            Platform = (Platform)chatMessage.Platform,
            UserId = chatMessage.UserId,
            MessageCount = user.MessageCount + 1,
            NickName = chatMessage.Nickname,
            TicketCode = chatMessage.TicketCode,
            TotalPay = user.TotalPay + chatMessage.Pay,
            TotalPayGuest = user.TotalPayGuest + chatMessage.PayGuest,
            Items = allItems,
            CreatedUtc = user.CreatedUtc,
            JoinTimestamp = user.JoinTimestamp,
            LastTimestamp = now
        };

        await _XiugouRepository.Save(userToUpdate);
    }
}
