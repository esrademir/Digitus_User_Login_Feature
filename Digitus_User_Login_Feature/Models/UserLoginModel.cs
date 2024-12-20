using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Digitus_User_Login_Feature.Models
{
    public partial class UserLoginModel : DbContext
    {
        public UserLoginModel()
            : base("name=UserLoginModel")
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public  virtual DbSet<UserActivationTime> UserActivationTimes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
