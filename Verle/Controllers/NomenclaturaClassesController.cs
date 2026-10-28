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
    public class NomenclaturaClassesController : Controller
    {
        private readonly VerleContext _context;

        public NomenclaturaClassesController(VerleContext context)
        {
            _context = context;
        }

        // GET: NomenclaturaClasses
        public async Task<IActionResult> Index()
        {
            return View(await _context.NomenclaturaClasses.ToListAsync());
        }

        // GET: NomenclaturaClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nomenclaturaClass = await _context.NomenclaturaClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nomenclaturaClass == null)
            {
                return NotFound();
            }

            return View(nomenclaturaClass);
        }

        // GET: NomenclaturaClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NomenclaturaClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] NomenclaturaClass nomenclaturaClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nomenclaturaClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nomenclaturaClass);
        }

        // GET: NomenclaturaClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nomenclaturaClass = await _context.NomenclaturaClasses.FindAsync(id);
            if (nomenclaturaClass == null)
            {
                return NotFound();
            }
            return View(nomenclaturaClass);
        }

        // POST: NomenclaturaClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] NomenclaturaClass nomenclaturaClass)
        {
            if (id != nomenclaturaClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nomenclaturaClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NomenclaturaClassExists(nomenclaturaClass.Id))
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
            return View(nomenclaturaClass);
        }

        // GET: NomenclaturaClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nomenclaturaClass = await _context.NomenclaturaClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nomenclaturaClass == null)
            {
                return NotFound();
            }

            return View(nomenclaturaClass);
        }

        // POST: NomenclaturaClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nomenclaturaClass = await _context.NomenclaturaClasses.FindAsync(id);
            if (nomenclaturaClass != null)
            {
                _context.NomenclaturaClasses.Remove(nomenclaturaClass);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NomenclaturaClassExists(int id)
        {
            return _context.NomenclaturaClasses.Any(e => e.Id == id);
        }
    }
}
