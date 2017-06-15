using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OpenRacingTelemetry.Data;
using OpenRacingTelemetry.Models;

namespace OpenRacingTelemetry.Controllers
{
    public class RacesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RacesController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Races
        public async Task<IActionResult> Index()
        {
            return View(await _context.Race.ToListAsync());
        }

        // GET: Races/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var race = await _context.Race
                .SingleOrDefaultAsync(m => m.RaceId == id);
            if (race == null)
            {
                return NotFound();
            }
            race.Records = _context.Records.Where(x => x.Race == race).ToList();
            return View(race);
        }

        // GET: Races/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Races/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RaceId,Name,Description,Weather,Date")] Race race)
        {
            if (ModelState.IsValid)
            {
                _context.Add(race);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(race);
        }

        // GET: Races/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var race = await _context.Race.SingleOrDefaultAsync(m => m.RaceId == id);
            if (race == null)
            {
                return NotFound();
            }
            return View(race);
        }

        // POST: Races/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RaceId,Name,Description,Weather,Date")] Race race)
        {
            if (id != race.RaceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(race);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RaceExists(race.RaceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(race);
        }

        // GET: Races/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var race = await _context.Race
                .SingleOrDefaultAsync(m => m.RaceId == id);
            if (race == null)
            {
                return NotFound();
            }

            return View(race);
        }

        // POST: Races/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var race = await _context.Race.SingleOrDefaultAsync(m => m.RaceId == id);
            _context.Race.Remove(race);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool RaceExists(int id)
        {
            return _context.Race.Any(e => e.RaceId == id);
        }
    }
}
