using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GirlFriend
{
    internal class TemplateResponder : Responder
    {
        public TemplateResponder(string name, Cdictionary dic) : base(name, dic)
        { 
        }

        public override string Response(string input, int mood, List<string[]> parts)
        {
            List<string> keywords = new();

            foreach (string[] morpheme in parts)
            {
                if (Analyzer.KeyWordCheck(morpheme[1]).Success)
                { 
                    keywords.Add(morpheme[0]);
                }
            }

            Random rdm = new(Environment.TickCount);

            int count = keywords.Count;

            if ((count > 0) && (Cdictionary.Template.ContainsKey(Convert.ToString(count))))
            { 
                var templates = Cdictionary.Template[Convert.ToString(count)];
                string temp = templates[rdm.Next(0, templates.Count)];

                Regex re = new("%noun%");

                foreach (string word in keywords)
                {
                    temp = re.Replace(temp, word, 1);
                }

                return temp;
            }

            return Cdictionary.Random[rdm.Next(0, Cdictionary.Random.Count)];
        }
    }
}
