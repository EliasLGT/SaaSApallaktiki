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
    public class TeamsController : Controller
    {
        private readonly SaaSDBContext _context;

        public TeamsController(SaaSDBContext context)
        {
            _context = context;
        }

        // GET: Teams
        public async Task<IActionResult> Index(int userId)
        {
            return View(await _context.Teams.ToListAsync());
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .FirstOrDefaultAsync(m => m.TeamId == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create(int userId)
        {
            TempData["userId"] = userId;
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TeamId,TeamName")] Team teaam)
        {
            var sameNameTeam = _context.Teams.Where(m => m.TeamName == teaam.TeamName);
            if (sameNameTeam.Any())
            {
                int userId = (int)TempData["userrId"];
                TempData["userId"] = userId;
                TempData["sameNameTeam"] = true;
                return View(teaam);
            }

            if (ModelState.IsValid)
            {
                while (true)
                {
                    teaam.TeamId = 0;
                    _context.Add(teaam);
                    await _context.SaveChangesAsync();

                    //Check if there is a user with the same Primary Key
                    string teamName = teaam.TeamName;
                    var teamm = await _context.Teams.Where(r => r.TeamName == teamName).ToListAsync();

                    var team = _context.Users.Where(m => m.UserId == teamm.First().TeamId);
                    if (!team.Any())
                    {
                        int userID = (int)TempData["userrId"];
                        TeamMember teamMember = new TeamMember();
                        teamMember.TeamId = teamm.First().TeamId; ///////////////////// Na simpliroso!!!!!!
                        teamMember.Status = 1;
                        teamMember.UserId = userID;
                        _context.Add(teamMember);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index", "TeamMembers", new { userId = userID });
                    }
                    _context.Remove(teaam);
                    await _context.SaveChangesAsync();
                }

                /*_context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));*/
            }
            return View(teaam);
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TeamId,TeamName")] Team team)
        {
            if (id != team.TeamId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.TeamId))
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
            return View(team);
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .FirstOrDefaultAsync(m => m.TeamId == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                _context.Teams.Remove(team);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.TeamId == id);
        }
    }
}
