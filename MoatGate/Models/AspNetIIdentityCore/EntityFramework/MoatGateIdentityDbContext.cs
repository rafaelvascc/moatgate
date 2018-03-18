using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using IdentityServer4.EntityFramework.Entities;

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
            builder.Entity<Client>().Property<int>(c => c.Id).ValueGeneratedOnAdd();
            builder.Entity<MoatGateIdentityUser>().Property<Guid>(c => c.Id).ValueGeneratedOnAdd();
            builder.Entity<MoatGateIdentityRole>().Property<Guid>(c => c.Id).ValueGeneratedOnAdd();
        }
    }
}