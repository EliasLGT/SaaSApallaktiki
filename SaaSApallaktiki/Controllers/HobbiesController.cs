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
    public class HobbiesController : Controller
    {
        private readonly SaaSDBContext _context;

        public HobbiesController(SaaSDBContext context)
        {
            _context = context;
        }

        // GET: Hobbies
        public async Task<IActionResult> Index(int userId)//ICollection<Hobby> hobbiess, int userrId)
        {
            //var user1 = _context.Users.Include(r => r.Hobbies).FirstOrDefaultAsync(m => m.UserId == userId).Result;
            
            //user1.Hobbies


            /*var user2 = _context.Users.FirstOrDefaultAsync(m => m.UserId == userId);
            //var hobies = _context.Hobbies.ToListAsync();
            *//*foreach (var hobie in hobies)
            {
                if(hobie.UserId == userId)
                {
                    user2.Result.Hobbies.Add(hobie);
                }
            }*//*
            var hobbies = user2.Result.Hobbies;
            if (hobbies.Count == 0)
            {
                var tempHobby = await _context.Hobbies.FirstOrDefaultAsync(m => m.HobbyId == 3);
                _context.Users.FirstAsync(m => m.UserId == userId).Result.Hobbies.Add(tempHobby);
                await _context.SaveChangesAsync();
            }*/
            TempData["userId"] = userId;
            //hobbies.First().
            //return View(hobbies);
            return View(await _context.Hobbies.Include(r => r.Users).ToListAsync());
        }

        public async Task<IActionResult> RemoveHobby(int userId, int hobbyId)//ICollection<Hobby> hobbiess, int userrId)
        {
            var targetedHobby = await _context.Hobbies.FirstOrDefaultAsync(m => m.HobbyId == hobbyId);
            _context.Users.Include(r=>r.Hobbies).FirstOrDefaultAsync(m => m.UserId == userId).Result.Hobbies.Remove(targetedHobby);
            //_context.Users.FirstOrDefaultAsync(m => m.UserId == userId).Result.Hobbies.Remove(targetedHobby);
            await _context.SaveChangesAsync();

            //var user1 = _context.Users.Include(r => r.Hobbies).FirstOrDefaultAsync(m => m.UserId == userId).Result;
            //user1.Hobbies

            return RedirectToAction("Index", new {userId = userId});
        }
        public async Task<IActionResult> AddHobby(int userId, int hobbyId)//ICollection<Hobby> hobbiess, int userrId)
        {
            var targetedHobby = await _context.Hobbies.FirstOrDefaultAsync(m => m.HobbyId == hobbyId);
            _context.Users.FirstOrDefaultAsync(m => m.UserId == userId).Result.Hobbies.Add(targetedHobby);
            await _context.SaveChangesAsync();

            //var user1 = _context.Users.Include(r => r.Hobbies).FirstOrDefaultAsync(m => m.UserId == userId).Result;
            //user1.Hobbies

            return RedirectToAction("Index", new { userId = userId });
        }











        // GET: Hobbies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hobby = await _context.Hobbies
                .FirstOrDefaultAsync(m => m.HobbyId == id);
            if (hobby == null)
            {
                return NotFound();
            }

            return View(hobby);
        }

        // GET: Hobbies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Hobbies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HobbyId,HobbyName")] Hobby hobby)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hobby);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hobby);
        }

        // GET: Hobbies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hobby = await _context.Hobbies.FindAsync(id);
            if (hobby == null)
            {
                return NotFound();
            }
            return View(hobby);
        }

        // POST: Hobbies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HobbyId,HobbyName")] Hobby hobby)
        {
            if (id != hobby.HobbyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hobby);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HobbyExists(hobby.HobbyId))
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
            return View(hobby);
        }

        // GET: Hobbies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hobby = await _context.Hobbies
                .FirstOrDefaultAsync(m => m.HobbyId == id);
            if (hobby == null)
            {
                return NotFound();
            }

            return View(hobby);
        }

        // POST: Hobbies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hobby = await _context.Hobbies.FindAsync(id);
            if (hobby != null)
            {
                _context.Hobbies.Remove(hobby);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HobbyExists(int id)
        {
            return _context.Hobbies.Any(e => e.HobbyId == id);
        }
    }
}
