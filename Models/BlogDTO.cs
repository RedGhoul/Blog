using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snips.Models
{
    public class BlogDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Slug { get; set; }
        public bool Draft { get; set; }
        public bool Deleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}
