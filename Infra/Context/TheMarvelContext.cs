using Domain.Entitys;
using Infra.EntityConfig;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Context
{


    public class TheMarvelContext : DbContext
    {
        public TheMarvelContext() : base("TheMarvelBD")
        {

        }

        public DbSet<Comic> Comics { get; set; }
        public DbSet<Personagem> Personagens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Properties()
           .Where(p => p.Name == "Id")
               .Configure(p => p.IsKey());
            modelBuilder.Properties()
                .Where(p => p.Name == "RowVersion")
                .Configure(p => p.IsRowVersion());
            modelBuilder.Properties()
                .Where(p => p.Name == "RowVersion")
                .Configure(p => p.IsConcurrencyToken());
            modelBuilder.Properties<string>()
                .Configure(p => p.HasColumnType("varchar"));
            modelBuilder.Properties<string>()
                .Configure(p => p.HasMaxLength(100));
            modelBuilder.Properties<decimal>()
                .Configure(c => c.HasPrecision(18, 2));

            modelBuilder.Configurations.Add(new ComicConfig());
            modelBuilder.Configurations.Add(new PersonagemConfig());

            base.OnModelCreating(modelBuilder);
        }
    }
}
