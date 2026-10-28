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
    public class SalesChannelsController : Controller
    {
        private readonly VerleContext _context;

        public SalesChannelsController(VerleContext context)
        {
            _context = context;
        }

        // GET: SalesChannels
        public async Task<IActionResult> Index()
        {
            return View(await _context.SalesChannels.ToListAsync());
        }

        // GET: SalesChannels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesChannel = await _context.SalesChannels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salesChannel == null)
            {
                return NotFound();
            }

            return View(salesChannel);
        }

        // GET: SalesChannels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SalesChannels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] SalesChannel salesChannel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salesChannel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salesChannel);
        }

        // GET: SalesChannels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesChannel = await _context.SalesChannels.FindAsync(id);
            if (salesChannel == null)
            {
                return NotFound();
            }
            return View(salesChannel);
        }

        // POST: SalesChannels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] SalesChannel salesChannel)
        {
            if (id != salesChannel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesChannel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesChannelExists(salesChannel.Id))
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
            return View(salesChannel);
        }

        // GET: SalesChannels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesChannel = await _context.SalesChannels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salesChannel == null)
            {
                return NotFound();
            }

            return View(salesChannel);
        }

        // POST: SalesChannels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salesChannel = await _context.SalesChannels.FindAsync(id);
            if (salesChannel != null)
            {
                _context.SalesChannels.Remove(salesChannel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesChannelExists(int id)
        {
            return _context.SalesChannels.Any(e => e.Id == id);
        }
    }
}
