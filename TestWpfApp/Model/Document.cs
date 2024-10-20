using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GrandSmetaReader.Model
{
    /// <summary>
    /// Абстрактный документ формата XML
    /// </summary>
    public abstract class Document
    {
        protected Document(string? name, string? description)
        {
            Name = name;
            Description = description;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public XDocument? Instance { get; private set; }

        public virtual void Load(string uri)
        {
            try
            {
                Instance = XDocument.Load(uri);
            }
            catch (Exception)
            {
                Close();
            }
        }

        public virtual void Close()
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Узел в документе
    /// </summary>
    public abstract class DocumentNode
    {
        public DocumentNode(string tagName, string caption)
        {
            TagName = tagName;
            Caption = caption;
        }

        public DocumentNode(string tagName, string caption, int? sysId) : this(tagName, caption)
        {
            SysId = sysId;
        }
        public DocumentNode(string tagName, string caption, string code) : this(tagName, caption)
        {
            Code = code;
        }
        public DocumentNode(string tagName, string caption, string code, int? sysId) : this(tagName, caption, code)
        {
            SysId = sysId;
        }

        public string TagName { get; protected set; }
        public string Caption { get; protected set; }
        public string? Code { get; protected set; }
        public int? SysId { get; protected set; }

    }

    /// <summary>
    /// Документ формата GrandSmeta
    /// </summary>
    public class GrandSmetaDoc : Document
    {
        public static readonly string Generator = "GrandSmeta";
        public static readonly Guid DocumentType = new("2B0470FD-477C-4359-9F34-EEBE36B7D340");

        public IEnumerable<Chapter> Chapters { get; set; }


        public GrandSmetaDoc(string? name, string? description) : base(name, description)
        {
            Chapters = new List<Chapter>();
        }

        public override void Load(string uri)
        {
            base.Load(uri);
            if (Instance != null)
            {

                string? generator = Instance.Root?.Attribute("Generator")?.Value;
                if (Generator != generator)
                {
                    base.Close();
                    throw new Exception(string.Format("Неверный формат документа: [{0}]", generator));

                }

                Guid.TryParse(Instance.Root?.Attribute("DocumentType")?.Value, out Guid docType);
                if (DocumentType != docType)
                {
                    base.Close();
                    throw new Exception(string.Format("Неверный тип документа: [{0}]", docType.ToString()));
                }

            }
        }
    }
}
