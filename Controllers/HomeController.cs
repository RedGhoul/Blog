using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Snips.Data;
using Snips.Models;

namespace Snips.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        
        public HomeController(UserManager<ApplicationUser> userManager, ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var snipsQueryItems = await _context.Blogs
                .Where(x => x.Deleted == false 
                            && x.Draft == false)
                                .OrderByDescending(x => x.LastModified)
                .Select(x => new
                    BlogDTO
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Slug = x.Slug,
                        Content = x.Content,
                        Created = x.Created,
                        LastModified = x.LastModified.GetValueOrDefault(DateTime.UtcNow)
                    }).ToListAsync();
            return View(snipsQueryItems);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        private bool NoteExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }

        private string GetCurrentUserId()
        {
            if (User != null)
            {
                return _userManager.GetUserId(User);
            }
            else { return null; }
        }
    }
}
