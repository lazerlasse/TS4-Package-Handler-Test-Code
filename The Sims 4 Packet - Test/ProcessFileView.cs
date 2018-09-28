using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Sims_4_Packet___Test
{
    class ProcessFileView
    {
        public string Name { get; set; }
        public string ResourceType { get; set; }
        public uint Type { get; set; }
        public uint Group { get; set; }
        public ulong Instance { get; set; }
        public string Conflict { get; set; }
    }
}
