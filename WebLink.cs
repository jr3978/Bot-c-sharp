using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moteur_de_Recherche
{
    public static class WebLink
    {
        public static List<string> LinkToVisit { get; set; }
        public static List<string> LinkVisited { get; set; }
        public static List<Error> LinkCauseError { get; set; }
        public static int CompteurLinkToVisit { get; set; }
        public static int LastPositionLinkToVisit { get; set; }

        static WebLink()
        {
            LinkToVisit = new List<string>();
            LinkVisited = new List<string>();
            LinkCauseError = new List<Error>();
        }
    }
}
