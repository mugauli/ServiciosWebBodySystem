﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ServiciosWebBodySystem.Datos
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BodySystemDBEntities : DbContext
    {
        public BodySystemDBEntities()
            : base("name=BodySystemDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Ciudad> Ciudad { get; set; }
        public virtual DbSet<ctStatusRegistro> ctStatusRegistro { get; set; }
        public virtual DbSet<Edad> Edad { get; set; }
        public virtual DbSet<HistoricoRegistro> HistoricoRegistro { get; set; }
        public virtual DbSet<Pais> Pais { get; set; }
        public virtual DbSet<Registro> Registro { get; set; }
        public virtual DbSet<RegistroEventos> RegistroEventos { get; set; }
        public virtual DbSet<ServiciosInteres> ServiciosInteres { get; set; }
    }
}
