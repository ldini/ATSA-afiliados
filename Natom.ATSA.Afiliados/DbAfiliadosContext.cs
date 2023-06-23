using Natom.ATSA.Afiliados.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Afiliados
{
    public class DbAfiliadosContext : DbContext
    {
        public DbSet<Pais> Paises { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Establecimiento> Establecimientos { get; set; }
        public DbSet<Cupones> Cupones { get; set; }


        public DbAfiliadosContext()
            : base("name=DbAfiliadosContext")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}