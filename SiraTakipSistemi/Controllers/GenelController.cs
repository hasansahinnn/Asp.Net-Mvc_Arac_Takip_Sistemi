using SiraTakipSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiraTakipSistemi.Controllers
{
    public class GenelController : Controller
    {
        // GET: Genel
        dbcontext db = new dbcontext();
        
     
        public ActionResult Liste(int? id)
        {
            try
            { Firma f = db.Firma.FirstOrDefault(x => x.FirmaId == id);
                if (f == null)
                {
                    TempData["GirisHata"] = "Bu Koda Sahip Bir Firma Bulunamadı..";
                    return Redirect("/SiraTakip/Giris");
                }
                TempData["FirmaAdi"] = f.FirmaAdi;
                TempData["FirmaId"] = f.FirmaId;
                List<SeferListesi> seferler = db.SeferListesi.Where(x => x.Company.FirmaId == id && x.IsActive == 1).OrderBy(x => x.SiraNo).ToList();

                if (Request.IsAjaxRequest())
                {
                     return PartialView("~/Views/Genel/_GenelTablo.cshtml", seferler);
                }

                return View(seferler);
            }
            catch (Exception)
            {
                throw new HttpException(404, "Firma Bulunamadı!");
            }
        }
    }
}