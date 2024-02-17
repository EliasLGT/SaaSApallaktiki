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
    public class MessagesController : Controller
    {
        private readonly SaaSDBContext _context;

        public MessagesController(SaaSDBContext context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> Index(int userId, int otherUserId, int teamId)
        {
            TempData["userId"] = userId;
            
            if (teamId == 0 && otherUserId != 0)//        DM with friend
            {
                TempData["DM"] = true;
                TempData["other"] = otherUserId;
                var messages = _context.Messages.Where(m => (m.SenderId == userId && m.RecipientId == otherUserId) || (m.RecipientId == userId && m.SenderId == otherUserId)).Include(r => r.Sender); //.Include(r => r.);
                return View(messages);
            }
            else if (teamId != 0 && otherUserId == 0)//     Team chat
            {
                TempData["DM"] = false;
                TempData["other"] = teamId;
                var messages = _context.Messages.Where(m => (m.SenderId == userId && m.RecipientId == teamId) || (m.RecipientId == userId && m.SenderId == teamId)).Include(r => r.Sender); //.Include(r => r.);
                return View(messages);
            }
            else
            {
                var saaSDBContext = _context.Messages.Include(m => m.Sender);
                return View(await saaSDBContext.ToListAsync());
            }

            /*var saaSDBContext = _context.Messages.Include(m => m.Sender);
            return View(await saaSDBContext.ToListAsync());*/
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public async Task<IActionResult> Create(int userId, int otherUserId, int teamId)//int userId, int otherId)
        {
            /*//An uparxoun notifications gia aytin tin syzitisi, ta diagrafo (kai lino sinama to problima tis diagrfis tous ama rtho se aytin tin sizitisi apo ena notification)
            var notifications = _context.Notifications
                .Include(n => n.User)
                .Where(m => m.UserId == userId && m.SourceId == otherUserId).ToListAsync().Result;
            foreach (Notification nt in notifications)
            {
                _context.Remove(nt);
            }
            _context.SaveChanges();*/


            TempData["userId"] = userId;
            List<Message> li;//= new List<Message>();
            Message message = new Message();
            if (teamId == 0 && otherUserId != 0)//        DM with friend
            {
                var user = _context.Users.FirstOrDefault(m=>m.UserId == otherUserId);
                TempData["otherUsername"] = user.Username;
                //An uparxoun notifications gia aytin tin syzitisi, ta diagrafo (kai lino sinama to problima tis diagrfis tous ama rtho se aytin tin sizitisi apo ena notification)
                var notifications = _context.Notifications
                    .Include(n => n.User)
                    .Where(m => m.UserId == userId && m.SourceId == otherUserId).ToListAsync().Result;
                foreach (Notification nt in notifications)
                {
                    _context.Remove(nt);
                }
                await _context.SaveChangesAsync();

                TempData["DM"] = true;
                TempData["other"] = otherUserId;
                var messages = _context.Messages.Where(m => (m.SenderId == userId && m.RecipientId == otherUserId) || (m.RecipientId == userId && m.SenderId == otherUserId)).Include(r => r.Sender); //.Include(r => r.);
                li = await messages.ToListAsync();
                message.SenderId = userId;
                message.RecipientId = otherUserId;
                message.SentDate = DateTime.UtcNow;
            }
            else if (teamId != 0 && otherUserId == 0)//     Team chat
            {
                var team = _context.Teams.FirstOrDefault(m => m.TeamId == teamId);
                TempData["otherUsername"] = "team '" + team.TeamName + "'";

                //An uparxoun notifications gia aytin tin syzitisi, ta diagrafo (kai lino sinama to problima tis diagrfis tous ama rtho se aytin tin sizitisi apo ena notification)
                var notifications = _context.Notifications
                    .Include(n => n.User)
                    .Where(m => m.UserId == userId && m.SourceId == teamId).ToListAsync().Result;
                foreach (Notification nt in notifications)
                {
                    _context.Remove(nt);
                }
                await _context.SaveChangesAsync();

                TempData["DM"] = false;
                TempData["other"] = teamId;
                var messages = _context.Messages.Where(m => m.RecipientId == teamId).Include(r => r.Sender); //.Include(r => r.);
                li = await messages.ToListAsync();
                message.SenderId = userId;
                message.RecipientId = teamId;
                message.SentDate = DateTime.UtcNow;
            }
            else
            {
                var saaSDBContext = _context.Messages.Include(m => m.Sender);
                li = await saaSDBContext.ToListAsync();
            }
            TempData["list"] = li;

            return View(message);

            /*Message message = new Message();
            message.SenderId = userId;
            message.RecipientId = otherId;
            message.SentDate = DateTime.UtcNow;
            return View(message);*/
            /*ViewData["SenderId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();*/
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MessageId,SenderId,RecipientId,MessageContent,SentDate")] Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                /*var temp = _context.Teams.FirstOrDefaultAsync(m => m.TeamId == message.RecipientId);
                var temp2 = _context.Users.FirstOrDefaultAsync(m => m.UserId == message.RecipientId);*/

                //Adding notification for the receiver
                
                if ((bool)TempData["dm"])
                {
                    Notification notification = new Notification();
                    notification.IsRead = false;
                    notification.UserId = message.RecipientId;
                    notification.SourceId = message.SenderId;
                    notification.NotificationDate = DateTime.UtcNow;
                    notification.NotificationType = 1;
                    _context.Add(notification);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var teamMembers = await _context.TeamMembers.Include(r=>r.User).Include(r=>r.Team).Where(m=>m.TeamId == message.RecipientId).ToListAsync();
                    foreach(TeamMember member in teamMembers)
                    {
                        Notification notification = new Notification();
                        notification.IsRead = false;
                        notification.UserId = member.UserId;
                        notification.SourceId = message.RecipientId;
                        notification.NotificationDate = DateTime.UtcNow;
                        notification.NotificationType = 5;
                        _context.Add(notification);
                    }
                    await _context.SaveChangesAsync();
                }

                if ((bool)TempData["dm"])//(temp == null && temp2 != null)
                {
                    return RedirectToAction("Create", new { userId = message.SenderId, otherUserId = message.RecipientId });
                }else //if (temp != null && temp2 == null)
                {
                    return RedirectToAction("Create", new { userId = message.SenderId, teamId = message.RecipientId });
                }
                //return RedirectToAction("Create", new { userId = message.SenderId, otherUserId = message.RecipientId });
                //return RedirectToAction("Create", new { userId = message.SenderId, otherUserId = message.RecipientId}); //return RedirectToAction(nameof(Index));
            }
            //ViewData["SenderId"] = new SelectList(_context.Users, "UserId", "UserId", message.SenderId);
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["SenderId"] = new SelectList(_context.Users, "UserId", "UserId", message.SenderId);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MessageId,SenderId,RecipientId,MessageContent,SentDate")] Message message)
        {
            if (id != message.MessageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.MessageId))
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
            ViewData["SenderId"] = new SelectList(_context.Users, "UserId", "UserId", message.SenderId);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.MessageId == id);
        }
    }
}
