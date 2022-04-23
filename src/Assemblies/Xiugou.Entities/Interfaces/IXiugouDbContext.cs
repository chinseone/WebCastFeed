using Microsoft.EntityFrameworkCore;

namespace Xiugou.Entities.Entities
{
    public interface IXiugouDbContext
    {
        DbSet<Ticket> Tickets { get; set; }

        DbSet<User> Users { get; set; }

        int SaveChanges();
    }
}
