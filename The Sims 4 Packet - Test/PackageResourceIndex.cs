using s4pi.Interfaces;
using s4pi.Package;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Sims_4_Packet___Test
{
    public class PackageDetailedViewList
    {
        // Definitions...
        public string PackageName { get; set; }
        public string ResourceTextType { get; set; }
        public uint ResourceType { get; set; }
        public uint ResourceGroup { get; set; }
        public ulong ResourceInstance { get; set; }
        public string ConflictType { get; set; }
    }

    public class PackageViewList
    {
        public string PackageName { get; set; }
        public string PackageType { get; set; }
        public DateTime PackageCreated { get; set; }
        public DateTime PackageEdited { get; set; }
		public string ResourceTag { get; set; }
		public string ResourceType { get; set; }
		public string ResourceGroup { get; set; }
		public string ResourceInstance { get; set; }
		public string ConflictDetected { get; set; }
    }
}
