using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using kifaro.Models;

namespace kifaro.Models
{
    public class kifaroContext : DbContext
    {
        public kifaroContext (DbContextOptions<kifaroContext> options)
            : base(options)
        {
        }

        public DbSet<Wish> Wish { get; set; }

        public DbSet<User> User { get; set; }
    }
}
