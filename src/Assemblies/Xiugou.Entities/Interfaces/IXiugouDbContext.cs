using Microsoft.EntityFrameworkCore;

namespace Xiugou.Entities.Entities
{
    public interface IXiugouDbContext
    {
        DbSet<Ticket> Tickets { get; set; }

        int SaveChanges();
    }
}
