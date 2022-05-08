using Microsoft.EntityFrameworkCore;

namespace Xiugou.Entities.Entities
{
    public interface IXiugouDbContext
    {
        DbSet<Ticket> Tickets { get; set; }

        DbSet<User> Users { get; set; }

        DbSet<Session> Sessions { get; set; }

        DbSet<H5Profile> H5Profiles { get; set; }

        int SaveChanges();
    }
}
