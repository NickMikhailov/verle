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
    public class SkuClassesController : Controller
    {
        private readonly VerleContext _context;

        public SkuClassesController(VerleContext context)
        {
            _context = context;
        }

        // GET: SkuClasses
        public async Task<IActionResult> Index()
        {
            return View(await _context.SkuClasses.ToListAsync());
        }

        // GET: SkuClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skuClass = await _context.SkuClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (skuClass == null)
            {
                return NotFound();
            }

            return View(skuClass);
        }

        // GET: SkuClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SkuClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] SkuClass skuClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(skuClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(skuClass);
        }

        // GET: SkuClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skuClass = await _context.SkuClasses.FindAsync(id);
            if (skuClass == null)
            {
                return NotFound();
            }
            return View(skuClass);
        }

        // POST: SkuClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] SkuClass skuClass)
        {
            if (id != skuClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(skuClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SkuClassExists(skuClass.Id))
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
            return View(skuClass);
        }

        // GET: SkuClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skuClass = await _context.SkuClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (skuClass == null)
            {
                return NotFound();
            }

            return View(skuClass);
        }

        // POST: SkuClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var skuClass = await _context.SkuClasses.FindAsync(id);
            if (skuClass != null)
            {
                _context.SkuClasses.Remove(skuClass);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SkuClassExists(int id)
        {
            return _context.SkuClasses.Any(e => e.Id == id);
        }
    }
}
