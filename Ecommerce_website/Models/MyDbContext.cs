using Ecommerce_website.Migrations;
using Ecommerce_website.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_website.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<order_detail_view> order_detail_view { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // View ke liye
            modelBuilder.Entity<order_detail_view>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("vw_OrderDetails");
            });

            // Table ke liye
            modelBuilder.Entity<order_item>(entity =>
            {
                entity.HasKey(e => e.order_item_id);
                entity.ToTable("order_items");
            });
        }


        public DbSet<admin> admins { get; set; }

        public DbSet<State> states { get; set; }

        public DbSet<catogery> catogerys { get; set; }

        public DbSet<product> products { get; set; }
     
        public DbSet<customer> customers  { get; set; }
        public DbSet<cart> carts { get; set; }

        public DbSet<feedback> feedbacks { get; set; }

        public DbSet<order> orders { get; set; }

        public DbSet<order_item> order_items { get; set; }

        

        public DbSet<Ecommerce_website.Models.feedback> feedback { get; set; } = default!;
    }
    
    
}
