using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab03
{
    public class Persona
    {
        public string name { get; set; }
        public string dpi { get; set; }
        public DateTime datebirth { get; set; }
        public string address { get; set; }
        public string[] companies { get; set; }       
        
        public string[] cartas { get; set; }
    }
}
