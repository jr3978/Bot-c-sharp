using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Moteur_de_Recherche;
using robot;
using System.Web;

namespace search_bot
{
    class Program
    {
        static void Main(string[] args)
        {
            //WebStatDataContext db = new WebStatDataContext();
            //var req = db.TableUnvisited.Where(l => l.Id >= 31 && l.Id <= 328);

            //foreach (var item in req)
            //{
            //    db.TableUnvisited.DeleteOnSubmit(item);
            //}
            //db.SubmitChanges();
            //Console.WriteLine("done");
            //Console.ReadKey();

            string[] startUrl = {  "http://3cw.ca" };

            Bot bot = new Bot(startUrl);
            bot.LinksOnConsole = true;
            bot.CurrentLinkOnConsole = true;
            bot.ErrorOnConsole = true;
            bot.H1OnConsole = true;

            bot.Search();
        }
    }
}
