using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ChloeShopModel
{
    public partial class ChloeShopEntitiesModel : DbContext
    {
        public ChloeShopEntitiesModel()
            : base("name=ChloeShopEntitiesModel")
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .Property(e => e.PhoneNumber)
                .IsFixedLength();

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Orders)
                .WithOptional(e => e.Customer)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Inventory>()
                .HasMany(e => e.Orders)
                .WithOptional(e => e.Inventory)
                .WillCascadeOnDelete();
        }
    }
}
