using FitnesAPP.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitnesAPP.Controllers
{
    public class HomeController : Controller
    {
        private static List<FitnesCentar> listaTeretana = new List<FitnesCentar>();
        public ActionResult Index()
        {
            Korisnik k = (Korisnik)Session["ulogovan"];
            if (k != null)
            {
                ViewBag.ulogovan = k;
            }
            else ViewBag.ulogovan = null;
            if (listaTeretana.Count == 0)
            {
                List<FitnesCentar> teretane = FitnesCentar.ReadFromJson();
                ViewBag.teretane = teretane;
                return View();
            }
            else
            {
                ViewBag.teretane = listaTeretana;
                return View();
            }
        }
        public ActionResult Refresh()
        {
            listaTeretana.Clear();
            return RedirectToAction("Index", "Home");
        }
        

        public ActionResult Detalji(string fc)
        {
            Korisnik k = (Korisnik)Session["ulogovan"];
            FitnesCentar teretana = null;
            List<FitnesCentar> teretane = FitnesCentar.ReadFromJson();
            List<Korisnik> sviKorisnici = Korisnik.ReadFromJson();
            List<GrupniTrening> grupniTreninzi = GrupniTrening.ReadFromJson();
            List<GrupniTrening> spisakTreninga = new List<GrupniTrening>();

            foreach (GrupniTrening x in grupniTreninzi)
            {
                if (x.FitnesCentar != null)
                {
                    if (x.FitnesCentar.Equals(fc))
                    {
                        spisakTreninga.Add(x);
                    }
                }                
            }

            foreach (FitnesCentar x in teretane)
            {
                if (x.Naziv == fc)
                {
                    teretana = x;
                    break;
                }
            }
            ViewBag.teretana = teretana;
            ViewBag.grupniTreninzi = spisakTreninga;           
            if (k == null)
            {
                ViewBag.korisnik = null;
                return View();
            }
            ViewBag.korisnik = k;
            return View();
        }

        public ActionResult SortBy(string opcija)
        {
            if(opcija.Equals("Sortiraj po imenu rastuce"))
            {
                List<FitnesCentar> teretane = FitnesCentar.ReadFromJson();
                listaTeretana = teretane.OrderBy(x => x.Naziv).ToList();
                FitnesCentar.WriteToJson(listaTeretana);
                return RedirectToAction("Index");
            }
            if (opcija.Equals("Sortiraj po imenu opadajuce"))
            {
                List<FitnesCentar> teretane = FitnesCentar.ReadFromJson();
                listaTeretana = teretane.OrderBy(x => x.Naziv).Reverse().ToList();
                FitnesCentar.WriteToJson(listaTeretana);
                return RedirectToAction("Index");
            }
            if (opcija.Equals("Sortiraj po adresi rastuce"))
            {
                List<FitnesCentar> teretane = FitnesCentar.ReadFromJson();
                listaTeretana = teretane.OrderBy(x => x.Adresa.Ulica).ToList();
                FitnesCentar.WriteToJson(listaTeretana);
                return RedirectToAction("Index");
            }
            if ( opcija.Equals("Sortiraj po adresi opadajuce"))
            {
                List<FitnesCentar> teretane = FitnesCentar.ReadFromJson();
                listaTeretana = teretane.OrderBy(x => x.Adresa.Ulica).Reverse().ToList();
                FitnesCentar.WriteToJson(listaTeretana);
                return RedirectToAction("Index");
            }
            if (opcija.Equals("Sortiraj po godini rastuce"))
            {
                List<FitnesCentar> teretane = FitnesCentar.ReadFromJson();
                listaTeretana = teretane.OrderBy(x => x.GodOtvaranja).ToList();
                FitnesCentar.WriteToJson(listaTeretana);
                return RedirectToAction("Index");
            }
            if (opcija.Equals("Sortiraj po godini opadajuce"))
            {
                List<FitnesCentar> teretane = FitnesCentar.ReadFromJson();
                listaTeretana = teretane.OrderBy(x => x.GodOtvaranja).Reverse().ToList();
                FitnesCentar.WriteToJson(listaTeretana);
                return RedirectToAction("Index");
            }
            else return RedirectToAction("Index");
        }

        public ActionResult NadjiIme(string Naziv)
        {
            listaTeretana.Clear();
            List<FitnesCentar> teretane = FitnesCentar.ReadFromJson();
            foreach (FitnesCentar x in teretane)
            {
                if (x.Naziv == Naziv)
                {
                    listaTeretana.Add(x);
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult NadjiAdresu(AdresaFitnesCentra a)
        {
            listaTeretana.Clear();
            List<FitnesCentar> teretane = FitnesCentar.ReadFromJson();
            foreach (FitnesCentar x in teretane)
            {
                if (x.Adresa.Ulica.Equals(a.Ulica) && x.Adresa.PostanskiBroj.Equals(a.PostanskiBroj) && x.Adresa.Mesto.Equals(a.Mesto))
                {
                    listaTeretana.Add(x);
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult NadjiGodinu(int from, int to)
        {
            listaTeretana.Clear();
            List<FitnesCentar> teretane = FitnesCentar.ReadFromJson();
            foreach (FitnesCentar x in teretane)
            {
                if (x.GodOtvaranja >= from && x.GodOtvaranja <= to)
                {
                    listaTeretana.Add(x);
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult KombinovanaPretraga(string Naziv, string Ulica, string Broj, string Grad, int from, int to)
        {
            listaTeretana.Clear();
            List<FitnesCentar> teretane = FitnesCentar.ReadFromJson();
            foreach (FitnesCentar x in teretane)
            {
                if (x.Naziv.Equals(Naziv) && x.Adresa.Ulica.Equals(Ulica) &&
                    x.Adresa.PostanskiBroj.Equals(Broj) && x.Adresa.Mesto.Equals(Grad) &&
                    x.GodOtvaranja >= from && x.GodOtvaranja <= to)
                {
                    listaTeretana.Add(x);
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult PrijaviSeZaGrupniTrening(string naziv)
        {
            List<GrupniTrening> grupniTreninzi = GrupniTrening.ReadFromJson();
            List<Korisnik> korisnici = Korisnik.ReadFromJson();
            GrupniTrening gt = new GrupniTrening();
            Korisnik user = new Korisnik();
            Korisnik ulogovan = (Korisnik)Session["ulogovan"];
            ViewBag.ulogovan = ulogovan;

            //ako nije ulogovan
            if (ulogovan == null)
            {
                ViewBag.message = "Morate se ulogovati da bi prijavili trening";
                return RedirectToAction("Login", "User");
            }

            //nadji selektovan grupni trening
            foreach (GrupniTrening x in grupniTreninzi) 
            {
                if (x.Naziv.Equals(naziv))
                {
                    gt = x;
                    break;
                }
            }
            foreach (Korisnik x in korisnici)
            {
                if (x.KorisnickoIme.Equals(ulogovan.KorisnickoIme))
                {
                    //ako je ulogovan kao POSETILAC
                    if (x.Uloga.ToString().Equals(Uloga.Posetilac.ToString()))
                    {
                        if (ulogovan.PosetilacTreninzi != null)
                        {
                            if (ulogovan.PosetilacTreninzi.Contains(gt))
                            {
                                ViewBag.message = "Ne moze se dva puta prijaviti na jedan trening";
                                return RedirectToAction("Detalji", "User");
                            }
                        }
                        
                        if (gt.ListaPosetilaca.Count < gt.MaxBrojPosetilaca)
                        {
                            if(x.PosetilacTreninzi==null)
                            {
                                x.PosetilacTreninzi = new List<GrupniTrening>();
                                x.PosetilacTreninzi.Add(gt);
                                Edit(x);
                                ViewBag.korisnik = x;
                                return RedirectToAction("Profil", "User");
                            }
                            else
                            {
                                foreach (GrupniTrening trening in x.PosetilacTreninzi)
                                {
                                    if (gt.Naziv != trening.Naziv)              //ako vec nije prijavljen na trening prijavi ga
                                    {
                                        x.PosetilacTreninzi.Add(gt);
                                        return RedirectToAction("Profil", "User");
                                    }
                                    else
                                    {
                                        ViewBag.message = "Korisnik je vec prijavljen na trening";
                                        return RedirectToAction("Detalji", "User");
                                    }
                                }
                            }
                            
                        }
                        else
                        {
                            ViewBag.message = "Sva mesta su popunjena";
                            return RedirectToAction("Detalji", "User");
                        }
                    }

                    //ako je ulogovan TRENER
                    if (x.Uloga.ToString().Equals(Uloga.Trener.ToString())) //ako korisnik ima ulogu trener
                    {
                        return RedirectToAction("Index", "Trener");
                    }

                    //ako je ulogovan VLASNIK
                    if (x.Uloga.ToString().Equals(Uloga.Vlasnik.ToString())) //ako korisnik ima ulogu vlasnik
                    {
                        return RedirectToAction("Index", "Vlasnik");
                    }
                }
                   
            }
            return View("Detalji");
        }

        public void Edit(Korisnik novi)
        {
            bool nasao = false;
            List<Korisnik> listaKorisnika = Korisnik.ReadFromJson();
            Korisnik stari = (Korisnik)Session["ulogovan"];
            int index = 0;
            int foundIndex = 0;

            foreach (Korisnik x in listaKorisnika)
            {
                if (x.KorisnickoIme.Equals(stari.KorisnickoIme) && !nasao)
                {
                    stari = x;
                    nasao = true;
                    foundIndex = index;
                }
                
                index++;
            }
            stari.PosetilacTreninzi = novi.PosetilacTreninzi;
           
            listaKorisnika[foundIndex] = novi;
            Korisnik.WriteToJson(listaKorisnika);
            Session["ulogovan"] = novi;           
        }
    }
}