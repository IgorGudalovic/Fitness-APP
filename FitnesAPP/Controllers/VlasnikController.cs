using FitnesAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitnesAPP.Controllers
{
    public class VlasnikController : Controller
    {
        private static List<FitnesCentar> profilFitnesCentri = new List<FitnesCentar>();
        private static AdresaFitnesCentra staticAdr = new AdresaFitnesCentra();
        public ActionResult Index()
        {
            Korisnik k = (Korisnik)Session["ulogovan"];
            if (k != null)
            {
                ViewBag.ulogovan = k.KorisnickoIme;
            }
            else ViewBag.ulogovan = "";
            if (profilFitnesCentri.Count == 0)
            {
                List<FitnesCentar> teretane = FitnesCentar.ReadFromJson();
                return View(teretane);
            }
            else
            {
                return View(profilFitnesCentri);
            }
        }

        public ActionResult Refresh()
        {
            Korisnik korisnikSession = (Korisnik)Session["ulogovan"];
            profilFitnesCentri.Clear();
            List<Korisnik> korisnici = Korisnik.ReadFromJson();
            foreach (Korisnik x in korisnici)
            {
                if (x.Ime == korisnikSession.Ime)
                {
                    profilFitnesCentri = x.CentarVlasnik;
                }
            }
            return RedirectToAction("Profil", "Vlasnik");
        }
        public ActionResult Profil()
        {
            List<Korisnik> korisnici = Korisnik.ReadFromJson();
            Korisnik korisnikSession = (Korisnik)Session["ulogovan"];
            if (korisnikSession == null)
            {
                ViewBag.message = "Ulogujte se da biste mogli pogledati profil";
                return RedirectToAction("ProfilViewZastita", "Vlasnik");
            }
            else
            {
                if (korisnikSession.CentarVlasnik == null)
                {
                    korisnikSession.CentarVlasnik = new List<FitnesCentar>();
                }
                foreach (Korisnik x in korisnici)
                {
                    if (x.KorisnickoIme == korisnikSession.KorisnickoIme)
                    {
                        korisnikSession = x;
                        profilFitnesCentri = x.CentarVlasnik;
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
            updated.CentarVlasnik = logged.CentarVlasnik;
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
                profilFitnesCentri = profilFitnesCentri.OrderBy(x => x.Naziv).ToList();
                return RedirectToAction("Profil");
            }
            if (opcija.Equals("Sortiraj po imenu opadajuce"))
            {
                profilFitnesCentri = profilFitnesCentri.OrderBy(x => x.Naziv).Reverse().ToList();
                return RedirectToAction("Profil");
            }
            if (opcija.Equals("Sortiraj po adresi rastuce"))
            {
                profilFitnesCentri = profilFitnesCentri.OrderBy(x => x.Adresa).ToList();
                return RedirectToAction("Profil");
            }
            if (opcija.Equals("Sortiraj po adresi opadajuce"))
            {
                profilFitnesCentri = profilFitnesCentri.OrderBy(x => x.Adresa).Reverse().ToList();
                return RedirectToAction("Profil");
            }
            if (opcija.Equals("Sortiraj po godini rastuce"))
            {
                profilFitnesCentri = profilFitnesCentri.OrderBy(x => x.GodOtvaranja).ToList();
                return RedirectToAction("Profil");
            }
            if (opcija.Equals("Sortiraj po godini opadajuce"))
            {
                profilFitnesCentri = profilFitnesCentri.OrderBy(x => x.GodOtvaranja).Reverse().ToList();
                return RedirectToAction("Profil");
            }
            else return RedirectToAction("Profil");
        }
        public ActionResult NadjiIme(string naziv)
        {
            Korisnik korisnikSession = (Korisnik)Session["ulogovan"];
            profilFitnesCentri.Clear();
            foreach (FitnesCentar x in korisnikSession.CentarVlasnik)
            {
                if (x.Naziv == naziv)
                {
                    profilFitnesCentri.Add(x);
                }
            }

            return RedirectToAction("Profil");
        }
        public ActionResult NadjiAdresu(string adresa)
        {
            Korisnik korisnikSession = (Korisnik)Session["ulogovan"];
            profilFitnesCentri.Clear();
            foreach (FitnesCentar x in korisnikSession.CentarVlasnik)
            {
                if (x.Adresa.ToString() == adresa)
                {
                    profilFitnesCentri.Add(x);
                }
            }

            return RedirectToAction("Profil");
        }
        public ActionResult NadjiGodinu(int from, int to)
        {
            profilFitnesCentri.Clear();
            List<FitnesCentar> teretane = FitnesCentar.ReadFromJson();
            foreach (FitnesCentar x in teretane)
            {
                if (x.GodOtvaranja >= from && x.GodOtvaranja <= to)
                {
                    profilFitnesCentri.Add(x);
                }
            }

            return RedirectToAction("Profil");
        }
        public ActionResult KombinovanaPretragaTeretane(string naziv, string adresa, int from, int to)
        {
            profilFitnesCentri.Clear();
            List<FitnesCentar> listafc = FitnesCentar.ReadFromJson();
            foreach (FitnesCentar x in listafc)
            {
                if (x.Naziv == naziv && x.Adresa.ToString() == adresa && x.GodOtvaranja >= from && x.GodOtvaranja <= to)
                {
                    profilFitnesCentri.Add(x);
                }
            }
            return RedirectToAction("Profil");
        }
        public ActionResult AddAdresa()
        {
            return View();
        }
        public ActionResult AddTrener()
        {
            return View();
        }
        public ActionResult AddTeretana()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddAdresa(AdresaFitnesCentra adr)
        {
            List<Korisnik> listaKorisnika = Korisnik.ReadFromJson();
            Korisnik logged = (Korisnik)Session["ulogovan"];
            if (adr.Ulica == "" || adr.Mesto == "" || adr.PostanskiBroj.ToString() == "" )
            {
                ViewBag.message = "Sva polja moraju biti popunjena";
                return View("AddTrener");
            }
            if (adr.Ulica == null || adr.Mesto == null )
            {
                ViewBag.message = "Sva polja moraju biti popunjena";
                return View("AddTrener");
            }
            staticAdr = adr;
            ViewBag.adresa = adr;
            return View("AddTeretana");
        }
        [HttpPost]
        public ActionResult AddTrener(Korisnik k)
        {
            List<Korisnik> listaKorisnika = Korisnik.ReadFromJson();
            Korisnik logged = (Korisnik)Session["ulogovan"];
            foreach (Korisnik x in listaKorisnika)
            {
                if (x.KorisnickoIme.Equals(k.KorisnickoIme))
                {
                    ViewBag.Message = $"Korisnik sa imenom > {k.KorisnickoIme} < vec postoji!";
                    return View("AddTrener");
                }
            }
            foreach (Korisnik x in listaKorisnika)
            {
                if (x.Email.Equals(k.Email))
                {
                    ViewBag.Message = $"Korisnik sa mejlon > {k.Email} < vec postoji!";
                    return View("AddTrener");
                }
            }
            if (k.KorisnickoIme == "" || k.Lozinka == "" || k.Ime == "" || k.Prezime == "" ||
               k.Pol.ToString() == "" || k.Email == "" || k.DatumRodjenja == "")
            {
                ViewBag.message = "Sva polja moraju biti popunjena";
                return View("AddTrener");
            }
            if (k.KorisnickoIme == null || k.Lozinka == null || k.Ime == null || k.Prezime == null ||
              k.Pol.ToString() == null || k.Email == null || k.DatumRodjenja == null)
            {
                ViewBag.message = "Sva polja moraju biti popunjena";
                return View("AddTrener");
            }

            if (k.Email.Contains(".com") == false || k.Email.Contains("@") == false)
            {
                ViewBag.message = "Email nije unet kako treba";
                ViewBag.message = "Primer: imenkoprezimic@gmail.com";
                return View("AddTrener");
            }
            k.Uloga = Uloga.Trener;
            List<FitnesCentar> listaFC = FitnesCentar.ReadFromJson();
            foreach (FitnesCentar fc in listaFC)
            {
                if (fc.Naziv == pomstr)
                {
                    k.CentarTrener = fc;
                }
            }
            listaKorisnika.Add(k);
            Korisnik.WriteToJson(listaKorisnika);           
            return RedirectToAction("Profil", "Vlasnik");
        }
        [HttpPost]
        public ActionResult AddTeretana(FitnesCentar fc)
        {
            fc.Adresa = staticAdr;
            List<FitnesCentar> sviFitnesCentri = FitnesCentar.ReadFromJson();
            List<Korisnik> korisnici = Korisnik.ReadFromJson();
            Korisnik logged = (Korisnik)Session["ulogovan"];
            foreach (FitnesCentar x in sviFitnesCentri)
            {
                if (x.Naziv.Equals(fc.Naziv))
                {
                    ViewBag.Message = $"Fitnes Centar sa imenom > {fc.Naziv} < vec postoji!";
                    return View("AddTeretana");
                }
            }
            
            if (fc.Naziv == "" || fc.Adresa.ToString() == "" || fc.GodOtvaranja.ToString() == "")
            {
                ViewBag.message = "Sva polja moraju biti popunjena";
                return View("AddTeretana");
            }
            if (fc.Naziv == null || fc.Adresa == null)
            {
                ViewBag.message = "Sva polja moraju biti popunjena";
                return View("AddTeretana");
            }
            if (logged.CentarVlasnik == null)
            {
                logged.CentarVlasnik = new List<FitnesCentar>();
            }
            int i = 0;
            int index = 0;
            sviFitnesCentri.Add(fc);
            FitnesCentar.WriteToJson(sviFitnesCentri);
            logged.CentarVlasnik.Add(fc);
            foreach(Korisnik kor in korisnici)
            {
                if(kor.KorisnickoIme == logged.KorisnickoIme)
                {
                    index = i;
                }
                i++;
            }
            korisnici[index] = logged;
            Korisnik.WriteToJson(korisnici);
            return RedirectToAction("Profil", "Vlasnik");
        }
        public ActionResult Obrisi(string naziv)
        {
            List<GrupniTrening> sviGrupni = GrupniTrening.ReadFromJson();
            List<FitnesCentar> sviFc = FitnesCentar.ReadFromJson();
            List<Korisnik> sviKorisnici = Korisnik.ReadFromJson();
            Korisnik k = (Korisnik)Session["ulogovan"];

            foreach (GrupniTrening x in sviGrupni)
            {
                if (x.FitnesCentar == naziv)
                {
                    if (DateTime.Compare(DateTime.Now, DateTime.Parse(x.DatumTreninga)) < 0)
                    {
                        ViewBag.message = "Postoje znakazani treninzi, ne moze se obrisati!";
                        return RedirectToAction("Profil", "Vlasnik");
                    }
                    sviGrupni.Remove(x);
                    GrupniTrening.WriteToJson(sviGrupni);
                    break;
                }
            }
            int i = 0;
            int index = 0;
            foreach (FitnesCentar x in k.CentarVlasnik)
            {
                if (x.Naziv == naziv)               
                {
                    k.CentarVlasnik.Remove(x);
                    foreach (Korisnik kor in sviKorisnici)
                    {
                        if (kor.KorisnickoIme == k.KorisnickoIme)
                        {
                            index = i;
                        }
                        i++;
                    }
                    sviKorisnici[index] = k;
                    Korisnik.WriteToJson(sviKorisnici);
                    break;
                }
            }

            foreach (FitnesCentar x in sviFc)
            {
                if (x.Naziv == naziv)               
                {
                    sviFc.Remove(x);
                    FitnesCentar.WriteToJson(sviFc);
                    break;
                }
            }

            return RedirectToAction("Profil");
        }
        public static string pomstr;
        public ActionResult Pregled(string naziv)
        {
            pomstr = naziv;
            foreach (FitnesCentar fc in profilFitnesCentri)
            {
                if (fc.Naziv == naziv)
                {
                    ViewBag.teretana = fc;
                    return View();
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult Modifikuj(FitnesCentar updated)
        {
            List<FitnesCentar> listaTeretana = FitnesCentar.ReadFromJson();
            List<Korisnik> listaKorisnika = Korisnik.ReadFromJson();
            List<GrupniTrening> gtLista = GrupniTrening.ReadFromJson();
            Korisnik logged = (Korisnik)Session["ulogovan"];
            int index = 0;
            int i = 0;

            if (updated.Naziv == "" || updated.GodOtvaranja.ToString() == "" || updated.CenaGClanaraine.ToString() == "" ||
                updated.CenaGrupnogTreninga.ToString() == "" || updated.CenaMClanarine.ToString() == "" || updated.CenaSaPersonalnimTrenerom.ToString() == "" || updated.CenaTreninga.ToString() == "")
            {
                ViewBag.message = "Sva polja moraju biti popunjena";
                return View("EditProfil");
            }          

            foreach (FitnesCentar fc in listaTeretana)
            {
                if (fc.Naziv.Equals(pomstr))
                {
                    index = i;
                }

                i++;
            }
            foreach(Korisnik kor in listaKorisnika)
            {
                if(kor.KorisnickoIme == logged.KorisnickoIme)
                {
                    logged = kor;
                }
            }
            i = 0;
            listaTeretana[index] = updated;
            foreach (FitnesCentar fc in logged.CentarVlasnik)
            {
                if (fc.Naziv.Equals(pomstr))
                {
                    index = i;
                }

                i++;
            }
            logged.CentarVlasnik[index] = updated;
            index = 0;
            i = 0;
            foreach (FitnesCentar fc in listaTeretana)
            {
                if (fc.Naziv.Equals(pomstr))
                {
                    index = i;
                }

                i++;
            }
            listaTeretana[index] = updated;

            index = 0;
            i = 0;
            foreach (Korisnik kor in listaKorisnika)
            {
                if (kor.KorisnickoIme == logged.KorisnickoIme)
                {
                    index = i;
                }

                i++;
            }
            listaKorisnika[index] = logged;

            index = 0;
            i = 0;
            foreach (GrupniTrening gt in gtLista)
            {
                if (gt.FitnesCentar == updated.Naziv)
                {
                    index = i;
                }

                i++;
            }
            gtLista[index].Naziv = updated.Naziv;
            GrupniTrening.WriteToJson(gtLista);
            FitnesCentar.WriteToJson(listaTeretana);
            Korisnik.WriteToJson(listaKorisnika);
            return RedirectToAction("Profil", "Vlasnik");
        }
    }
        
    
}