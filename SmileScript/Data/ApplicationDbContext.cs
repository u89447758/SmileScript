using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmileScript.Models;

namespace SmileScript.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the relationship between BlogPost and Comment to Cascade on delete.
            // This is usually the default, but we're being explicit.
            builder.Entity<Comment>()
                .HasOne(p => p.BlogPost)
                .WithMany(b => b.Comments)
                .HasForeignKey(p => p.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between User(Author) and Comment to Restrict on delete.
            // This is the key change to prevent the multiple cascade paths issue.
            builder.Entity<Comment>()
                .HasOne(p => p.Author)
                .WithMany() // A user can have many comments, but we don't need a navigation property on the IdentityUser class.
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
