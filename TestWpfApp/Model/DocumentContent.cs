using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrandSmetaReader.Model
{
    /// <summary>
    /// Глава в смете
    /// </summary>
    public class Chapter : DocumentNode
    {
        public Chapter(string tagName, string caption, int? sysId) : base(tagName, caption, sysId)
        {
            Positions = new List<Position>();
        }

        //Строки (позиции) в смете
        public IEnumerable<Position> Positions { get; set; }
    }

    /// <summary>
    /// Позиция в смете (строка внутри главы)
    /// </summary>
    public class Position : DocumentNode
    {
        public Position(string tagName, string caption, int number, string code, int? sysId, string? units) : base(tagName, caption, code, sysId)
        {
            Number = number;
            Units = units ?? "N/A";
        }

        public int Number { get; private set; }
        public string Units { get; private set; }
        public Quantity Qnty { get; set; }
        public Resources Resources { get; set; }
        public IEnumerable<Resource> ResourcesList 
        { 
            get
            {
                return new List<Resource>() { Resources.Human, Resources.Machine };
            }
        }

    }
    /// <summary>
    /// Количество для позиции в смете
    /// </summary>
    public class Quantity : DocumentNode
    {
        public Quantity(string tagName, string caption, string? fx, int? precision, double? result) : base(tagName, caption)
        {
            Fx = fx.Replace(",", ".") ?? "0";
            Precision = precision ?? 3;
            Result = result ?? CalcResult();
        }

        public string Fx { get; private set; }
        public int Precision { get; private set; }
        public double Result { get; private set; }

        private double CalcResult()
        {
            try
            {
                return Math.Round(Convert.ToDouble(new DataTable().Compute(Fx, null)), Precision);
            }
            catch { return 0; }
        }
    }
    /// <summary>
    /// Типы ресурсов в смете
    /// </summary>
    public enum ResourceType
    {
        Tzm, //человек
        Mch  //техника
    }

    /// <summary>
    /// Ресурс для позиции в смете
    /// </summary>
    public abstract class Resource : DocumentNode
    {
        public Resource(string tagName, string caption, string code, string? units, double? quantity) : base(tagName, caption, code)
        {
            Units = units ?? "N/A";
            Quantity = quantity ?? 0;
        }
        public Resource(string tagName, string caption, string code, string? units, double? quantity, ResourceType resourceType) : this(tagName, caption, code, units, quantity)
        {
            ResourceType = resourceType;
        }

        public string Units { get; private set; }

        public double Quantity { get; private set; }

        public ResourceType ResourceType { get; protected set; }

    }

    // Затраты труда машинистов
    public class HumanResource : Resource
    {
        public HumanResource(string tagName, string caption, string code, string? units, double? quantity) : base(tagName, caption, code, units, quantity)
        {
            ResourceType = ResourceType.Tzm;
        }
    }
    //Технический ресурс
    public class MachineResource : Resource
    {
        public MachineResource(string tagName, string caption, string code, string? units, double? quantity) : base(tagName, caption, code, units, quantity)
        {
            ResourceType = ResourceType.Mch;
        }
    }

    //Ресурсы в позиции сметы
    public class Resources
    {
        public Resources(HumanResource human, MachineResource machine)
        {
            Human = human;
            Machine = machine;
        }

        public Resource Human { get; private set; }

        public Resource Machine { get; private set; }

    }
}
