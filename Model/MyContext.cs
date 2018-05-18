using Microsoft.EntityFrameworkCore;

namespace dotnetcore_demo.Model
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {

        }

        public DbSet<UserModel> Users { get; set; }
    }
}