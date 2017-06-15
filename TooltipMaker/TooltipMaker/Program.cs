using CsvHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooltipMaker
{
    class Program
    {
        static string file = "Data\\Actions.csv";
        static string traitsFile = "Data\\Traits.csv";
        static void Main(string[] args)
        {
            generateActions();
            generateTraits();
            Console.WriteLine("Done!");
            Console.Read();

        }

        static void generateTraits()
        {
            List<Trait> traits;
            using (var textReader = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), traitsFile)))
            {
                var csv = new CsvReader(textReader);
                traits = csv.GetRecords<Trait>().ToList();
            }

            string json = JsonConvert.SerializeObject(traits);

            using (var writer = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "traits.json")))
            {
                writer.Write(json);
            }
        }

        static void generateActions()
        {
            List<Action> actions;
            using (var textReader = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), file)))
            {
                var csv = new CsvReader(textReader);
                actions = csv.GetRecords<Action>().ToList();
            }

            JArray actionsArray = new JArray();
            foreach (var action in actions)
            {
                var desc = action.Description;
                while (desc.Contains("<Color("))
                {
                    int indexOf = desc.IndexOf("<Color(");
                    string subst = desc.Substring(indexOf);
                    int indexEnd = subst.IndexOf(")>");
                    string color = desc.Substring(indexOf + 7, indexEnd - 7);

                    string hexValue = "";
                    if (color.StartsWith("-"))
                    {
                        int colorVal = int.Parse(color.Substring(1));
                        colorVal = 16777215 - colorVal;
                        hexValue = colorVal.ToString("X");
                    }
                    else
                    {
                        int colorVal = int.Parse(color);
                        hexValue = colorVal.ToString("X");
                        while (hexValue.Length < 6)
                        {
                            hexValue = "0" + hexValue;
                        }
                    }

                    desc = desc.Remove(indexOf, indexEnd + 2);
                    desc = desc.Insert(indexOf, string.Format("<span style='color: #{0};'>", hexValue));
                }
                desc = desc.Replace("</Color>", "</span>");

                while (desc.Contains("<If"))
                {
                    int indexOf = desc.IndexOf("<If");
                    string subt = desc.Substring(indexOf);
                    int indexEnd = subt.IndexOf(">");
                    string fullIf = desc.Substring(indexOf, indexEnd + 1);
                    string newIf = fullIf.Replace("<", "&lt;");
                    newIf = newIf.Replace(">", "&gt;");
                    desc = desc.Replace(fullIf, newIf);
                }

                desc = desc.Replace("</If>", "&lt;/If&gt;");
                desc = desc.Replace("<Else/>", "&lt;Else/&gt;");

                JObject a = new JObject();
                a["ID"] = action.ID;
                a["Name"] = action.Name;
                a["Category"] = action.Category;
                a["Cooldown"] = ((float)action.Cooldown / 10).ToString("0.00");
                a["Description"] = desc;
                actionsArray.Add(a);
            }

            using (var writer = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "actions.json"), false))
            {
                writer.Write(actionsArray.ToString(Formatting.None));
            }
        }
    }
}
