using GrandSmetaReader.Model;
using GrandSmetaReader.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GrandSmetaReader.ViewModel
{
    public class SmetaViewModel
    {
        public List<Chapter> Chapters { get; private set; }

        public string Uri { get; private set; }

        public SmetaViewModel(string uri)
        {
            Uri = uri;
            SmetaReader reader = new SmetaReader("Test smeta", Uri);
            var smeta = reader.Read();

            if (smeta != null)
                Chapters = smeta.Chapters?.ToList();
        }
    }
}
