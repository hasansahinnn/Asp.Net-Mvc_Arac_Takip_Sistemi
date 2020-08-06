using SiraTakipSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiraTakipSistemi.Controllers
{
    public class SiraTakipController : Controller
    {
        // GET: SiraTakip
        private dbcontext db = new dbcontext();
        public ActionResult Giris()
        {
            return View();
        }    
        [HttpPost]
        public ActionResult Giris(Kullanici kul,int CompanyCode=0,string remember="")
        {
        if (remember == "1")
        {
                HttpCookie cookie = new HttpCookie("KullaniciAd", kul.Username);
                HttpCookie cookie2 = new HttpCookie("FirmaId", CompanyCode.ToString());
                Response.Cookies.Add(cookie); Response.Cookies.Add(cookie2);
        }
        else
        {
                Request.Cookies.Remove("KullaniciAd"); Request.Cookies.Remove("FirmaId");
        }
            Firma firma = db.Firma.FirstOrDefault(x => x.Username == kul.Username && x.Sifre == kul.Sifre);
            if (firma != null)
                CompanyCode = firma.FirmaId;
        if (CompanyCode != 0)
        {
            Kullanici k = db.Kullanici.FirstOrDefault(x => x.Company.FirmaId == CompanyCode && x.Username == kul.Username && x.Sifre == kul.Sifre);
            if (k != null)
            {
                k.LastLogin = DateTime.Now;
                db.SaveChanges();
                Session["Kullanici"] = k; CurrentSession.Set<int>("giris", 1);
                    return Redirect("/Anasayfa/SiraListesi");
            }
        }
        TempData["GirisHata"] = "Lütfen Giriş Bilgilerinizi Kontrol Ediniz";
        return View();
        }
        [HttpPost]
        public ActionResult YeniUyelik(Firma firma,string Ad)
        {
            if(firma.Sifre.Length<8 || String.IsNullOrEmpty(firma.FirmaAdi) || String.IsNullOrEmpty(firma.Sifre) || String.IsNullOrEmpty(firma.Username) || String.IsNullOrEmpty(firma.Mail) || String.IsNullOrEmpty(Ad))
            {
                TempData["GirisHata"] = "Lütfen Üyelik Açarken Gerekli Yerleri Doldurunuz";
                TempData["FirmaAdi"] = firma.FirmaAdi;
                TempData["Sifre"] = firma.Sifre;
                TempData["Username"] = firma.Username;
                TempData["Mail"] = firma.Mail;
                TempData["Ad"] = Ad;
                if(firma.Sifre.Length < 8)
                   TempData["GirisHata"] = "Lütfen Şifrenizi En Az 7 Haneli Olucak Şekilde Yapınız";
                return RedirectToAction("Giris");
            }
            if (db.Firma.FirstOrDefault(x => x.Username == firma.Username) != null)
            {
                TempData["GirisHata"] = "Lütfen Üyelik Açarken Gerekli Yerleri Doldurunuz";
                TempData["FirmaAdi"] = firma.FirmaAdi;
                TempData["Sifre"] = firma.Sifre;
                TempData["Username"] = firma.Username;
                TempData["Mail"] = firma.Mail;
                TempData["Ad"] = Ad;
                TempData["GirisHata"] = "Bu Kullanıcı Adı Mevcut. Lütfen Başka Deneyiniz.";
                return Redirect("/Anasayfa/SiraListesi");
            }
            firma.CreatedTime = DateTime.Now;
            Random r = new Random();
            bool kontrol = true;
            while (kontrol)
            {
                int rand = r.Next(1000, 9999);
                if (db.Firma.FirstOrDefault(x => x.FirmaId == rand) == null)
                { kontrol = false; firma.FirmaId = rand; }
            }
            firma.IsActive = 1;
            db.Firma.Add(firma);
            db.SaveChanges();
            Kullanici k = new Kullanici();
            k.Ad = Ad;
            k.Username = firma.Username;
            k.Sifre = firma.Sifre;
            k.Yetki = 1;
            k.LastLogin = DateTime.Now;
            k.IsActive = 1;k.Company = firma;
            db.Kullanici.Add(k);
            db.SaveChanges();
            CurrentSession.Set<int>("giris", 1);
            Session["Kullanici"] = k;
            return Redirect("/Anasayfa/SiraListesi");
        }
        public ActionResult Cikis()
        {
            CurrentSession.Set<int>("giris", 0);
            Session.Clear();
            return RedirectToAction("Giris");
        }
    }
}