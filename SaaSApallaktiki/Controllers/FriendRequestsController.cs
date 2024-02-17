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
    public class FriendRequestsController : Controller
    {
        private readonly SaaSDBContext _context;

        public FriendRequestsController(SaaSDBContext context)
        {
            _context = context;
        }

        // GET: FriendRequests
        public async Task<IActionResult> Index(int userId)
        {
            TempData["userId"] = userId;
            /*var requests = _context.FriendRequests.Where(m => m.Status == 1 && ((m.SenderId == userId) || (m.ReceiverId == userId))).Include(r => r.Sender).Include(r => r.Receiver);*/
            var requests = _context.FriendRequests.Where(m => (m.SenderId == userId) || (m.ReceiverId == userId)).Include(r => r.Sender).Include(r => r.Receiver);

            var notifications = _context.Notifications
                    .Include(n => n.User)
                    .Where(m => m.UserId == userId && m.NotificationType == 2).ToListAsync().Result;
            foreach (Notification nt in notifications)
            {
                _context.Remove(nt);
            }
            await _context.SaveChangesAsync();

            return View(requests);// await requests.ToListAsync());  //isos na min prepei na iparxei to tolist

            /*var saaSDBContext = _context.FriendRequests.Include(f => f.Receiver).Include(f => f.Sender);//.Where(m => m.Status ==1);
            return View(await saaSDBContext.ToListAsync());*/
        }

        // GET: FriendRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friendRequest = await _context.FriendRequests
                .Include(f => f.Receiver)
                .Include(f => f.Sender)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (friendRequest == null)
            {
                return NotFound();
            }

            return View(friendRequest);
        }

        // GET: FriendRequests/Create
        public IActionResult Create(int userId, int otherUserId)
        {
            ViewData["ReceiverId"] = otherUserId; /*new SelectList(_context.Users, "UserId", "UserId");*/
            ViewData["SenderId"] = userId;/*new SelectList(_context.Users, "UserId", "UserId");*/
            return View();
            /*FriendRequest request = new FriendRequest();
            request.SenderId = userId;
            request.ReceiverId = otherUserId;
            request.Status = 0;
            request.RequestDate = DateTime.Now;
            return RedirectToAction("Create", new { friendRequest = request });*/
        }

        // POST: FriendRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestId,SenderId,ReceiverId,RequestDate,Status")] FriendRequest friendRequest)
        {
            friendRequest.RequestId = 0;
            //friendRequest.RequestDate = DateTime.Now;
            /*if (ModelState.IsValid)
            {
                _context.Add(friendRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }*/
            _context.Add(friendRequest);
            await _context.SaveChangesAsync();

            Notification notification = new Notification();
            notification.IsRead = false;
            notification.UserId = friendRequest.ReceiverId;
            notification.SourceId = friendRequest.SenderId;
            notification.NotificationDate = DateTime.UtcNow;
            notification.NotificationType = 2;
            _context.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Users", new {userid = friendRequest.SenderId});

            ViewData["ReceiverId"] = friendRequest.ReceiverId;//new SelectList(_context.Users, "UserId", "UserId", friendRequest.ReceiverId);
            ViewData["SenderId"] = friendRequest.SenderId; //new SelectList(_context.Users, "UserId", "UserId", friendRequest.SenderId);
            return View(friendRequest);
        }

        // GET: FriendRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friendRequest = await _context.FriendRequests.FindAsync(id);
            if (friendRequest == null)
            {
                return NotFound();
            }
            ViewData["ReceiverId"] = new SelectList(_context.Users, "UserId", "UserId", friendRequest.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.Users, "UserId", "UserId", friendRequest.SenderId);
            return View(friendRequest);
        }

        // POST: FriendRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestId,SenderId,ReceiverId,RequestDate,Status")] FriendRequest friendRequest)
        {
            if (id != friendRequest.RequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(friendRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FriendRequestExists(friendRequest.RequestId))
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
            ViewData["ReceiverId"] = new SelectList(_context.Users, "UserId", "UserId", friendRequest.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.Users, "UserId", "UserId", friendRequest.SenderId);
            return View(friendRequest);
        }

        // GET: FriendRequests/Delete/5
        public async Task<IActionResult> Delete(int userId, int otherUserId)//int? id)
        {
            var friendRequest = await _context.FriendRequests.Include(f => f.Receiver).Include(f => f.Sender).FirstOrDefaultAsync( m => (m.SenderId == userId && m.ReceiverId == otherUserId) || (m.SenderId == otherUserId && m.ReceiverId == userId));
            TempData["userId"]= userId;
            /*return RedirectToAction("Delete", new {id = friendRequest.RequestId});*/
            /*var requestId = friendRequest.*/
            /*var id = friendRequest.RequestId;*/
            /*if (id == null)
            {
                return NotFound();
            }

            var friendRequest = await _context.FriendRequests
                .Include(f => f.Receiver)
                .Include(f => f.Sender)
                .FirstOrDefaultAsync(m => m.RequestId == id);*/
            if (friendRequest == null)
            {
                return NotFound();
            }

            return View(friendRequest);
        }

        // POST: FriendRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int requestId)
        {
            var friendRequest = await _context.FriendRequests.FindAsync(requestId);
            if (friendRequest != null)
            {
                _context.FriendRequests.Remove(friendRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Users", new { userid = TempData["userrId"] });
        }

        private bool FriendRequestExists(int id)
        {
            return _context.FriendRequests.Any(e => e.RequestId == id);
        }
    }
}
