using SiraTakipSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiraTakipSistemi.Controllers
{
    [Auth]
    public class AnasayfaController : Controller
    {
        private dbcontext db = new dbcontext();
        MesajModel mesaj = new MesajModel();

        #region Sira_İşlemleri

        public ActionResult SiraListesi()
        {
            Kullanici k = GetKullanici();
            List<SeferListesi> s = db.SeferListesi.Where(x => x.Company.FirmaId == k.Company.FirmaId && x.IsActive == 1).OrderBy(x => x.SiraNo).ToList();
            List<Sofor> soforler = db.Sofor.Where(x=>x.Company.FirmaId==k.Company.FirmaId && x.IsActive==1).ToList();
            TempData["Soforler"] = soforler;
            return View(s);
        }
        [HttpPost]
        public ActionResult SiraListesi(int a)
        {
            return View();
        }
        public JsonResult UpdateOrder(int fromPosition, int toPosition)
        {
            try
            {
                Kullanici k = Session["Kullanici"] as Kullanici;
                string direction = fromPosition < toPosition ? "back" : "";
                if (direction == "back")
                {
                    List<SeferListesi> movedCompanies = db.SeferListesi
                                .Where(c => (toPosition >= c.SiraNo && c.SiraNo > fromPosition && k.Company.FirmaId == c.Company.FirmaId))
                                .ToList();
                    movedCompanies.ForEach(x => x.SiraNo--);
                }
                else
                {
                    List<SeferListesi> movedCompanies = db.SeferListesi
                                .Where(c => (fromPosition > c.SiraNo && c.SiraNo >= toPosition && k.Company.FirmaId == c.Company.FirmaId))
                                .ToList();
                    movedCompanies.ForEach(x => x.SiraNo++);
                }
                SeferListesi s = db.SeferListesi.First(c => c.SiraNo == fromPosition && k.Company.FirmaId == c.Company.FirmaId);
                s.SiraNo = toPosition;
                db.SaveChanges();
                mesaj.Status = 1;
                mesaj.Baslik = "Başarılı";
                mesaj.Mesaj = "Sıra Güncellendi";
                return Json(mesaj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                mesaj.Status = 0;
                mesaj.Baslik = "Hata";
                mesaj.Mesaj = "Sıra Güncellerken Hata Oluştu";
                return Json(mesaj, JsonRequestBehavior.AllowGet);
            }
        }
        public void ReorderRows(int sirano)
        {
            Kullanici k = GetKullanici();
            List<SeferListesi> seferler = db.SeferListesi.Where(x => x.Company.FirmaId == k.Company.FirmaId && x.IsActive == 1 && x.SiraNo > sirano).ToList();
            seferler.ForEach(x => x.SiraNo--);
            db.SaveChanges();
        }
        [HttpPost]
        public JsonResult SiraEkle(int Soforid,string Not)
        {
            try
            {
                Kullanici k = GetKullanici();
                SeferListesi sefer = db.SeferListesi.Where(x => x.Company.FirmaId == k.Company.FirmaId && x.IsActive==1).OrderByDescending(x => x.SiraNo).FirstOrDefault();
                Sofor sofor = db.Sofor.Where(x => x.Company.FirmaId == k.Company.FirmaId && x.Id == Soforid).FirstOrDefault();
                SeferListesi yenisefer = new SeferListesi();
                yenisefer.SiraNo =sefer==null?1: sefer.SiraNo + 1;
                yenisefer.Sofor = sofor;
                yenisefer.SoforAdi = sofor.Plaka + " - " + sofor.Ad;
                yenisefer.Company = db.Firma.FirstOrDefault(x=>x.FirmaId==k.Company.FirmaId);
                yenisefer.IsActive = 1;
                yenisefer.Not = Not;
                yenisefer.SeferZamani = DateTime.Now;
                db.SeferListesi.Add(yenisefer);
                db.SaveChanges();
                mesaj.Status = 1;
                mesaj.Baslik = "Başarılı";
                mesaj.Mesaj = "Sıra Başarılı Bir Şekilde Eklendi";
                mesaj.Ek = yenisefer.Id + "_" + yenisefer.SiraNo + "_" + yenisefer.SoforAdi + "_" + yenisefer.SeferZamani+"_"+Not; 
                return Json(mesaj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                mesaj.Status = 0;
                mesaj.Baslik = "Hata";
                mesaj.Mesaj = "Sıraya Eklemede Hata Oluştu";
                return Json(mesaj, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SiraOnay(int seferid)
        {
            try
            {
                Kullanici k = GetKullanici();
                SeferListesi sefer= db.SeferListesi.Where(x => x.Company.FirmaId == k.Company.FirmaId && x.Id == seferid).FirstOrDefault();
                ReorderRows(sefer.SiraNo);
                sefer.IsActive = 0;sefer.SiraNo = 9999;
                sefer.SeferZamani = DateTime.Now;
                db.SaveChanges();
                mesaj.Status = 1;
                mesaj.Baslik = "Başarılı";
                mesaj.Mesaj = "Sıra Başarılı Bir Şekilde Tamamlandi";
                mesaj.Ek = seferid.ToString();
                return Json(mesaj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                mesaj.Status = 0;
                mesaj.Baslik = "Hata";
                mesaj.Mesaj = "Sıraya Eklemede Hata Oluştu";
                return Json(mesaj, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SiraSil(int seferid)
        {
            try
            {
                Kullanici k = GetKullanici();
                SeferListesi sefer = db.SeferListesi.Where(x => x.Company.FirmaId == k.Company.FirmaId && x.Id == seferid).FirstOrDefault();
                if(sefer.IsActive==1)
                   ReorderRows(sefer.SiraNo);
                db.SeferListesi.Remove(sefer);
                db.SaveChanges();
                mesaj.Status = 1;
                mesaj.Baslik = "Başarılı";
                mesaj.Mesaj = "Sıra Başarılı Bir Şekilde Silindi";
                mesaj.Ek = seferid.ToString();
                return Json(mesaj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                mesaj.Status = 0;
                mesaj.Baslik = "Hata";
                mesaj.Mesaj = "Sıraya Eklemede Hata Oluştu";
                return Json(mesaj, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult SiraGecmisi(int Soforid = 0,string tarih="")
        {
            Kullanici k = GetKullanici();
            List<SeferListesi> sefergecmisi;
            List<Sofor> soforler = db.Sofor.Where(x => x.Company.FirmaId == k.Company.FirmaId && x.IsActive == 1).ToList();
            TempData["Soforler"] = soforler;
            if (Request.IsAjaxRequest())
            {
                DateTime tarih1=DateTime.Now, tarih2= DateTime.Now;
                if (tarih != "")
                {
                    tarih1 = Convert.ToDateTime(tarih.Split('-')[0]);
                    tarih2 = Convert.ToDateTime(tarih.Split('-')[1]);
                }
                if (Soforid == 0 && tarih == "")
                    sefergecmisi = db.SeferListesi.Where(x => x.Company.FirmaId == k.Company.FirmaId && x.IsActive == 0).OrderByDescending(x => x.SeferZamani).ToList();
                else if (Soforid == 0)
                    sefergecmisi = db.SeferListesi.Where(x => x.Company.FirmaId == k.Company.FirmaId && x.IsActive == 0 && x.SeferZamani>=tarih1 && x.SeferZamani<=tarih2).OrderByDescending(x => x.SeferZamani).ToList();
                else if (tarih == "")
                    sefergecmisi = db.SeferListesi.Where(x => x.Company.FirmaId == k.Company.FirmaId && x.IsActive == 0 && x.Sofor.Id==Soforid).OrderByDescending(x => x.SeferZamani).ToList();
                else
                    sefergecmisi = db.SeferListesi.Where(x => x.Company.FirmaId == k.Company.FirmaId && x.IsActive == 0 && x.SeferZamani >= tarih1 && x.SeferZamani <= tarih2 &&x.Sofor.Id==Soforid).OrderByDescending(x => x.SeferZamani).ToList();
                return PartialView("_SeferGecmisTablosu", sefergecmisi);
            }
            else
            {
                sefergecmisi = db.SeferListesi.Where(x => x.Company.FirmaId == k.Company.FirmaId && x.IsActive == 0).OrderByDescending(x => x.SeferZamani).ToList();
                return View(sefergecmisi);
            }
        }

        #endregion

        #region Şöför_İşlemleri
        public ActionResult Soforler()
        {
            Kullanici k = GetKullanici();
            List<Sofor> soforler;
            soforler = db.Sofor.Where(x => x.Company.FirmaId == k.Company.FirmaId).OrderByDescending(x => x.Id).ToList();
            if (Request.IsAjaxRequest())
            {
               return PartialView("_SoforTablosu", soforler);
            }
            else
            {
                return View(soforler);
            }
        }
        public ActionResult SoforActive(int Soforid=0, int Active=0)
        {
            Kullanici k = GetKullanici();
            if (Soforid != 0)
            {
                Sofor sofor = db.Sofor.FirstOrDefault(x => x.Company.FirmaId == k.Company.FirmaId && x.Id == Soforid);
                if (sofor != null)
                {
                    sofor.IsActive = Active;
                    if (Active == -1)
                    {
                        Random r = new Random();
                        sofor.Username = "xxx" + r.Next(10000000, 99999999).ToString();
                    }
                }
                db.SaveChanges();
            }
            List<Sofor> soforler = soforler = db.Sofor.Where(x => x.Company.FirmaId == k.Company.FirmaId).OrderByDescending(x => x.Id).ToList();
            return PartialView("_SoforTablosu", soforler);
        }
        public ActionResult SoforEkle(Sofor s)
        {
            if(String.IsNullOrEmpty(s.Ad) || String.IsNullOrEmpty(s.Plaka))
            {
                mesaj.Status = 0;
                mesaj.Baslik = "Dikkat";
                mesaj.Mesaj = "Ad-Soyad ve Plakanın Girilmesi Gerekmektedir.";
                return Json(mesaj, JsonRequestBehavior.AllowGet);
            }
            Kullanici k = GetKullanici();
            Firma firma = db.Firma.FirstOrDefault(x => x.FirmaId == k.Company.FirmaId);
            s.Company = firma;
                Random r = new Random();
            if (String.IsNullOrEmpty(s.Username) )
                s.Username = s.Ad + r.Next(1000, 10000);
            if (String.IsNullOrEmpty(s.Sifre) )
                s.Sifre = r.Next(10000000, 99999999).ToString();
            if (s.Id == 0 && db.Sofor.FirstOrDefault(x=>x.Company.FirmaId==k.Company.FirmaId && x.Username==s.Username)==null)
            {
                s.IsActive = 1;
                db.Sofor.Add(s);
                db.SaveChanges();
                mesaj.Status = 1;
                mesaj.Baslik = "Başarılı";
                mesaj.Mesaj = "Şöför Başarılı Bir Şekilde Eklendi";
                mesaj.Ek = s.Id.ToString()+"_"+s.Username+"_"+s.Sifre;
                mesaj.Ek2 = "0";
            }
            else
            {
                Sofor sofor = db.Sofor.FirstOrDefault(x => x.Id == s.Id && x.Company.FirmaId == k.Company.FirmaId);
                if (sofor != null)
                {
                    sofor.Ad = s.Ad;
                    sofor.Plaka = s.Plaka;
                    sofor.IsActive = 1;
                    sofor.Sifre = s.Sifre;
                    sofor.Username = s.Username;
                    sofor.Telefon = s.Telefon;
                    sofor.Adres = s.Adres;
                    mesaj.Status = 1;
                    mesaj.Baslik = "Başarılı";
                    mesaj.Mesaj = "Şöför Başarılı Bir Şekilde Güncellendi";
                    mesaj.Ek = s.Id.ToString() + "_" + s.Ad + "_" + s.Username + "_" + s.Sifre + "_" + s.Telefon + "_" + s.Adres + "_" + s.Plaka;
                    mesaj.Ek2 = "1";
                }
                else
                {
                    if(db.Sofor.FirstOrDefault(x => x.Company.FirmaId == k.Company.FirmaId && x.Username == s.Username) != null)
                    {
                        mesaj.Status = 0;
                        mesaj.Baslik = "Başarısız";
                        mesaj.Mesaj = "Şöföre Girilen Kullanıcı Adı Mevcut. Lütfen Boş Bırakın veya Başka Girin";
                    }
                    else
                    {
                        mesaj.Status = 0;
                        mesaj.Baslik = "Başarısız";
                        mesaj.Mesaj = "Şöför Bulunamadı! Lütfen Sayfayı Yeniden Başlatıp Tekrar Deneyiniz.";
                    }
                }
            }
            db.SaveChanges();
            return Json(mesaj, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Kullanici_İşlemleri

        public ActionResult Kullanici()
        {
            Kullanici k = GetKullanici();
            List<Kullanici> kullanicilar;
            kullanicilar = db.Kullanici.Where(x => x.Company.FirmaId == k.Company.FirmaId && x.IsActive == 1).OrderByDescending(x => x.Id).ToList();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_KullaniciTablosu", kullanicilar);
            }
            else
            {
                return View(kullanicilar);
            }
        }
        public ActionResult KullaniciActive(int Kullaniciid = 0, int Active = 0)
        {
            Kullanici k = GetKullanici();
            if (Kullaniciid != 0)
            {
                Kullanici kullanici = db.Kullanici.FirstOrDefault(x => x.Company.FirmaId == k.Company.FirmaId && x.Id == Kullaniciid);
                if (kullanici != null)
                {
                    kullanici.IsActive = Active;
                    if (Active == -1)
                    {
                        db.Kullanici.Remove(db.Kullanici.FirstOrDefault(x => x.Id == Kullaniciid && x.Company.FirmaId == k.Company.FirmaId));
                    }
                }
                db.SaveChanges();
            }
            List<Kullanici> kullanicilar = db.Kullanici.Where(x => x.Company.FirmaId == k.Company.FirmaId).OrderByDescending(x => x.Id).ToList();
            return PartialView("_KullaniciTablosu", kullanicilar);
        }
        public ActionResult KullaniciEkle(Kullanici s)
        {
            if (String.IsNullOrEmpty(s.Username) || String.IsNullOrEmpty(s.Sifre) || String.IsNullOrEmpty(s.Ad))
            {
                mesaj.Status = 0;
                mesaj.Baslik = "Dikkat";
                mesaj.Mesaj = "Kullanıcı Adı ve Şifrenin Girilmesi Gerekmektedir.";
                return Json(mesaj, JsonRequestBehavior.AllowGet);
            }
            Kullanici k = GetKullanici();
            Firma firma = db.Firma.FirstOrDefault(x => x.FirmaId == k.Company.FirmaId);
            s.Company = firma;
            Random r = new Random();
            if (String.IsNullOrEmpty(s.Username))
                s.Username = s.Ad + r.Next(1000, 10000);
            if (String.IsNullOrEmpty(s.Sifre))
                s.Sifre = r.Next(10000000, 99999999).ToString();
            if (s.Id == 0 && db.Sofor.FirstOrDefault(x => x.Company.FirmaId == k.Company.FirmaId && x.Username == s.Username) == null)
            {
                s.IsActive = 1;s.Yetki = 0;
                db.Kullanici.Add(s);
                db.SaveChanges();
                mesaj.Status = 1;
                mesaj.Baslik = "Başarılı";
                mesaj.Mesaj = "Kullanici Başarılı Bir Şekilde Eklendi";
                mesaj.Ek = s.Id.ToString() + "_" + s.Username + "_" + s.Sifre;
                mesaj.Ek2 = "0";
            }
            else
            {
                Kullanici kullanici = db.Kullanici.FirstOrDefault(x => x.Id == s.Id && x.Company.FirmaId == k.Company.FirmaId);
                if (kullanici != null)
                {
                    kullanici.Ad = s.Ad;
                    kullanici.IsActive = 1;
                    kullanici.Sifre = s.Sifre;
                    kullanici.Username = s.Username;
                    mesaj.Status = 1;
                    mesaj.Baslik = "Başarılı";
                    mesaj.Mesaj = "Kullanici Başarılı Bir Şekilde Güncellendi";
                    mesaj.Ek = kullanici.Id.ToString() + "_" + kullanici.Ad + "_" + kullanici.Username + "_" + kullanici.Sifre;
                    mesaj.Ek2 = "1";
                }
                else
                {
                    if (db.Kullanici.FirstOrDefault(x => x.Company.FirmaId == k.Company.FirmaId && x.Username == s.Username) != null)
                    {
                        mesaj.Status = 0;
                        mesaj.Baslik = "Başarısız";
                        mesaj.Mesaj = "Girilen Kullanıcı Adı Mevcut. Lütfen Boş Bırakın veya Başka Girin";
                    }
                    else
                    {
                        mesaj.Status = 0;
                        mesaj.Baslik = "Başarısız";
                        mesaj.Mesaj = "Kullanıcı Bulunamadı! Lütfen Sayfayı Yeniden Başlatıp Tekrar Deneyiniz.";
                    }
                }
            }
            db.SaveChanges();
            return Json(mesaj, JsonRequestBehavior.AllowGet);
        }

        #endregion


        public Kullanici GetKullanici()
        {
            Kullanici k = Session["Kullanici"] as Kullanici;
            return k;
        }

    }
}