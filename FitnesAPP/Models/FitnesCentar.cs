using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FitnesAPP.Models
{
    public class FitnesCentar
    {
        public FitnesCentar()
        {
        }

        public FitnesCentar(string naziv, AdresaFitnesCentra adresa, int godOtvaranja, Korisnik vlasnik, double cenaMClanarine, double cenaGClanaraine, double cenaTreninga, double cenaGrupnogTreninga, double cenaSaPersonalnimTrenerom)
        {
            Naziv = naziv;
            Adresa = adresa;
            GodOtvaranja = godOtvaranja;
            Vlasnik = vlasnik;
            CenaMClanarine = cenaMClanarine;//mesecna clanarina
            CenaGClanaraine = cenaGClanaraine;//godisnja clanarina
            CenaTreninga = cenaTreninga;
            CenaGrupnogTreninga = cenaGrupnogTreninga;
            CenaSaPersonalnimTrenerom = cenaSaPersonalnimTrenerom;
        }

        public string Naziv { get; set; }
        public AdresaFitnesCentra Adresa { get; set; }
        public int GodOtvaranja { get; set; }
        public Korisnik Vlasnik { get; set; }
        public double CenaMClanarine { get; set; }
        public double CenaGClanaraine { get; set; }
        public double CenaTreninga { get; set; }
        public double CenaGrupnogTreninga { get; set; }
        public double CenaSaPersonalnimTrenerom { get; set; }

        public static List<FitnesCentar> ReadFromJson()
        {
            List<FitnesCentar> teretane = new List<FitnesCentar>();//promeni path!!!!!!!!!!!!!!!!!!!!!!

            string jsonFromFile;
            using (var reader = new StreamReader("C:\\Users\\a\\Desktop\\FitnesAPP\\FitnesAPP\\TextFiles\\FitnesCentri.json"))
            {
                jsonFromFile = reader.ReadToEnd();
            }
            List<FitnesCentar> fitnesCentri = JsonConvert.DeserializeObject<List<FitnesCentar>>(jsonFromFile);
            foreach (var x in fitnesCentri)
            {
                teretane.Add(x);
            }
            return teretane;
        }

        public static void WriteToJson(List<FitnesCentar> teretane)//promeni path!!!!!!!!!!!!!!!!!!!!!!
        {
            var jsonToWrite = JsonConvert.SerializeObject(teretane);
            using (var writer = new StreamWriter("C:\\Users\\a\\Desktop\\FitnesAPP\\FitnesAPP\\TextFiles\\FitnesCentri.json"))
            {
                writer.Write(jsonToWrite);
            }
        }

        public override string ToString()
        {
            return $"{Naziv}";
        }
    }
}