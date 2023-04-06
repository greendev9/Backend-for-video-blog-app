using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cultures
{
    public class Culture
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsRTL { get; set; }
        public bool IsImplemented { get; set; }
        public bool IsDefault { get; set; }
        public string Money { get; set; }
        public string SepStart { get; set; }
        public string SepEnd { get; set; }
    }
}
