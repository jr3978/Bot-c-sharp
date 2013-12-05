using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moteur_de_Recherche;

namespace robot
{
    class Bot
    {
        private HtmlDataBase m_dataBase;
        private List<Html> m_stat;
        private Random m_random;
        private int m_errorIndex;

        public bool CurrentLinkOnConsole { get; set; }
        public bool LinksOnConsole { get; set; }
        public bool H1OnConsole { get; set; }
        public bool H2OnConsole { get; set; }
        public bool MetaOnConsole { get; set; }
        public bool TitleOnConsole { get; set; }
        public bool ErrorOnConsole { get; set; }


        public Bot(params string[] startUrl)
        {
            m_dataBase = new HtmlDataBase();
            m_random = new Random();
            m_stat = new List<Html>();

            if (m_dataBase.CountUnvisitedSite() > 0)
                m_dataBase.DropUnvisitedSiteInDataBase();
            else
            {
                for (int i = 0; i < startUrl.Count(); i++)
                {
                    WebLink.LinkToVisit.Add(startUrl[i]);
                }
            }
        }



        public void Search()
        {
            for (int i = 0; i < WebLink.LinkToVisit.Count; i++)
            {
                int r = m_random.Next(0, WebLink.LinkToVisit.Count);
                Html webSite = new Html(WebLink.LinkToVisit[r]);

                if (CurrentLinkOnConsole)
                    ConsoleCurrentLink(ref webSite);

                m_errorIndex = WebLink.LinkCauseError.Count;


                if (!m_dataBase.IsLinkAlreadyVisited(webSite.Link))
                     webSite.ReadHtml();


                #region WriteInConsole


                if (LinksOnConsole)
                    ConsoleLinks(ref webSite);
              
                if (H1OnConsole)
                    ConsoleH1(ref webSite);
              
                if (H2OnConsole)
                    ConsoleH2(ref webSite);

                if (MetaOnConsole)
                    ConsoleMeta(ref webSite);

                if (TitleOnConsole)
                    ConsoleTitle(ref webSite);
               
                if (m_errorIndex != WebLink.LinkCauseError.Count && ErrorOnConsole)
                    ConsoleError();
                #endregion

                m_stat.Add(webSite);



                WebLink.LinkVisited.Add(WebLink.LinkToVisit[r]);
                WebLink.LinkToVisit.RemoveAt(r);
                WebLink.CompteurLinkToVisit--;

                if (m_stat.Count >= 500)
                {
                    m_dataBase.DropHtmlObjectInDataBase(ref m_stat);
                    m_dataBase.DropErrorInDataBase();
                    m_dataBase.DropUnvisitedSiteInDataBase();
                    WebLink.LinkVisited.Clear();
                }

            }
        }



        private void ConsoleCurrentLink(ref Html webSite)
        {
            //------------Test----------------------
            Console.WriteLine("#########");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(webSite.Link + "    " + m_stat.Count.ToString());
            Console.ResetColor();
            Console.WriteLine("--------------------------------");
            //--------------------------------------
        }



        private void ConsoleLinks(ref Html webSite)
        {
            for (int i = 0; i < webSite.ListLink.Count; i++)
            {
                Console.WriteLine(webSite.ListLink[i]);
            }
        }



        private void ConsoleH1(ref Html webSite)
        {
            for (int i = 0; i < webSite.ListH1.Count; i++)
            {
                Console.WriteLine(webSite.ListH1[i]);
            }
        }



        private void ConsoleH2(ref Html webSite)
        {
            for (int i = 0; i < webSite.ListH2.Count; i++)
            {
                Console.WriteLine(webSite.ListH2[i]);
            }
        }



        private void ConsoleMeta(ref Html webSite)
        {
            for (int i = 0; i < webSite.ListMeta.Count; i++)
            {
                Console.WriteLine(webSite.ListMeta[i]);
            }
        }



        private void ConsoleTitle(ref Html webSite)
        {
            Console.WriteLine(webSite.Title);
        }



        private void ConsoleError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(WebLink.LinkCauseError.Last().ErrorMessage);
            Console.ResetColor();
        }

    }
}
