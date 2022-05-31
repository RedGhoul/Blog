using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Snips.Data
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.DateCreated = DateTime.UtcNow;
        }
        [Column(TypeName = "nvarchar(280)")]
        public string FirstName { get; set; }
        [Column(TypeName = "nvarchar(280)")]
        public string LastName { get; set; }
        public ICollection<Note> Notes { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
