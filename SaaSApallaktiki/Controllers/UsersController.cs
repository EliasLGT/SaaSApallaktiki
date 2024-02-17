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
    public class UsersController : Controller
    {
        private readonly SaaSDBContext _context;

        public UsersController(SaaSDBContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(int userId, string filterBy)
        {
            /*TempData["userId"] = userId;
            return View(await _context.Users.Include(r => r.FriendRequestReceivers).Include(r => r.FriendRequestSenders).Where(m => m.UserId == userId).ToListAsync());*/

            //var acceptedRequestsRelatedToMe = _context.FriendRequests.Include(f => f.Receiver).ThenInclude(f)  //.Include(f => f.Sender).Where(m => (m.Status == 1) && ((m.SenderId == userId) || (m.ReceiverId == userId)));

            /*List<User> li = new List<User>();
            foreach(var request in acceptedRequestsRelatedToMe)
            {
                if(request.SenderId == userId)
                {
                    li.Add(request.Receiver);
                }else if(request.ReceiverId == userId)
                {
                    li.Add(request.Sender);
                }
            }*/


            //TempData["friends"] = li;

            TempData["hobbies"] = await _context.Hobbies.ToListAsync();
            TempData["courses"] = await _context.Courses.ToListAsync();

            TempData["userId"] = userId;
            if(filterBy != null)
            {
                TempData["filterBy"] = filterBy;
                /*_context.Hobbies*/
                return View(await _context.Users.Include(r => r.FriendRequestReceivers).Include(r => r.FriendRequestSenders).Include(r => r.Hobbies).Include(r => r.Courses).Where(m => m.Hobbies.Any(r=>r.HobbyName == filterBy) || m.Courses.Any(r => r.CourseName == filterBy)).ToListAsync());
            }
            return View(await _context.Users.Include(r => r.FriendRequestReceivers).Include(r => r.FriendRequestSenders).ToListAsync()); //.ThenInclude(r=>r.Receiver).Include(r => r.FriendRequestSenders).ThenInclude(r => r.Sender).ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                while (true)
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();

                    //Check if there is a team with the same Primary Key
                    string usernmae = user.Username;
                    var userr = await _context.Users.Where(r => r.Username == usernmae).ToListAsync();

                    var team = _context.Teams.Where(m => m.TeamId == userr.First().UserId);
                    if (!team.Any())
                    {
                        return RedirectToAction("MainMenu", "Home", new {userId = userr.First().UserId});
                    }
                    _context.Remove(user);
                    await _context.SaveChangesAsync();
                }
                /*_context.Add(user);
                await _context.SaveChangesAsync();

                //Check if there is a db with the same Primary Key
                string usernmae = user.Username;
                var userr = await _context.Users.Where(r => r.Username == usernmae).ToListAsync();

                var team = _context.Teams.Where(m => m.TeamId == userr.First().UserId);
                if (team.Any())
                {

                }

                return RedirectToAction("LoggedIn", "Home");*/
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Password")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
