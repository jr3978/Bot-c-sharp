using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using robot;

namespace Moteur_de_Recherche
{
    public class Html
    {
        private string m_link;
        private string m_title;
        private int m_linkCounter = 0;
        private string[] m_metaName = { "description", "author", "copyright" };
        private string m_htmlCode;

        private List<string> m_listH1;
        private List<string> m_listH2;
        private List<string> m_listLink;
        private List<MetaTag> m_listMeta;
        private List<string> m_email;

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
        public List<string> Email { get { return m_email; } set { m_email = value; } }
        public DateTime VisiteDate { get; set; }


        public Html(string url)
        {
            m_link = url;

            m_listLink = new List<string>();
            m_listH1 = new List<string>();
            m_listH2 = new List<string>();
            m_listMeta = new List<MetaTag>();
            m_email = new List<string>();
            this.VisiteDate = DateTime.Now;
        }


        public void ReadHtml()
        {
            WebClient webPage = new WebClient();
            try
            {
                m_htmlCode = webPage.DownloadString(m_link);

                GetAllLink();
                GetTagInnerText("<h1", "</h1>", ref this.m_listH1);
                GetTagInnerText("<h2", "</h2>", ref this.m_listH2);
                m_title = GetHtmlTitle();
                GetMetaContent(m_metaName);
                GetAllEmailAddress();
            }
            catch (Exception ex)
            {
                Error error = new Error();
                error.ErrorMessage = ex.Message;
                error.Link = m_link;
                error.VisitedDate = DateTime.Now;
                
                WebLink.LinkCauseError.Add(error);
            }


        }


        private void GetAllLink()
        {
            if (m_htmlCode == null)
                return;

            int index = 0;

            while (m_htmlCode.IndexOf("<a href=\"http", index) > 0)
            {
                string lien = "";
                int s = m_htmlCode.IndexOf("<a href=\"http", index + 1);
                int e = m_htmlCode.IndexOf("\">", s + 1);

                for (int i = s + 9; i < e; i++)
                {
                    if (m_htmlCode.ElementAt(i) == '\"')
                        break;
                    lien += m_htmlCode.ElementAt(i);
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
            ////-------------------------Test Affiche les liens sur le site actuel-----------------------
            //Console.WriteLine("-----------------------------------------------------");

            //for (int i = WebLink.LastPositionLinkToVisit; i < WebLink.LinkToVisit.Count; i++)
            //{
            //    Console.WriteLine(WebLink.LinkToVisit[i]);
            //}
            //WebLink.LastPositionLinkToVisit = WebLink.CompteurLinkToVisit;
            ////------------------------------------------------------------------------------------------
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
        private void GetTagInnerText(string openTag, string closeTag, ref List<string> list)
        {
            string innerText;
            int index = 0;
            while (m_htmlCode.IndexOf(openTag, index) > 0)
            {
                // balise ouvrante
                int s = m_htmlCode.IndexOf(openTag, index + 1);

                if (s < 0)
                    break;
                // fin de la balise ouvrante (a part si elle comporte un class ou un id)
                int sE = m_htmlCode.IndexOf(">", s);
                sE += 1;


                //debut balise fermante
                int e = m_htmlCode.IndexOf(closeTag, s + 1);

                innerText = InnerTextTagManagement(ref sE, ref e);
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
            ////--------------------------Test Affiche les H1 du site actuel------------------------
            //for (int i = 0; i < list.Count; i++)
            //{
            //    Console.WriteLine(list[i]);
            //}
            //lastPositionTag = tagCounter;
            ////--------------------------------------------------------------------------------------
            #endregion
        }

        private string InnerTextTagManagement(ref int startPosition, ref int endPosition)
        {
            bool innerTag = false;
            string innerText = "";

            for (int i = startPosition; i < endPosition; i++)
            {

                if (m_htmlCode.ElementAt(i) == '<')
                    innerTag = true;
                else if (m_htmlCode.ElementAt(i) == '>')
                    innerTag = false;
                else if (!innerTag)
                    innerText += m_htmlCode.ElementAt(i);
            }

            return innerText.TrimStart().TrimEnd();
        }

        private string GetHtmlTitle()
        {
            m_title = "";

            int sT = m_htmlCode.IndexOf("<title");
            if (sT < 0)
                return m_title;


            int eT = m_htmlCode.IndexOf(">", sT);
            eT += 1;

            int sClosetTag = m_htmlCode.IndexOf("</title>", eT);

            for (int i = eT; i < sClosetTag; i++)
            {
                m_title += m_htmlCode.ElementAt(i);
            }

            #region TestTitle
            ////--------------------Test Affichage titre----------------
            //Console.WriteLine("TITLE: " + m_title);
            #endregion

            return m_title;

        }

        private void GetMetaContent(string[] namesPropertiesTag)
        {

            int index = 0;
            string name = "";

            while (m_htmlCode.IndexOf("<meta", index) > 0)
            {
                int sM = m_htmlCode.IndexOf("<meta", index);
                int eM = m_htmlCode.IndexOf(">", sM);
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
                    sName = m_htmlCode.IndexOf(namesPropertiesTag[i], sM, eM - sM);
                    if (sName > 0)
                    {
                        name = namesPropertiesTag[i];
                        break;
                    }
                }



                int content = 0;

                if (sName > 0)
                    content = m_htmlCode.IndexOf("content=", sName, eM - sName);


                if (content > 0)
                {
                    string contentText = "";
                    bool isContentText = false;

                    for (int i = content; i < eM; i++)
                    {
                        if (m_htmlCode.ElementAt(i) == '\"')
                            isContentText = true;
                        else if (m_htmlCode.ElementAt(i) == '\"')
                            isContentText = false;
                        else if (isContentText)
                            contentText += m_htmlCode.ElementAt(i);
                    }

                    m_listMeta.Add(new MetaTag(name, contentText));
                }
            }
        }

        private void GetAllEmailAddress()
        {
            int index = 0;

            while (m_htmlCode.IndexOf("href=\"mailto:", index) > 0)
            {
                string emailAddress = "";

                int sE = m_htmlCode.IndexOf("href=\"mailto:", index);

                if (sE < 0)
                    break;

                int eE = m_htmlCode.IndexOf("</a>", sE);

                bool onEmailAddress = false;
                short compteurGuillement = 0;

                for (int i = sE; i < eE; i++)
                {
                    if (m_htmlCode.ElementAt(i) == ':' && compteurGuillement == 1)
                        onEmailAddress = true;
                    else if (m_htmlCode.ElementAt(i) == '\"')
                        compteurGuillement++;
                    else if (onEmailAddress)
                        emailAddress += m_htmlCode.ElementAt(i);

                    if (compteurGuillement != 1)
                        onEmailAddress = false;
                }

                if (emailAddress != "")
                    m_email.Add(emailAddress);


                if (eE > 0)
                    index = eE;
                else
                    break;
            }
        }
    }
}
