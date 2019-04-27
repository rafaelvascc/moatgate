using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace MoatGate.Models.AspNetIIdentityCore.EntityFramework
{
    public class MoatGateIdentityDbContext : IdentityDbContext<MoatGateIdentityUser, MoatGateIdentityRole, Guid>
    {
        public MoatGateIdentityDbContext() : base()
        {
        }

        public MoatGateIdentityDbContext(DbContextOptions<MoatGateIdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {            
            base.OnModelCreating(builder);
        }
    }
}
