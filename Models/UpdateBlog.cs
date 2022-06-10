using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snips.Models
{
    public class UpdateBlog
    {
        public string content { get; set; }
        public string name { get; set; }
        public DateTime date { get; set; }
        public bool draft { get; set; }
    }
}
