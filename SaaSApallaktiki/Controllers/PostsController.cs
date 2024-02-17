using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaaSApallaktiki.Models;

namespace SaaSApallaktiki.Controllers
{
    public class PostsController : Controller
    {
        private readonly SaaSDBContext _context;

        public PostsController(SaaSDBContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index(int userId, string filterBy)
        {
            TempData["hobbies"] = await _context.Hobbies.ToListAsync();
            TempData["courses"] = await _context.Courses.ToListAsync();
            TempData["users"] = await _context.Users.ToListAsync();
            var notifications = _context.Notifications
                    .Include(n => n.User)
                    .Where(m => m.UserId == userId && m.NotificationType == 4).ToListAsync().Result;
            foreach (Notification nt in notifications)
            {
                _context.Remove(nt);
            }
            await _context.SaveChangesAsync();

            TempData["userId"] = userId;
            if (filterBy != null)
            {
                TempData["filterBy"] = filterBy;
                return View(await _context.Posts.Include(p => p.Course).Include(p => p.Hobby).Include(p => p.User).Where(m=>m.Hobby.HobbyName == filterBy || m.Course.CourseName == filterBy || m.User.Username == filterBy).ToListAsync());
            }
            return View(await _context.Posts.Include(p => p.Course).Include(p => p.Hobby).Include(p => p.User).ToListAsync());

            /*var saaSDBContext = _context.Posts.Include(p => p.Course).Include(p => p.Hobby).Include(p => p.User);
            return View(await saaSDBContext.ToListAsync());*/
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Course)
                .Include(p => p.Hobby)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create(int userId, string aboutA)
        {
            TempData["aboutA"] = aboutA;
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName");
            ViewData["HobbyId"] = new SelectList(_context.Hobbies, "HobbyId", "HobbyName");
            ViewData["userId"] = userId; // new SelectList(_context.Users, "UserId", "UserId");
            Post post = new Post { UserId = userId, PostDate = DateTime.Now };
            return View(post);
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,UserId,HobbyId,CourseId,PostTitle,PostContent,PostDate")] Post post)
        {
            string ab = TempData["AboutA"] as string;
            if (((string)TempData["AboutA"]).Equals("hobby"))
            {
                post.CourseId = null;
            }else if (((string)TempData["AboutA"]).Equals("course"))
            {
                post.HobbyId = null;
            }
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();

                var users = await _context.Users.ToListAsync();
                foreach(User user in users)
                {
                    Notification notification = new Notification();
                    notification.IsRead = false;
                    notification.UserId = user.UserId;
                    notification.SourceId = post.PostId;
                    notification.NotificationDate = DateTime.UtcNow;
                    notification.NotificationType = 4;
                    _context.Add(notification);
                }
                
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new {userId = post.UserId});
            }
            return RedirectToAction("Create", new {userId = post.UserId, aboutA = ab});
            /*ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", post.CourseId);
            ViewData["HobbyId"] = new SelectList(_context.Hobbies, "HobbyId", "HobbyId", post.HobbyId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", post.UserId);
            return View(post);*/
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", post.CourseId);
            ViewData["HobbyId"] = new SelectList(_context.Hobbies, "HobbyId", "HobbyId", post.HobbyId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", post.UserId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,UserId,HobbyId,CourseId,PostTitle,PostContent,PostDate")] Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", post.CourseId);
            ViewData["HobbyId"] = new SelectList(_context.Hobbies, "HobbyId", "HobbyId", post.HobbyId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", post.UserId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Course)
                .Include(p => p.Hobby)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
