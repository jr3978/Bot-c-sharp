using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moteur_de_Recherche
{
    public class MetaTag
    {
        public string Name { get; set; }
        public string Content { get; set; }

        public MetaTag(string name, string content)
        {
            this.Name = name;
            this.Content = content;
        }
    }
}
