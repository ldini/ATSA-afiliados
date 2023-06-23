using Natom.ATSA.Afiliados.Models;
using Natom.ATSA.Afiliados.Models.Afiliados;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Afiliados
{
    public class DbLocalAfiliadosContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<HistoricoConsultas> HistoricosConsultas { get; set; }

        public DbLocalAfiliadosContext()
            : base("name=DbLocalAfiliadosContext")
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