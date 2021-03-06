using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Snips.Data
{
    public class Blog : AuditableEntity
    {
        public Blog()
        {
            this.Deleted = false;
            this.Draft = true;
        }
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(280)")]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Content { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string Slug { get; set; }
        public bool Deleted { get; set; }
        public bool Draft { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string? ApplicationUserId { get; set; }
    }
}
