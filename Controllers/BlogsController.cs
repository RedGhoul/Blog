using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Slugify;
using Snips.Data;
using Snips.Models;

namespace Snips.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly ILogger<BlogsController> _logger;
        
        public BlogsController(ILogger<BlogsController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            var snipsQueryItems = _context.Blogs
                .Where(x => x.Deleted == false && x.ApplicationUserId.Equals(GetCurrentUserId()))
                .OrderByDescending(x => x.LastModified)
                .Select(x => new
                    BlogDTO
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Draft = x.Draft,
                            Deleted = x.Deleted,
                            Created = x.Created,
                            LastModified = x.LastModified.GetValueOrDefault(DateTime.UtcNow)
                        }).ToListAsync();
            return View(await snipsQueryItems);
        }

        // POST: Search
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string SearchTerm, DateTime CreatedDate, DateTime LastModifiedDate)
        {
            IQueryable<Data.Blog> snipsQuery = _context.Blogs.Where(x => x.ApplicationUserId.Equals(GetCurrentUserId()) && x.Deleted == false);


            if (!string.IsNullOrEmpty(SearchTerm))
            {
                snipsQuery = snipsQuery
                        .Where(x => x.Content.Contains(SearchTerm) || x.Name.Contains(SearchTerm));
            }


            if (LastModifiedDate != new DateTime())
            {

                snipsQuery = snipsQuery.Where(x => x.LastModified.Value.Date == LastModifiedDate.ToUniversalTime().Date);
            }

            if (CreatedDate != new DateTime())
            {
                snipsQuery = snipsQuery.Where(x => x.Created.Date == CreatedDate.ToUniversalTime().Date);
            }
            var snipsQueryItems = snipsQuery.OrderByDescending(x => x.LastModified).Select(x => new BlogDTO
            {
                Id = x.Id,
                Name = x.Name,
                Created = x.Created,
                LastModified = (DateTime)x.LastModified
            });

            try
            {
                return View("Index", await snipsQueryItems.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching the DB for Blogs with SearchTerms {SearchTerm}");
                var AllUserSnips = await _context.Blogs
                    .Where(x => x.ApplicationUserId.Equals(GetCurrentUserId()) && x.Deleted == false)
                    .Select(x => new BlogDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Created = x.Created,
                    LastModified = (DateTime)x.LastModified
                }).ToListAsync();
                return View("Index", AllUserSnips);
            }
        }
        

        // GET: Blogs/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> DetailsById(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var note = await _context.Blogs.Where(
                    x => x.Deleted == false && x.ApplicationUserId.Equals(GetCurrentUserId()))
                .Include(x => x.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            return View("Details",note);
        }
        
        // GET: Blogs/Details/slug
        [AllowAnonymous]
        public async Task<IActionResult> DetailsBySlug(string? slug)
        {
            if (slug == null)
            {
                return NotFound();
            }

            var note = await _context.Blogs.Where(
                    x => x.Deleted == false && x.Draft == false)
                .Include(x => x.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Slug == slug);
            if (note == null)
            {
                return NotFound();
            }

            return View("Details",note);
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            var vm = new NoteCreateViewModel
            {
                Blog = new Data.Blog()
                {

                },
            };
            return View(vm);
        }

        // POST: Blogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NoteCreateViewModel vm)
        {

            if (string.IsNullOrWhiteSpace(vm.Blog.Name))
            {
                return View(vm);
            }
            vm.Blog.ApplicationUserId = GetCurrentUserId();
            vm.Blog.Slug = $"{new SlugHelper().GenerateSlug(vm.Blog.Name)}-{vm.Blog.Id}";
            vm.Blog.LastModified = DateTime.UtcNow;
            vm.Blog.Created = DateTime.UtcNow;

            _context.Add(vm.Blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Blogs.Where(x => x.ApplicationUserId.Equals(GetCurrentUserId()))
                .SingleOrDefaultAsync(x => x.Id == id);

            if (note == null)
            {
                return NotFound();
            }

            var vm = new BlogEditViewModel
            {
                Blog = note,
            };

            return View(vm);
        }

        // POST: Blogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogEditViewModel vm)
        {
            if (id != vm.Blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    vm.Blog.ApplicationUserId = GetCurrentUserId();
                    vm.Blog.Slug = $"{new SlugHelper().GenerateSlug(vm.Blog.Name)}-{vm.Blog.Id}";
                    vm.Blog.LastModified = DateTime.UtcNow;

                    _context.Update(vm.Blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(vm.Blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Blogs.Where(x => x.ApplicationUserId.Equals(GetCurrentUserId()))
                .FirstOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var note = await _context.Blogs
                .Where(x => x.ApplicationUserId.Equals(GetCurrentUserId()) && x.Id == id)
                .FirstOrDefaultAsync();
            if (note != null)
            {
                note.Deleted = true;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return RedirectToAction(nameof(Edit), new { id });
            
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
