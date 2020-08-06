using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace SiraTakipSistemi.Models
{
    public class Kullanici
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Username { get; set; }
        public string Sifre { get; set; }
        public int IsActive { get; set; }
        public int Yetki { get; set; }

        public DateTime? LastLogin { get; set; }
        public virtual Firma Company { get; set; }

    }
}