using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moteur_de_Recherche;
using System.Diagnostics;

namespace robot
{
    class Bot
    {
        private bool m_aloneOnWork;
        private Stopwatch m_stopwatch;
        private Stopwatch m_stopwatchRobots;
        private RobotTable m_tbot;
        private WebStatDataContext m_dataBase;
        private HtmlDataBase m_htmlDataBase;
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
            m_aloneOnWork = true;
            m_stopwatchRobots = new Stopwatch();
            m_stopwatch = new Stopwatch();
            m_dataBase = new WebStatDataContext();
            m_tbot = new RobotTable { IsDroppingDataInTables = false, IsWorkingOnTableUnvisited = false, IsWorking = true, TotalLinkVisited = 0 };
            m_dataBase.RobotTable.InsertOnSubmit(m_tbot);
            m_dataBase.SubmitChanges();

            m_htmlDataBase = new HtmlDataBase();
            m_random = new Random();
            m_stat = new List<Html>();


            WaitForTableUnvisited();
            if (m_htmlDataBase.CountUnvisitedSite() > 0)
            {
                m_tbot.IsWorkingOnTableUnvisited = true;
                m_dataBase.SubmitChanges();

                m_htmlDataBase.DropUnvisitedSiteInDataBase();

                m_tbot.IsWorkingOnTableUnvisited = false;
                m_dataBase.SubmitChanges();
            }
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
            m_stopwatchRobots.Start();


            for (int i = 0; i < WebLink.LinkToVisit.Count; i++)
            {
                if (m_stopwatchRobots.ElapsedMilliseconds > 500000)
                {
                    m_stopwatchRobots.Stop();

                    if (m_dataBase.RobotTable.Count(r => r.IsWorking) != 0)
                        m_aloneOnWork = false;

                    m_stopwatchRobots.Reset();
                    m_stopwatchRobots.Start();
                }


                if (WebLink.LinkToVisit.Count == 0)
                {
                    WaitForTableUnvisited();

                    m_htmlDataBase.DropUnvisitedSiteInDataBase();
                }

                int ra = m_random.Next(0, WebLink.LinkToVisit.Count);
                Html webSite = new Html(WebLink.LinkToVisit[ra]);

                if (CurrentLinkOnConsole)
                    ConsoleCurrentLink(ref webSite);

                m_errorIndex = WebLink.LinkCauseError.Count;


                if (!m_htmlDataBase.IsLinkAlreadyVisited(webSite.Link))
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



                WebLink.LinkVisited.Add(WebLink.LinkToVisit[ra]);
                WebLink.LinkToVisit.RemoveAt(ra);
                WebLink.CompteurLinkToVisit--;

                if (m_stat.Count >= 500)
                {
                    if (!m_aloneOnWork)
                        WaitForDropData();

                    m_tbot.IsDroppingDataInTables = true;
                    m_tbot.TotalLinkVisited += 500;
                    m_dataBase.SubmitChanges();


                    m_htmlDataBase.DropHtmlObjectInDataBase(ref m_stat);
                    m_htmlDataBase.DropErrorInDataBase();
                    m_htmlDataBase.DropUnvisitedSiteInDataBase();
                    WebLink.LinkVisited.Clear();

                    m_tbot.IsDroppingDataInTables = false;
                    m_dataBase.SubmitChanges();
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


        private void WaitForTableUnvisited()
        {
            if (!m_aloneOnWork)
            {
                while (m_dataBase.RobotTable.Count(r => r.IsWorkingOnTableUnvisited) != 0)
                {
                    m_stopwatch.Reset();
                    m_stopwatch.Start();
                    while (m_stopwatch.IsRunning)
                    {
                        if (m_stopwatch.ElapsedMilliseconds > 10)
                            m_stopwatch.Stop();
                    }
                }
            }
        }

        private void WaitForDropData()
        {
            if (!m_aloneOnWork)
            {
                while (m_dataBase.RobotTable.Count(r => r.IsDroppingDataInTables) != 0)
                {
                    m_stopwatch.Reset();
                    m_stopwatch.Start();
                    while (m_stopwatch.IsRunning)
                    {
                        if (m_stopwatch.ElapsedMilliseconds > 10)
                            m_stopwatch.Stop();
                    }
                }
            }
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
