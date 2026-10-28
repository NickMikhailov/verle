using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Verle.Data;
using Verle.Models;

namespace Verle.Controllers
{
    public class BrandGroupsController : Controller
    {
        private readonly VerleContext _context;

        public BrandGroupsController(VerleContext context)
        {
            _context = context;
        }

        // GET: BrandGroups
        public async Task<IActionResult> Index()
        {
            return View(await _context.BrandGroups.ToListAsync());
        }

        // GET: BrandGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brandGroup = await _context.BrandGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brandGroup == null)
            {
                return NotFound();
            }

            return View(brandGroup);
        }

        // GET: BrandGroups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BrandGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] BrandGroup brandGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brandGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brandGroup);
        }

        // GET: BrandGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brandGroup = await _context.BrandGroups.FindAsync(id);
            if (brandGroup == null)
            {
                return NotFound();
            }
            return View(brandGroup);
        }

        // POST: BrandGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] BrandGroup brandGroup)
        {
            if (id != brandGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brandGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandGroupExists(brandGroup.Id))
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
            return View(brandGroup);
        }

        // GET: BrandGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brandGroup = await _context.BrandGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brandGroup == null)
            {
                return NotFound();
            }

            return View(brandGroup);
        }

        // POST: BrandGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brandGroup = await _context.BrandGroups.FindAsync(id);
            if (brandGroup != null)
            {
                _context.BrandGroups.Remove(brandGroup);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrandGroupExists(int id)
        {
            return _context.BrandGroups.Any(e => e.Id == id);
        }
    }
}
