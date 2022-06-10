using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slugify;
using Snips.Data;
using Snips.Models;

namespace Snips.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class BlogsController : BaseController
    {

        public BlogsController(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager):base(context, userManager)
        {
        }

        // POST: api/Blogs/BlogName
        [HttpPost("BlogName/{id}")]
        public async Task<ActionResult> UpdateBlogName(int id, [FromBody] UpdateBlog value)
        {
            var curUser = await GetCurrentUser();
            var Blog = await _context.Blogs.Where(
                x => x.Id == id &&
                x.ApplicationUserId.Equals(curUser.Id)).FirstOrDefaultAsync();

            if (Blog == null)
            {
                return NotFound();
            }

            Blog.Name = value.name;
            Blog.Slug = $"{new SlugHelper().GenerateSlug(Blog.Name)}-{Blog.Id}";
            _context.Entry(Blog).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok("Saved");
        }
        
        // POST: api/Blogs/BlogCreated
        [HttpPost("BlogCreated/{id}")]
        public async Task<ActionResult> UpdateBlogCreated(int id, [FromBody] UpdateBlog value)
        {
            var curUser = await GetCurrentUser();
            var Blog = await _context.Blogs.Where(
                x => x.Id == id &&
                     x.ApplicationUserId.Equals(curUser.Id)).FirstOrDefaultAsync();

            if (Blog == null)
            {
                return NotFound();
            }

            Blog.Created = value.date;
            _context.Entry(Blog).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok("Saved");
        }
        
        // POST: api/Blogs/BlogDraft
        [HttpPost("BlogDraft/{id}")]
        public async Task<ActionResult> UpdateDraftState(int id, [FromBody] UpdateBlog value)
        {
            var curUser = await GetCurrentUser();
            var Blog = await _context.Blogs.Where(
                x => x.Id == id &&
                     x.ApplicationUserId.Equals(curUser.Id)).FirstOrDefaultAsync();

            if (Blog == null)
            {
                return NotFound();
            }

            Blog.Draft = value.draft;
            _context.Entry(Blog).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok("Saved");
        }

        // POST: api/Blogs/BlogContent
        [HttpPost("BlogContent/{id}")]
        public async Task<ActionResult> UpdateCodeContent(int id,[FromBody] UpdateBlog value)
        {
            var curUser = await GetCurrentUser();
            var Blog = await _context.Blogs.Where(
                x => x.Id == id && 
                x.ApplicationUserId.Equals(curUser.Id)).FirstOrDefaultAsync();

            if (Blog == null)
            {
                return NotFound();
            }

            Blog.Content = value.content;
            _context.Entry(Blog).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok("Saved");
        }
        

        // GET: api/Blogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Data.Blog>>> GetBlog()
        {
            return await _context.Blogs.ToListAsync();
        }

        // GET: api/Blogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Data.Blog>> GetBlog(int id)
        {
            var Blog = await _context.Blogs.FindAsync(id);

            if (Blog == null)
            {
                return NotFound();
            }

            return Blog;
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}
