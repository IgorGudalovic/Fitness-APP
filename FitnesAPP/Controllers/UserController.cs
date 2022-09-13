using FitnesAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitnesAPP.Controllers
{
    public class UserController : Controller
    {
        private static List<GrupniTrening> profilTreninzi = new List<GrupniTrening>();
        public ActionResult Refresh()
        {
            Korisnik korisnikSession = (Korisnik)Session["ulogovan"];
            profilTreninzi.Clear();
            List<Korisnik> korisnici = Korisnik.ReadFromJson();
            foreach (Korisnik x in korisnici)
            {                
                if(x.Ime == korisnikSession.Ime)
                {
                    profilTreninzi = x.PosetilacTreninzi;
                }    
            }
            return RedirectToAction("Profil", "User");
        }
        public ActionResult Index()
        {
            Korisnik k = (Korisnik)Session["ulogovan"];
            if (k != null)
            {
                ViewBag.message = $"Vec ste ulogovani kao {k.KorisnickoIme}";
                return View("Notification");
            }
            return View("Login");
        }
        public ActionResult Profil()
        {
            List<Korisnik> korisnici = Korisnik.ReadFromJson();
            Korisnik korisnikSession = (Korisnik)Session["ulogovan"];
            if (korisnikSession == null)
            {
                ViewBag.message = "Ulogujte se da biste mogli pogledati profil";
                return RedirectToAction("ProfilViewZastita", "User");
            }
            else
            {
                if (korisnikSession.Uloga.ToString().Equals("Posetilac"))
                {
                    if (korisnikSession.PosetilacTreninzi == null)
                    {
                        korisnikSession.PosetilacTreninzi = new List<GrupniTrening>();
                    }
                    foreach (Korisnik x in korisnici)
                    {
                        if (x.Ime == korisnikSession.Ime)
                        {
                            profilTreninzi = x.PosetilacTreninzi;
                        }
                    }
                    korisnikSession.PosetilacTreninzi = profilTreninzi;
                    ViewBag.korisnik = korisnikSession;
                    return View();
                }
                if (korisnikSession.Uloga.ToString().Equals("Trener"))
                {
                    return RedirectToAction("Profil", "Trener");
                }
                else
                {
                    return RedirectToAction("Profil", "Vlasnik");
                }

            }
        }

        public ActionResult EditProfil(string naziv)
        {
            List<Korisnik> sviKorisnici = Korisnik.ReadFromJson();
            Korisnik k = new Korisnik();

            foreach (Korisnik x in sviKorisnici)
            {
                if (x.KorisnickoIme.Equals(naziv))
                {
                    k = x;
                    break;
                }
            }
            ViewBag.korisnik = k;
            return View();
        }
        [HttpPost]
        public ActionResult Edit(Korisnik updated)
        {
            bool nasao = false;
            List<Korisnik> listaKorisnika = Korisnik.ReadFromJson();
            Korisnik logged = (Korisnik)Session["ulogovan"];
            int index = 0;
            int foundIndex = 0;

            if (updated.KorisnickoIme == "" || updated.Lozinka == "" || updated.Ime == "" || updated.Prezime == "" ||
                updated.Pol.ToString() == "" || updated.Email == "" || updated.DatumRodjenja == "")
            {
                ViewBag.message = "Sva polja moraju biti popunjena";
                ViewBag.korisnik = logged;
                return View("EditProfil");
            }
            if (updated.KorisnickoIme == null || updated.Lozinka == null || updated.Ime == null || updated.Prezime == null ||
              updated.Pol.ToString() == null || updated.Email == null || updated.DatumRodjenja == null)
            {
                ViewBag.message = "Sva polja moraju biti popunjena";
                ViewBag.korisnik = logged;
                return View("EditProfil");
            }

            foreach (Korisnik x in listaKorisnika)
            {
                if (x.KorisnickoIme.Equals(logged.KorisnickoIme) && !nasao)
                {
                    logged = x;
                    nasao = true;
                    foundIndex = index;
                }
                if (updated.KorisnickoIme.Equals(x.KorisnickoIme) && !updated.KorisnickoIme.Equals(logged.KorisnickoIme))
                {
                    ViewBag.message = $"Korisnicko ime > {updated.KorisnickoIme}< je zauzeto";
                    return View("Notification");
                }
                if (updated.Email.Equals(x.Email) && !updated.Email.Equals(logged.Email))
                {
                    ViewBag.message = $"Mejl > {updated.Email} < je zauzet";
                    return View("Notification");
                }
                index++;
            }

            try
            {
                DateTime unetDatum = Convert.ToDateTime(updated.DatumRodjenja);
            }
            catch
            {
                ViewBag.message = $"Uneti datum rodjenja je pogresnog formata";
                return View("Notification");
            }

            updated.PosetilacTreninzi = logged.PosetilacTreninzi;
            updated.TrenerTreninzi = logged.TrenerTreninzi;
            updated.CentarTrener = logged.CentarTrener;
            updated.CentarVlasnik = logged.CentarVlasnik;
            updated.Uloga = logged.Uloga;

            listaKorisnika[foundIndex] = updated;
            Korisnik.WriteToJson(listaKorisnika);
            Session["ulogovan"] = updated;
            ViewBag.ulogovan = updated.KorisnickoIme;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ProfilViewZastita()
        {
            Korisnik k = (Korisnik)Session["ulogovan"];
            if (k == null)
            {
                ViewBag.message = "Ulogujte se da biste mogli pogledati profil";
                return View("Login");
            }
            return View("Login");
        }

        public ActionResult Register()
        {
            Korisnik k = new Korisnik();
            Session["ulogovan"] = k;
            ViewBag.ulogovan = k.KorisnickoIme;
            return View(k);
        }
        [HttpPost]
        public ActionResult Register(Korisnik k)
        {
            List<Korisnik> listaKorisnika = Korisnik.ReadFromJson();
            foreach (Korisnik x in listaKorisnika)
            {
                if (x.KorisnickoIme.Equals(k.KorisnickoIme))
                {
                    ViewBag.Message = $"Korisnik sa imenom > {k.KorisnickoIme} < vec postoji!";
                    return View("Register");
                }
            }
            foreach (Korisnik x in listaKorisnika)
            {
                if (x.Email.Equals(k.Email))
                {
                    ViewBag.Message = $"Korisnik sa mejlon > {k.Email} < vec postoji!";
                    return View("Register");
                }
            }
            if (k.KorisnickoIme == "" || k.Lozinka == "" || k.Ime == "" || k.Prezime == "" ||
               k.Pol.ToString() == "" || k.Email == "" || k.DatumRodjenja == "")
            {
                ViewBag.message = "Sva polja moraju biti popunjena";
                return View("Register");
            }
            if (k.KorisnickoIme == null || k.Lozinka == null || k.Ime == null || k.Prezime == null ||
              k.Pol.ToString() == null || k.Email == null || k.DatumRodjenja == null)
            {
                ViewBag.message = "Sva polja moraju biti popunjena";
                return View("Register");
            }
            
            if (k.Email.Contains(".com") == false || k.Email.Contains("@") == false)
            {
                ViewBag.message = "Email nije unet kako treba";
                ViewBag.message = "Primer: imenkoprezimic@gmail.com";
                return View("Register");
            }
            k.Uloga = Uloga.Posetilac;
            listaKorisnika.Add(k);
            Korisnik.WriteToJson(listaKorisnika);
            Session["ulogovan"] = k;
            return RedirectToAction("Index", "Home");
        }
        
        public ActionResult Login()
        {
            return View("Login");
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {

            if (username.Equals(String.Empty) || username.Equals(String.Empty))
            {
                ViewBag.message = "Sva polja moraju biti popunjena";
                return View("Login");
            }
            List<Korisnik> listaKorisnika = Korisnik.ReadFromJson();
            Korisnik user = listaKorisnika.Find(u => u.KorisnickoIme.Equals(username) && u.Lozinka.Equals(password));
            if (user == null)
            {
                ViewBag.Message = $"Korisnik sa unetim korisnickim imenom i lozinkom ne postoji!";
                return View("Login");
            }

            Session["ulogovan"] = user;
            ViewBag.ulogovan = user.KorisnickoIme;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Logout()
        {
            Session["ulogovan"] = null;
            ViewBag.ulogovan = "";
            return RedirectToAction("Index", "Home");

        }
        public ActionResult SortBy(string opcija)
        {
            if (opcija.Equals("Sortiraj po imenu rastuce"))
            {
                profilTreninzi = profilTreninzi.OrderBy(x => x.Naziv).ToList();
                return RedirectToAction("Profil");
            }
            if (opcija.Equals("Sortiraj po imenu opadajuce"))
            {
                profilTreninzi = profilTreninzi.OrderBy(x => x.Naziv).Reverse().ToList();
                return RedirectToAction("Profil");
            }
            if (opcija.Equals("Sortiraj po tipu rastuce"))
            {
                profilTreninzi = profilTreninzi.OrderBy(x => x.TipTreninga).ToList();
                return RedirectToAction("Profil");
            }
            if (opcija.Equals("Sortiraj po tipu opadajuce"))
            {
                profilTreninzi = profilTreninzi.OrderBy(x => x.TipTreninga).Reverse().ToList();
                return RedirectToAction("Profil");
            }
            if (opcija.Equals("Sortiraj po datumu rastuce"))
            {
                profilTreninzi = profilTreninzi.OrderBy(x => x.DatumTreninga).ToList();
                return RedirectToAction("Profil");
            }
            if (opcija.Equals("Sortiraj po datumu opadajuce"))
            {
                profilTreninzi = profilTreninzi.OrderBy(x => x.DatumTreninga).Reverse().ToList();
                return RedirectToAction("Profil");
            }
            else return RedirectToAction("Profil");
        }
        public ActionResult NadjiIme(string naziv)
        {
            Korisnik korisnikSession = (Korisnik)Session["ulogovan"];
            profilTreninzi.Clear();
            foreach (GrupniTrening x in korisnikSession.PosetilacTreninzi)
            {
                if (x.Naziv == naziv)
                {
                    profilTreninzi.Add(x);
                }
            }

            return RedirectToAction("Profil");
        }
        public ActionResult NadjiTip(string tipTreninga)
        {
            Korisnik korisnikSession = (Korisnik)Session["ulogovan"];
            profilTreninzi.Clear();
            foreach (GrupniTrening x in korisnikSession.PosetilacTreninzi)
            {
                if (x.TipTreninga == tipTreninga)
                {
                    profilTreninzi.Add(x);
                }
            }

            return RedirectToAction("Profil");
        }
        public ActionResult NadjiDatum(string from, string to)
        {
            Korisnik korisnikSession = (Korisnik)Session["ulogovan"];
            profilTreninzi.Clear();            
            foreach (GrupniTrening x in korisnikSession.PosetilacTreninzi)
            {
                if(DateTime.Compare(DateTime.Parse(from), DateTime.Parse(x.DatumTreninga)) < 0 && DateTime.Compare(DateTime.Parse(to), DateTime.Parse(x.DatumTreninga)) > 0)
                {
                    profilTreninzi.Add(x);
                }
            }
            return RedirectToAction("Profil");
        }
        public ActionResult KombinovanaPretragaPosetilac(string naziv, string tipTreninga, string from, string to)
        {
            profilTreninzi.Clear();
            List<GrupniTrening> treninzi = GrupniTrening.ReadFromJson();
            foreach (GrupniTrening x in treninzi)
            {
                if(x.Naziv == naziv && x.TipTreninga == tipTreninga && DateTime.Compare(DateTime.Parse(from), DateTime.Parse(x.DatumTreninga)) < 0 && DateTime.Compare(DateTime.Parse(to), DateTime.Parse(x.DatumTreninga)) > 0)
                {
                    profilTreninzi.Add(x);
                }
            }
            return RedirectToAction("Profil");
        }
    }
}