using FitnesAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitnesAPP.Controllers
{
    public class TrenerController : Controller
    {
        private static List<GrupniTrening> profilTreninzi = new List<GrupniTrening>();
        public ActionResult Index()
        {
            Korisnik k = (Korisnik)Session["ulogovan"];
            if (k != null)
            {
                ViewBag.ulogovan = k.KorisnickoIme;
            }
            else ViewBag.ulogovan = "";
            if (profilTreninzi.Count == 0)
            {
                List<FitnesCentar> teretane = FitnesCentar.ReadFromJson();
                return View(teretane);
            }
            else
            {
                return View(profilTreninzi);
            }
        }        
        public ActionResult Refresh()
        {
            Korisnik korisnikSession = (Korisnik)Session["ulogovan"];
            profilTreninzi.Clear();
            List<Korisnik> korisnici = Korisnik.ReadFromJson();
            foreach (Korisnik x in korisnici)
            {
                if (x.Ime == korisnikSession.Ime)
                {
                    profilTreninzi = x.TrenerTreninzi;
                }
            }
            return RedirectToAction("Profil", "Trener");
        }
        public ActionResult Profil()
        {
            List<Korisnik> sviKorisnici = Korisnik.ReadFromJson();
            Korisnik korisnikSession = (Korisnik)Session["ulogovan"];
            if (korisnikSession == null)
            {
                ViewBag.message = "Ulogujte se da biste mogli pogledati profil";
                return RedirectToAction("ProfilViewZastita", "Trener");
            }
            else
            {
                if (korisnikSession.TrenerTreninzi == null)
                {
                    korisnikSession.TrenerTreninzi = new List<GrupniTrening>();
                }
                foreach (Korisnik x in sviKorisnici)
                {
                    if (x.KorisnickoIme == korisnikSession.KorisnickoIme)
                    {
                        profilTreninzi = korisnikSession.TrenerTreninzi;
                    }
                }
                ViewBag.korisnik = korisnikSession;
                return View();
            }
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
                return RedirectToAction("EditProfil", "User");
            }
            if (updated.KorisnickoIme == null || updated.Lozinka == null || updated.Ime == null || updated.Prezime == null ||
              updated.Pol.ToString() == null || updated.Email == null || updated.DatumRodjenja == null)
            {
                ViewBag.message = "Sva polja moraju biti popunjena";
                ViewBag.korisnik = logged;
                return RedirectToAction("EditProfil", "User");
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
                    return RedirectToAction("EditProfil", "User");
                }
                if (updated.Email.Equals(x.Email) && !updated.Email.Equals(x.Email))
                {
                    ViewBag.message = $"Mejl > {updated.Email} < je zauzet";
                    return RedirectToAction("EditProfil", "User");
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
                return RedirectToAction("EditProfil", "User");
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
            foreach (GrupniTrening x in korisnikSession.TrenerTreninzi)
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
            foreach (GrupniTrening x in korisnikSession.TrenerTreninzi)
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
            foreach (GrupniTrening x in korisnikSession.TrenerTreninzi)
            {
                if (DateTime.Compare(DateTime.Parse(from), DateTime.Parse(x.DatumTreninga)) < 0 && DateTime.Compare(DateTime.Parse(to), DateTime.Parse(x.DatumTreninga)) > 0)
                {
                    profilTreninzi.Add(x);
                }
            }
            return RedirectToAction("Profil");
        }
        public ActionResult KombinovanaPretragaTrener(string naziv, string tipTreninga, string from, string to)
        {
            profilTreninzi.Clear();
            List<GrupniTrening> treninzi = GrupniTrening.ReadFromJson();
            foreach (GrupniTrening x in treninzi)
            {
                if (x.Naziv == naziv && x.TipTreninga == tipTreninga && DateTime.Compare(DateTime.Parse(from), DateTime.Parse(x.DatumTreninga)) < 0 && DateTime.Compare(DateTime.Parse(to), DateTime.Parse(x.DatumTreninga)) > 0)
                {
                    profilTreninzi.Add(x);
                }
            }
            return RedirectToAction("Profil");
        }
        public ActionResult AddView()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddGroup(GrupniTrening gt)
        {
            List<FitnesCentar> sveTeretane = FitnesCentar.ReadFromJson();
            List<Korisnik> sviKorisnici = Korisnik.ReadFromJson();
            Korisnik k = (Korisnik)Session["ulogovan"];
            List<GrupniTrening> sviTreninzi = GrupniTrening.ReadFromJson();

            foreach (GrupniTrening x in sviTreninzi)
            {
                if (x.Naziv == gt.Naziv)
                {
                    ViewBag.message = $"Trening pod nazivom {gt.Naziv} vec postoji";
                    return View("AddView");
                }
            }
            foreach (FitnesCentar x in sveTeretane)
            {
                if (x.Naziv.Equals(gt.FitnesCentar))
                {
                    break;
                }
                else
                {
                    ViewBag.message = $"Teretana > {gt.FitnesCentar} < ne postoji, probajte opet";
                    return View("AddView");
                }
            }
            if (k.CentarTrener != null && k.CentarTrener.Naziv!=gt.FitnesCentar)
            {
                ViewBag.message = $"Trener radi samo u > {gt.FitnesCentar} < ";
                return View("AddView");
            }
            if (k.CentarTrener == null)
            {
                k.CentarTrener = new FitnesCentar();
                k.CentarTrener.Naziv = gt.FitnesCentar;
            }


            if (DateTime.Parse(gt.DatumTreninga) < DateTime.Now.AddDays(3))
            {
                ViewBag.message = $"Trening mora biti zakazan najmanje 3 dana u napred!";
                return View("AddView");
            }

            int i = 0;
            int index = 0;
            gt.ListaPosetilaca = new List<Korisnik>();
            sviTreninzi.Add(gt);
            if (k.TrenerTreninzi == null)
            {
                k.TrenerTreninzi = new List<GrupniTrening>();
            }
            foreach (Korisnik x in sviKorisnici)
            {
                if (x.KorisnickoIme == k.KorisnickoIme)
                {
                    k.TrenerTreninzi.Add(gt);
                    profilTreninzi = k.TrenerTreninzi;
                    index = i;
                }
                i++;
            }
            
            sviKorisnici[index] = k;
            GrupniTrening.WriteToJson(sviTreninzi);
            Korisnik.WriteToJson(sviKorisnici);
            return RedirectToAction("Profil", "Trener");
        }
        public ActionResult Obrisi(string naziv)
        {
            List<GrupniTrening> sviTreninzi = GrupniTrening.ReadFromJson();
            Korisnik k = (Korisnik)Session["ulogovan"];
            foreach (GrupniTrening x in profilTreninzi)
            {
                if (x.Naziv == naziv && (x.ListaPosetilaca==null || x.ListaPosetilaca.Count()==0))
                {
                    profilTreninzi.Remove(x);
                    foreach(GrupniTrening gr in sviTreninzi)
                    {
                        if (gr.Naziv == naziv)
                        {
                            sviTreninzi.Remove(gr);
                            break;
                        }
                    }
                    GrupniTrening.WriteToJson(sviTreninzi);
                    k.TrenerTreninzi.Remove(x);
                    return RedirectToAction("Profil", "Trener");
                }
                else
                {
                    ViewBag.message = "Nije moguce obrisati termin";
                    return RedirectToAction("Profil", "Trener");
                }
            }
            return View("Profil");
        }
        public ActionResult Pregled(string naziv)
        {
            pomstr = naziv;
            foreach(GrupniTrening gt in profilTreninzi)
            {
                if(gt.Naziv == naziv)
                {
                    ViewBag.korisnici = gt.ListaPosetilaca;
                    ViewBag.trening = gt;
                    return View();
                }
            }
            return View();
        }
        public static string pomstr = "";
        [HttpPost]
        public ActionResult Modifikuj(GrupniTrening updated)
        {
            bool nasao = false;
            List<Korisnik> listaKorisnika = Korisnik.ReadFromJson();
            List<GrupniTrening> listaTreninga = GrupniTrening.ReadFromJson();
            Korisnik logged = (Korisnik)Session["ulogovan"];
            int index = 0;
            int index2 = 0;
            int i = 0;

            if (updated.Naziv == "" || updated.TipTreninga == "" || updated.FitnesCentar == "" || updated.TrajanjeTreninga == "" ||
                updated.DatumTreninga.ToString() == "" || updated.VremeTreninga == "" || updated.MaxBrojPosetilaca.ToString() == "")
            {
                ViewBag.message = "Sva polja moraju biti popunjena";
                ViewBag.korisnik = logged;
                return View("Modifikuj");
            }
            if (updated.Naziv == null || updated.TipTreninga == null || updated.FitnesCentar == null || updated.TrajanjeTreninga == null ||
               updated.DatumTreninga.ToString() == null || updated.VremeTreninga == null || updated.MaxBrojPosetilaca.ToString() == null)
            {
                ViewBag.message = "Sva polja moraju biti popunjena";
                ViewBag.korisnik = logged;
                return View("Modifikuj");
            }

            foreach (GrupniTrening gt in listaTreninga)
            {
                if (gt.Naziv.Equals(pomstr) && !nasao)
                {                   
                    nasao = true;
                    index = i;
                }
               
                i++;
            }
            i = 0;
            foreach (GrupniTrening gt in logged.TrenerTreninzi)
            {
                if (gt.Naziv.Equals(pomstr) && !nasao)
                {
                    nasao = true;
                    index2 = i;
                }

                i++;
            }
           

            listaTreninga[index] = updated;
            Korisnik.WriteToJson(listaKorisnika);
            logged.TrenerTreninzi[index2] = updated;
            ViewBag.korisnik = logged;
            return RedirectToAction("Profil", "Trener");
        }
    }
}