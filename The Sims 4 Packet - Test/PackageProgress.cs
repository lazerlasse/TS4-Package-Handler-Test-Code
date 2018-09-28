using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Sims_4_Packet___Test
{
	class PackageProgress
	{
		public IPackage Package { get; set; }
		public FileInfo FileInfos { get; set; }
		public string Conflict { get; set; }
	}
}
