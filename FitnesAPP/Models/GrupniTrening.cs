using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FitnesAPP.Models
{
    public class GrupniTrening
    {
        public GrupniTrening() { }
        public GrupniTrening(string naziv, string fitnesCentar, string tipTreninga, string trajanjeTreninga, string datumTreninga, string vremeTreninga, int maxBrojPosetilaca, List<Korisnik> listaPosetilaca)
        {
            Naziv = naziv;         
            TipTreninga = tipTreninga;
            FitnesCentar = fitnesCentar;
            TrajanjeTreninga = trajanjeTreninga;       //u minutima
            DatumTreninga = datumTreninga;             //format d/m/y h:m
            MaxBrojPosetilaca = maxBrojPosetilaca;
            ListaPosetilaca = listaPosetilaca;         //lista korisnika koji su se prijavili
        }

        public string Naziv { get; set; }
        public string TipTreninga { get; set; }
        public string FitnesCentar { get; set; }
        public string TrajanjeTreninga { get; set; }
        public string DatumTreninga { get; set; }
        public string VremeTreninga { get; set; }
        public int MaxBrojPosetilaca { get; set; }
        public List<Korisnik> ListaPosetilaca { get; set; }

        public static List<GrupniTrening> ReadFromJson()
        {
            List<GrupniTrening> grupniTreninzi = new List<GrupniTrening>();//promeni path!!!!!!!!!!!!!!!!!!!!!!
            string jsonFromFile;
            using (var reader = new StreamReader("C:\\Users\\igorg\\Source\\Repos\\pr148-2019-web-projekat\\FitnesAPP\\TextFiles\\GrupniTreninzi.json"))
            {
                jsonFromFile = reader.ReadToEnd();
            }

            List<GrupniTrening> treninzi = JsonConvert.DeserializeObject<List<GrupniTrening>>(jsonFromFile);
            foreach (var x in treninzi)
            {
                grupniTreninzi.Add(x);
            }
            return grupniTreninzi;    
        }

        public static void WriteToJson(List<GrupniTrening> gt)//promeni path!!!!!!!!!!!!!!!!!!!!!!
        {
            var jsonToWrite = JsonConvert.SerializeObject(gt);
            using (var writer = new StreamWriter("C:\\Users\\a\\Desktop\\FitnesAPP\\FitnesAPP\\TextFiles\\GrupniTreninzi.json"))
            {
                writer.Write(jsonToWrite);
            }
        }
    }
}