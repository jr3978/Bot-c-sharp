using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace Moteur_de_Recherche
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            
            InitializeComponent();
            
            List<Html> stat = new List<Html>();

            #region StartUrl
            WebLink.LinkToVisit.Add("http://vortex.infosth.com");
            WebLink.LinkToVisit.Add("http://tva.ca");
            #endregion

            Random random = new Random();

            for (int i = 0; i < WebLink.LinkToVisit.Count; i++)
            {
                int r = random.Next(0, WebLink.LinkToVisit.Count);

                Html webSite = new Html(WebLink.LinkToVisit[r]);
               
                
                #region TestCurrentWebSite
                //------------Test----------------------
                Console.WriteLine("#########");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(WebLink.LinkToVisit[r]);
                Console.ResetColor();
                //--------------------------------------
                #endregion
               
                
                webSite.ReadHtml();
                stat.Add(webSite);

                WebLink.LinkVisited.Add(WebLink.LinkToVisit[r]);
                WebLink.LinkToVisit.RemoveAt(r);
                WebLink.CompteurLinkToVisit--;
            }
        }
    }
}
