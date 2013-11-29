using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Moteur_de_Recherche
{
    public class Html
    {
        private string m_link;
        private string m_title;
        private int m_linkCounter = 0;
        private string[] m_metaName = { "description", "author", "copyright" };

        private List<string> m_listH1;
        private List<string> m_listH2;
        private List<string> m_listLink;
        private List<MetaTag> m_listMeta;

        private int tagCounter = 0;
        private int lastPositionTag = 0;


        //Reachable properties
        public string Link { get { return m_link; } }
        public string Title { get { return m_title; } }
        public int LinkCounter { get { return m_linkCounter; } }

        public List<string> ListH1 { get { return m_listH1; } set { m_listH1 = value; } }
        public List<string> ListH2 { get { return m_listH2; } set { m_listH2 = value; } }
        public List<string> ListLink { get { return m_listLink; } set { m_listLink = value; } }
        public List<MetaTag> ListMeta { get { return m_listMeta; } set { m_listMeta = value; } }


        public Html(string url)
        {
            m_link = url;

            m_listLink = new List<string>();
            m_listH1 = new List<string>();
            m_listH2 = new List<string>();
            m_listMeta = new List<MetaTag>();
        }


        public void ReadHtml()
        {
            WebClient webPage = new WebClient();
            String htmlCode = "";
            try
            {
                htmlCode = webPage.DownloadString(m_link);
            }
            catch (Exception ex)
            {
                Error error = new Error();
                error.ErrorMessage = ex.Message;
                error.Link = m_link;

                WebLink.LinkCauseError.Add(error);
                WebLink.LinkToVisit.Remove(m_link);

                #region TestLinkCauseError
                //-----------Test Affiche erreur--------
                Console.WriteLine(ex.Message);
                #endregion
            }

            GetAllLink(htmlCode);
            GetTagInnerText(htmlCode, "<h1", "</h1>", ref this.m_listH1);
            GetTagInnerText(htmlCode, "<h2", "</h2>", ref this.m_listH2);
            m_title = GetHtmlTitle(ref htmlCode);
            GetMetaContent(ref htmlCode, m_metaName);

        }


        private void GetAllLink(string htmlCode)
        {
            int index = 0;

            while (htmlCode.IndexOf("<a href=\"http", index) > 0)
            {
                string lien = "";
                int s = htmlCode.IndexOf("<a href=\"http", index + 1);
                int e = htmlCode.IndexOf("\">", s + 1);

                for (int i = s + 9; i < e; i++)
                {
                    lien += htmlCode.ElementAt(i);
                }
                index = e;

                if (!WebLink.LinkToVisit.Contains(lien) && !WebLink.LinkVisited.Contains(lien))
                {

                    if (!CheckTopDomain(lien, ".com"))
                        if (!CheckTopDomain(lien, ".ca"))
                            if (!CheckTopDomain(lien, ".fr"))
                                if (!CheckTopDomain(lien, ".tv"))
                                    if (!CheckTopDomain(lien, ".gov"))
                                        if (!CheckTopDomain(lien, ".org"))
                                            CheckTopDomain(lien, ".net");
                }

            }

            #region TestLinkInWebSite
            //-------------------------Test Affiche les liens sur le site actuel-----------------------
            Console.WriteLine("-----------------------------------------------------");

            for (int i = WebLink.LastPositionLinkToVisit; i < WebLink.LinkToVisit.Count; i++)
            {
                Console.WriteLine(WebLink.LinkToVisit[i]);
            }
            WebLink.LastPositionLinkToVisit = WebLink.CompteurLinkToVisit;
            //------------------------------------------------------------------------------------------
            #endregion
        }


        private bool CheckTopDomain(string lien, string topDomain)
        {
            int index = lien.IndexOf(topDomain);

            if (index > 0)
            {
                lien = lien.Substring(0, index + topDomain.Length);
                if (!WebLink.LinkToVisit.Contains(lien))
                {
                    m_listLink.Add(lien);
                    m_linkCounter++;

                    WebLink.LinkToVisit.Add(lien);
                    WebLink.CompteurLinkToVisit++;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Methode pour type de balises ayant une balise fermante seulement
        /// </summary>
        private void GetTagInnerText(string htmlCode, string openTag, string closeTag, ref List<string> list)
        {
            string innerText;
            int index = 0;
            while (htmlCode.IndexOf(openTag, index) > 0)
            {
                // balise ouvrante
                int s = htmlCode.IndexOf(openTag, index + 1);

                if (s < 0)
                    break;
                // fin de la balise ouvrante (a part si elle comporte un class ou un id)
                int sE = htmlCode.IndexOf(">", s);
                sE += 1;


                //debut balise fermante
                int e = htmlCode.IndexOf(closeTag, s + 1);

                innerText = InnerTextTagManagement(ref sE, ref e, ref htmlCode);
                if (innerText.Trim() != "")
                    list.Add(innerText);

                //--------------------test-------------------------------------------
                //string test ="  <a href=\"http://yuilibrary.com/yuiconf/2013/\" title=\"Join us at YUIConf 2013\"> Join us at YUIConf 2013  </a> ";
                //int sta = 0, end = test.Length;

                //InnerTextTagManagement(ref sta, ref end, ref test);
                //--------------------------------------------------------------------

                ////debut balise inside balise <h1>
                //int sA = htmlCode.IndexOf("<", sE + 1, e - sE - 1);
                //int eA = 0;

                //// Si H1 contient pas de balise <a>
                //if (sA < 0)
                //    for (int i = sE + 1; i < e; i++)
                //        innerText += htmlCode.ElementAt(i);
                //// H1 contient une balise <a>
                //else
                //{
                //    for (int i = sA; i < e; i++)
                //        if (htmlCode.ElementAt(i) == '>')
                //        {
                //            eA = i;
                //            break;
                //        }
                //    for (int i = eA + 1; htmlCode.ElementAt(i) != '<'; i++)
                //        innerText += htmlCode.ElementAt(i);
                //}


                index = e;

                if (index < 0)
                    return;
                tagCounter++;
            }


            #region TestTagFromCurrentWebSite
            //--------------------------Test Affiche les H1 du site actuel------------------------
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine(list[i]);
            }
            lastPositionTag = tagCounter;
            //--------------------------------------------------------------------------------------
            #endregion
        }

        private string InnerTextTagManagement(ref int startPosition, ref int endPosition, ref string htmlCode)
        {
            bool innerTag = false;
            string innerText = "";

            for (int i = startPosition; i < endPosition; i++)
            {

                if (htmlCode.ElementAt(i) == '<')
                    innerTag = true;
                else if (htmlCode.ElementAt(i) == '>')
                    innerTag = false;
                else if (!innerTag)
                    innerText += htmlCode.ElementAt(i);
            }

            return innerText.TrimStart().TrimEnd();
        }

        private string GetHtmlTitle(ref string htmlCode)
        {
            m_title = "";

            int sT = htmlCode.IndexOf("<title");
            if (sT < 0)
                return m_title;


            int eT = htmlCode.IndexOf(">", sT);
            eT += 1;

            int sClosetTag = htmlCode.IndexOf("</title>", eT);

            for (int i = eT; i < sClosetTag; i++)
            {
                m_title += htmlCode.ElementAt(i);
            }

            #region TestTitle
            //--------------------Test Affichage titre----------------
            Console.WriteLine("TITLE: " + m_title);
            #endregion

            return m_title;

        }

        private void GetMetaContent(ref string htmlCode, string[] namesPropertiesTag)
        {
            
            int index = 0;
            string name = "";

            while (htmlCode.IndexOf("<meta", index) > 0)
            {
                int sM = htmlCode.IndexOf("<meta", index);
                int eM = htmlCode.IndexOf(">", sM);
                if (eM < 0)
                    return;
                else
                    index = eM;

                if (sM < 0)
                    return;

                //int nameProperties = htmlCode.IndexOf(namesPropertiesTag, sM, eM - sM);
                int sName = 0;

                for (int i = 0; i < namesPropertiesTag.Length; i++)
                {
                    sName = htmlCode.IndexOf(namesPropertiesTag[i], sM, eM - sM);
                    if (sName > 0)
                    {
                        name = namesPropertiesTag[i];
                        break;
                    }
                }
                    
                
                
                int content = 0;

                if (sName > 0)
                    content = htmlCode.IndexOf("content=", sName, eM - sName);


                if (content > 0)
                {
                    string contentText = "";
                    bool isContentText = false;
                    
                    for (int i = content; i < eM; i++)
                    {
                        if (htmlCode.ElementAt(i) == '\"')
                            isContentText = true;
                        else if (htmlCode.ElementAt(i) == '\"')
                            isContentText = false;
                        else if (isContentText)
                            contentText += htmlCode.ElementAt(i);
                    }

                    m_listMeta.Add(new MetaTag(name, contentText));
                }
            }



        }
    }
}
