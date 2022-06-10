using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Snips.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            

            builder.Entity<Blog>()
            .HasOne<ApplicationUser>(note => note.ApplicationUser)
            .WithMany(appuser => appuser.Blogs)
            .HasForeignKey(note => note.ApplicationUserId)
            .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<Blog>().HasIndex(n => n.LastModified);
            builder.Entity<Blog>().HasIndex(n => n.Created);
        }
    }
}
