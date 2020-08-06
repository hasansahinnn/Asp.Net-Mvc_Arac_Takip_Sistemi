using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiraTakipSistemi.Models
{
    public class MesajModel
    {
        public int Status { get; set; }
        public string Baslik { get; set; }
        public string Mesaj { get; set; }
        public string Ek { get; set; }
        public string Ek2 { get; set; }
    }
}