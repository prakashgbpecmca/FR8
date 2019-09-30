using Frayte.Security.IdentityModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Security.IdentityDataAccess
{
    public class FrayteIdentityDbContext : IdentityDbContext<FrayteIdentityUser, FrayteIdentityRole, int, FrayteIdentityUserLogin, FrayteIdentityUserRole, FrayteIdentityUserClaim>
    {

        #region constructors and destructors
        public FrayteIdentityDbContext()
            : base("IdentityConnection")
        {

        }
        #endregion
        #region methods
        public static FrayteIdentityDbContext Create()
        {
            return new FrayteIdentityDbContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map Entities to their tables. 
            modelBuilder.Entity<FrayteIdentityUser>().ToTable("User");
            modelBuilder.Entity<FrayteIdentityRole>().ToTable("Role");
            modelBuilder.Entity<FrayteIdentityUserClaim>().ToTable("UserClaim");
            modelBuilder.Entity<FrayteIdentityUserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<FrayteIdentityUserRole>().ToTable("UserRole");

            // Override some column mappings that do not match our default
            modelBuilder.Entity<FrayteIdentityUser>().Property(r => r.Id).HasColumnName("UserId");
            modelBuilder.Entity<FrayteIdentityRole>().Property(r => r.Id).HasColumnName("RoleId");
            modelBuilder.Entity<FrayteIdentityRole>().Property(r => r.Name).HasColumnName("RoleName");

            // Set AutoIncrement-Properties
            modelBuilder.Entity<FrayteIdentityUser>().Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<FrayteIdentityUserClaim>().Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<FrayteIdentityRole>().Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        #endregion
    }
}
