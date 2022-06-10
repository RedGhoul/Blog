using Microsoft.AspNetCore.Mvc.Rendering;
using Snips.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snips.Models
{
    public class BlogEditViewModel
    {
        public Data.Blog Blog { get; set; }
    }
}
