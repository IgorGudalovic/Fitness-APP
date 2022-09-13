using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitnesAPP.Models
{
    public class AdresaFitnesCentra
    {
        public AdresaFitnesCentra()
        {
        }

        public AdresaFitnesCentra(string ulica, string mesto, int postanskiBroj)
        {
            Ulica = ulica;
            Mesto = mesto;
            PostanskiBroj = postanskiBroj;
        }

        public string Ulica { get; set; }
        public string Mesto { get; set; }
        public int PostanskiBroj { get; set; }

        public override string ToString()
        {
            return $"{Ulica} {PostanskiBroj}, {Mesto}";
        }
    }
}