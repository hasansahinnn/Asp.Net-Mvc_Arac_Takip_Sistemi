using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SiraTakipSistemi.Models
{
    public class SeferListesi
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SiraNo { get; set; }
        public string SoforAdi { get; set; }
        public string Not { get; set; }
        public int IsActive { get; set; }
        public DateTime? SeferZamani { get; set; }
        public virtual Firma Company { get; set; }
        public virtual Sofor Sofor { get; set; }
    }
}