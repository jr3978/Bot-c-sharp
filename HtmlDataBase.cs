using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moteur_de_Recherche;

namespace robot
{
    public class HtmlDataBase
    {

        private TableLink m_tableLink;
        private TableEmail m_tableEmail;
        private TableH1 m_tableH1;
        private TableH2 m_tableH2;
        private TableUnvisited m_tableUnvisited;
        private TableMeta m_tableMeta;
        private TableError m_tableError;
        private WebStatDataContext m_dataBase;




        public HtmlDataBase()
        {
            m_dataBase = new WebStatDataContext();
        }


        public void DropHtmlObjectInDataBase(ref List<Html> htmlObjects)
        {
            int total = htmlObjects.Count;

            for (int c = 0; c < total; c++)
            {
                //----TableLink----
                m_tableLink = new TableLink { Link = htmlObjects[0].Link, Title = htmlObjects[0].Title, VisiteDate = htmlObjects[0].VisiteDate };
                m_dataBase.TableLink.InsertOnSubmit(m_tableLink);
                m_dataBase.SubmitChanges();

                // breakpoint
                string link = htmlObjects[0].Link;
                var req = m_dataBase.TableLink.Where(l => l.Link == link).Take(1);
                int linkId = req.First().Id;

                //----TableH1----
                for (int i = 0; i < htmlObjects[0].ListH1.Count; i++)
                {
                    m_tableH1 = new TableH1 { IdLink = linkId, InnerText = htmlObjects[0].ListH1[i] };
                    m_dataBase.TableH1.InsertOnSubmit(m_tableH1);
                }

                //----TableH2----
                for (int i = 0; i < htmlObjects[0].ListH2.Count; i++)
                {
                    m_tableH2 = new TableH2 { IdLink = linkId, InnerText = htmlObjects[0].ListH2[i] };
                    m_dataBase.TableH2.InsertOnSubmit(m_tableH2);
                }

                //----TableMeta----
                for (int i = 0; i < htmlObjects[0].ListMeta.Count; i++)
                {
                    m_tableMeta = new TableMeta { IdLink = linkId, InnerText = htmlObjects[0].ListMeta[i].Content, Name = htmlObjects[0].ListMeta[i].Name };
                    m_dataBase.TableMeta.InsertOnSubmit(m_tableMeta);
                }

                //----TableEmail----
                for (int i = 0; i < htmlObjects[0].Email.Count; i++)
                {
                    m_tableEmail = new TableEmail { IdLink = linkId, Address = htmlObjects[0].Email[i] };
                    m_dataBase.TableEmail.InsertOnSubmit(m_tableEmail);
                }

                htmlObjects.RemoveAt(0);
            }

            WebLink.LinkVisited.Clear();
            htmlObjects.Clear();
            m_dataBase.SubmitChanges();
        }


        public void DropErrorInDataBase()
        {
            int total = WebLink.LinkCauseError.Count;

            for (int i = 0; i < total; i++)
            {
                //----TableError----
                m_tableError = new TableError { Link = WebLink.LinkCauseError[0].Link, Date = WebLink.LinkCauseError[0].VisitedDate, MessageError = WebLink.LinkCauseError[0].ErrorMessage };

                m_dataBase.TableError.InsertOnSubmit(m_tableError);
                m_dataBase.SubmitChanges();

                WebLink.LinkCauseError.RemoveAt(0);
            }

            WebLink.LinkCauseError.Clear();
        }


        /// <summary>
        /// Vide la liste de site a visiter et la rempli avec 300 autres sites
        /// </summary>\
        public void DropUnvisitedSiteInDataBase()
        {
            //----TableUnvisited----
            int total = WebLink.LinkToVisit.Count;

            for (int i = 0; i < total; i++)
            {
                m_tableUnvisited = new TableUnvisited { Link = WebLink.LinkToVisit[0] };
                m_dataBase.TableUnvisited.InsertOnSubmit(m_tableUnvisited);
                WebLink.LinkToVisit.RemoveAt(0);
            }

            WebLink.LinkToVisit.Clear();

            m_dataBase.SubmitChanges();

            for (int i = 0; i < 300; i++)
            {
                if (m_dataBase.TableUnvisited.Count() == 0)
                    break;

                WebLink.LinkToVisit.Add(m_dataBase.TableUnvisited.First().Link);
                m_dataBase.TableUnvisited.DeleteOnSubmit(m_dataBase.TableUnvisited.First());
                m_dataBase.SubmitChanges();
            }

            
        }


        public bool IsLinkAlreadyVisited(string lien)
        {
            var req = m_dataBase.TableLink.Where(l => l.Link == lien);

            if (req.Count() > 0)
                return true;
            return false;
        }

        public int CountUnvisitedSite()
        {
            return m_dataBase.TableUnvisited.Count();
        }
    }
}
