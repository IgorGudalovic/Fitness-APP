using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FitnesAPP.Models
{
    public enum Uloga
    {
        Posetilac, Trener, Vlasnik
    }
    public enum Pol
    {
        M, Z
    }
    public class Korisnik
    {
        public Korisnik() { }
        public Korisnik(string korisnickoIme, string lozinka, string ime, string prezime, Pol pol, string email, string datumRodjenja, Uloga uloga, List<GrupniTrening> trenerTreninzi, List<GrupniTrening> posetilacTreninzi, FitnesCentar centarTrener, List<FitnesCentar> centarVlasnik)
        {
            KorisnickoIme = korisnickoIme;
            Lozinka = lozinka;
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            Email = email;
            DatumRodjenja = datumRodjenja;
            Uloga = uloga;
            TrenerTreninzi = trenerTreninzi;       //grupni treninzi gde je korisnik trener
            PosetilacTreninzi = posetilacTreninzi; //grupni treninzi gde je korisnik posetilac
            CentarTrener = centarTrener;           //fitnes centar gde je korisnik angazovan
            CentarVlasnik = centarVlasnik;         //fitnes centar ciji je korisnik vlasnik
        }

        public string KorisnickoIme { get; set; }
        public string Lozinka { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public Pol Pol { get; set; }
        public string Email { get; set; }
        public string DatumRodjenja { get; set; }
        public Uloga Uloga { get; set; }
        public List<GrupniTrening> TrenerTreninzi { get; set; }
        public List<GrupniTrening> PosetilacTreninzi { get; set; }
        public FitnesCentar CentarTrener { get; set; }
        public List<FitnesCentar> CentarVlasnik { get; set; }

        public static List<Korisnik> ReadFromJson()
        {
            List<Korisnik> korisnici = new List<Korisnik>();//promeni path!!!!!!!!!!!!!!!!!!!!!!

            string jsonFromFile;
            using (var reader = new StreamReader("C:\\Users\\igorg\\Source\\Repos\\pr148-2019-web-projekat\\FitnesAPP\\TextFiles\\Korisnici.json"))
            {
                jsonFromFile = reader.ReadToEnd();
            }
            List<Korisnik> users = JsonConvert.DeserializeObject<List<Korisnik>>(jsonFromFile);
            foreach (var x in users)
            {
                korisnici.Add(x);
            }
            return korisnici;  
        }

        public static void WriteToJson(List<Korisnik> k)
        {
            string jsonToWrite = JsonConvert.SerializeObject(k);//promeni path!!!!!!!!!!!!!!!!!!!!!!
            using (var writer = new StreamWriter("C:\\Users\\a\\Desktop\\FitnesAPP\\FitnesAPP\\TextFiles\\Korisnici.json"))
            {
                writer.Write(jsonToWrite);
            }
        }
    }
}