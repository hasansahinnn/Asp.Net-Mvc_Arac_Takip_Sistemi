using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SiraTakipSistemi.Models
{
    public class Firma
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FirmaAdi { get; set; }
        public string FirmaLogo { get; set; }
        public string Mail { get; set; }
        public string Username { get; set; }
        public string Sifre { get; set; }
        public int FirmaId { get; set; }
        public int IsActive { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? OdemeZamani { get; set; }
        public virtual List<Kullanici> Kullanicilar { get; set; }
        public virtual List<SeferListesi> SeferListesi { get; set; }
        public virtual List<Sofor> Sofor { get; set; }

    }
}