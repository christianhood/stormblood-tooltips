using CsvHelper;
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
        static string file = "Actions.csv";
        static string traitsFile = "Traits.csv";
        static void Main(string[] args)
        {
            var actionsHtml = generateActions();
            var traitsHtml = generateTraits();
            Console.Read();

        }

        static string generateTraits()
        {
            List<Trait> traits;
            string fullHtml = "";
            using (var textReader = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), traitsFile)))
            {
                var csv = new CsvReader(textReader);
                traits = csv.GetRecords<Trait>().ToList();
            }

            string htmlTemplate = "<div class='ability container'><div class='row'><h3>{0}</h3></div>" +
                "<div class='row'>" +
                    "<span class='info col-xs-3' >Trait</span>" +
                "</div>" +
                "<hr />" +
                "<div class='row'>" +
                    "<div class='col-md-12'>" +
                        "<p>{1}</p>" +
                    "</div>" +
                "</div>" +
                "<div class='row'>" +
                    "<div class='col-md-12'>" +
                        "Level: {2}" +
                        "<br />Class/Job: {3}" +
                    "</div>" +
                "</div>" +
            "</div>";

            foreach (var trait in traits)
            {
                fullHtml += string.Format(htmlTemplate, trait.Name, trait.Description, trait.Level, trait.ClassJob);
            }


            return fullHtml;
        }

        static string generateActions()
        {
            List<Action> actions;
            using (var textReader = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), file)))
            {
                var csv = new CsvReader(textReader);
                actions = csv.GetRecords<Action>().ToList();
            }

            string htmlTemplate = "<div class='ability container'><div class='row'><h3>{0}</h3></div>" +
                "<div class='row'>" +
                    "<span class='info col-xs-3' >{1}</span>" +
                    "<span class='col-xs-3' ></span>" +
                    "<span class='info col-xs-3'>Range: ??</span>" +
                    "<span class='info col-xs-3'>Radius: ??</span>" +
                "</div>" +
                "<div class='row'>" +
                    "<div class='col-xs-1'></div>" +
                    "<div class='col-xs-5'>" +
                        "<h3><small class='cast'>Cast</small><br/>??</h3>" +
                    "</div>" +
                    "<div class='col-xs-5'>" +
                        "<h3><small class='recast'>Recast</small><br/>{2}s</h3>" +
                    "</div>" +
                    "<div class='col-xs-1'></div>" +
                "</div>" +
                "<hr />" +
                "<div class='row'>" +
                    "<div class='col-md-12'>" +
                        "<p>{3}</p>" +
                    "</div>" +
                "</div>" +
            "</div>";

            var fullHtml = "";
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


                fullHtml += string.Format(htmlTemplate, action.Name, action.Category, ((float)action.Cooldown / 10).ToString("0.00"), desc);

            }

            return fullHtml;


        }
    }
}
