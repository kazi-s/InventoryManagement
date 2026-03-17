using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<InventoryAccess> InventoryAccesses { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ItemLike> ItemLikes { get; set; }
        public DbSet<InventoryTag> InventoryTags { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.HasDefaultSchema("public");
            
            builder.Entity<InventoryTag>()
                .HasKey(it => new { it.InventoryId, it.TagId });
            
            builder.Entity<Item>()
                .HasIndex(i => new { i.InventoryId, i.CustomId })
                .IsUnique();
            
            builder.Entity<Inventory>()
                .HasOne(i => i.Creator)
                .WithMany(u => u.OwnedInventories)
                .HasForeignKey(i => i.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Item>()
                .HasOne(i => i.CreatedBy)
                .WithMany()
                .HasForeignKey(i => i.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Equipment" },
                new Category { Id = 2, Name = "Furniture" },
                new Category { Id = 3, Name = "Book" },
                new Category { Id = 4, Name = "Other" }
            );

            builder.Entity<Item>(entity =>
            {
                entity.Property(e => e.NumberValue1)
                    .HasColumnType("numeric(18,2)");
                entity.Property(e => e.NumberValue2)
                    .HasColumnType("numeric(18,2)");
                entity.Property(e => e.NumberValue3)
                    .HasColumnType("numeric(18,2)");
            });

            builder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType("text");
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType("text");
                entity.Property(e => e.Name)
                    .HasColumnType("varchar(256)");
                entity.Property(e => e.NormalizedName)
                    .HasColumnType("varchar(256)");
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.Property(e => e.UserId)
                    .HasColumnType("text");
                entity.Property(e => e.RoleId)
                    .HasColumnType("text");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.Property(e => e.UserId)
                    .HasColumnType("text");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(e => e.LoginProvider)
                    .HasColumnType("text");
                entity.Property(e => e.ProviderKey)
                    .HasColumnType("text");
                entity.Property(e => e.UserId)
                    .HasColumnType("text");
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(e => e.UserId)
                    .HasColumnType("text");
                entity.Property(e => e.LoginProvider)
                    .HasColumnType("text");
                entity.Property(e => e.Name)
                    .HasColumnType("text");
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.Property(e => e.RoleId)
                    .HasColumnType("text");
            });
        }
    }
}