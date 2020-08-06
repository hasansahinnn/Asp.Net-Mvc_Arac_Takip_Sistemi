using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace SiraTakipSistemi.Models
{
    public class Sofor
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Username { get; set; }
        public string Sifre { get; set; }
        public string Telefon { get; set; }
        public string Adres { get; set; }
        public string Plaka { get; set; }
        public int IsActive { get; set; }
        public virtual Firma Company { get; set; }
        public virtual List<SeferListesi> Seferler { get; set; }

    }
}