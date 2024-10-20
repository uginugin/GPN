using GrandSmetaReader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrandSmetaReader.Services
{
    public class SmetaReader
    {
        public SmetaReader(string? name, string uri)
        {
            Document = new GrandSmetaDoc(name ?? "Новая смета", "GrandSmeta файл");
            if (Document != null)
                Document.Load(uri);
        }

        public GrandSmetaDoc Document { get; private set; }

        public GrandSmetaDoc Read()
        {
            if (Document != null)
            {
                //главы
                var chapters = Document.Instance?.Descendants("Chapter").Select(x => new Chapter("Chapter",
                    x.Attribute("Caption")?.Value ?? string.Empty, (int?)x.Attribute("SysId"))
                {
                    //позиции
                    Positions = x.Elements("Position").Select(p => new Position(
                        "Position",
                        p.Attribute("Caption")?.Value ?? string.Empty,
                        (int?)p.Attribute("Number") ?? 0,
                        p.Attribute("Code")?.Value ?? string.Empty,
                        (int?)p.Attribute("SysID"),
                        p.Attribute("Units")?.Value
                        )
                    {
                        //кол-во
                        Qnty = new Quantity("Quantity",
                        p.Element("Quantity")?.Attribute("Caption")?.Value ?? string.Empty,
                        p.Element("Quantity")?.Attribute("Fx")?.Value ?? string.Empty,
                        (int?)p.Element("Quantity")?.Attribute("Presicion"),
                        Convert.ToDouble(p.Element("Quantity")?.Attribute("Result").Value ?? "0")),

                        //ресурсы
                        Resources = new Resources(

                            human: new HumanResource(
                            "Tzm",
                            p.Element("Resources")?.Element("Tzm")?.Attribute("Caption")?.Value ?? string.Empty,
                            p.Element("Resources")?.Element("Tzm")?.Attribute("Code")?.Value ?? string.Empty,
                            p.Element("Resources")?.Element("Tzm")?.Attribute("Units")?.Value,
                            Convert.ToDouble(p.Element("Resources")?.Element("Tzm")?.Attribute("Quantity").Value ?? "0")),

                            machine: new MachineResource(
                            "Mch",
                            p.Element("Resources")?.Element("Mch")?.Attribute("Caption")?.Value ?? string.Empty,
                            p.Element("Resources")?.Element("Mch")?.Attribute("Code")?.Value ?? string.Empty,
                            p.Element("Resources")?.Element("Mch")?.Attribute("Units")?.Value,
                            Convert.ToDouble(p.Element("Resources")?.Element("Tzm")?.Attribute("Quantity").Value ?? "0"))

                        )
                    })
                }

                ).ToList();

                if (chapters != null) Document.Chapters = chapters;
            }

            return Document;

        }

    }
}
