using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SiraTakipSistemi.Models
{
    public class dbcontext : DbContext
    {
        public dbcontext() : base("Name=DatabaseContext")
        {
        }
        public DbSet<Firma> Firma { get; set; }
        public DbSet<Kullanici> Kullanici { get; set; }
        public DbSet<SeferListesi> SeferListesi { get; set; }
        public DbSet<Sofor> Sofor { get; set; }


    }
}