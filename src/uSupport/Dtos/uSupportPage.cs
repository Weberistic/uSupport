using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace uSupport.Dtos
{
	public class uSupportPage<T>
	{
		public long CurrentPage { get; set; }
		public long TotalPages { get; set; }
		public long TotalItems { get; set; }
		public long ItemsPerPage { get; set; }
		public IEnumerable<T> Items { get; set; }
	}
}