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
    public class TeamMembersController : Controller
    {
        private readonly SaaSDBContext _context;

        public TeamMembersController(SaaSDBContext context)
        {
            _context = context;
        }

        // GET: TeamMembers
        public async Task<IActionResult> Index(int userId)
        {
            //delete notifications
            var notifications = _context.Notifications
                    .Include(n => n.User)
                    .Where(m => m.UserId == userId && m.NotificationType == 3).ToListAsync().Result;
            foreach (Notification nt in notifications)
            {
                _context.Remove(nt);
            }
            await _context.SaveChangesAsync();

            //oi klasikes leitourgies tis Index
            TempData["userId"] = userId;
            var saaSDBContext = _context.TeamMembers.Include(t => t.Team).Include(t => t.User).Where(m=>m.UserId == userId);
            return View(await saaSDBContext.ToListAsync());
        }

        // GET: TeamMembers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teamMember = await _context.TeamMembers
                .Include(t => t.Team)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TeamId == id);
            if (teamMember == null)
            {
                return NotFound();
            }

            return View(teamMember);
        }

        // GET: TeamMembers/Create
        public async Task<IActionResult> Create(int userId, int teamId)
        {
            //ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "TeamId");
            var requests = _context.FriendRequests.Where(m => (m.SenderId == userId && m.Status == 0) || (m.ReceiverId == userId && m.Status == 0)).Include(r=>r.Receiver).Include(r => r.Sender);
            /*IEnumerable*/List<User> friends = new List<User>();
            foreach (FriendRequest req in requests)
            {
                if(req.ReceiverId == userId)
                {
                    //friends.Append(req.Sender);
                    friends.Add(req.Sender);
                }else if(req.SenderId == userId)
                {
                    //friends.Append(req.Receiver);
                    friends.Add(req.Receiver);
                }
            }

            var members = await _context.TeamMembers.Where(m => (m.TeamId == teamId)).Include(r=>r.User).ToListAsync();
            /*foreach (User friend in users)
            {
                if (members.Contains(friend))
            }*/
            foreach (TeamMember member in members)
            {
                if (friends.Contains(member.User))
                {
                    friends.Remove(member.User);
                }
            }

            TempData["members"] = members;
            TempData["userId"]=userId;
            TeamMember teamMember = new TeamMember();
            teamMember.TeamId = teamId;
            teamMember.Status = 1;
            ViewData["UserId"] = new SelectList(friends, "UserId", "Username");
            return View(teamMember);
        }

        // POST: TeamMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TeamId,UserId,Status")] TeamMember teamMember)
        {

            Notification notification = new Notification();
            notification.IsRead = false;
            notification.UserId = teamMember.UserId;
            notification.SourceId = teamMember.TeamId;
            notification.NotificationDate = DateTime.UtcNow;
            notification.NotificationType = 3;
            _context.Add(notification);
            await _context.SaveChangesAsync();

            if (ModelState.IsValid)
            {
                _context.Add(teamMember);
                await _context.SaveChangesAsync();
                /*return RedirectToAction("Create", new {userId = TempData["userId"], teamId = teamMember.TeamId});*/
            }
            /*ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "TeamId", teamMember.TeamId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", teamMember.UserId);
            return View(teamMember);*/
            return RedirectToAction("Create", new { userId = TempData["userId"], teamId = teamMember.TeamId });
        }

        // GET: TeamMembers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teamMember = await _context.TeamMembers.FindAsync(id);
            if (teamMember == null)
            {
                return NotFound();
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "TeamId", teamMember.TeamId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", teamMember.UserId);
            return View(teamMember);
        }

        // POST: TeamMembers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TeamId,UserId,Status")] TeamMember teamMember)
        {
            if (id != teamMember.TeamId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teamMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamMemberExists(teamMember.TeamId))
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
            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "TeamId", teamMember.TeamId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", teamMember.UserId);
            return View(teamMember);
        }

        // GET: TeamMembers/Delete/5
        public async Task<IActionResult> Delete(int userId, int teamId)//int? id)
        {
            /*if (id == null)
            {
                return NotFound();
            }*/

            var teamMember = await _context.TeamMembers
                .Include(t => t.Team)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == userId);// == id);
            if (teamMember == null)
            {
                return NotFound();
            }

            return View(teamMember);
        }

        // POST: TeamMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int userId, int teamId)//int id)
        {
            var teamMembers = await _context.TeamMembers.Where(m => m.UserId == userId && m.TeamId == teamId).ToListAsync();//FindAsync(id);
            var teamMember = teamMembers.ElementAt(0);
            if (teamMember != null)
            {
                _context.TeamMembers.Remove(teamMember);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { userId = userId });// nameof(Index));
        }

        private bool TeamMemberExists(int id)
        {
            return _context.TeamMembers.Any(e => e.TeamId == id);
        }
    }
}
