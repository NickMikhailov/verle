using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Verle.Data;
using Verle.Models;
using Verle.ViewModels;

namespace Verle.Controllers
{

    public class CounterpartGroupsController : Controller
    {
        private readonly VerleContext _context;

        public CounterpartGroupsController(VerleContext context)
        {
            _context = context;
        }

        // GET: CounterpartGroups
        public async Task<IActionResult> Index()
        {
            var groups = await _context.CounterpartGroups
                .AsNoTracking()
                .Include(x => x.SalesChannelLink)
                    .ThenInclude(x => x.SalesChannel)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return View(groups);
        }

        // GET: CounterpartGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var counterpartGroup = await _context.CounterpartGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (counterpartGroup == null)
            {
                return NotFound();
            }

            return View(counterpartGroup);
        }

        // GET: CounterpartGroups/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CounterpartGroupFormViewModel();

            await LoadSalesChannelOptionsAsync(model);

            return View(model);
        }

        // POST: CounterpartGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CounterpartGroupFormViewModel model)
        {
            await ValidateSalesChannelAsync(model);

            model.Name = model.Name?.Trim() ?? string.Empty;

            if (!ModelState.IsValid)
            {
                await LoadSalesChannelOptionsAsync(model);

                return View(model);
            }

            var group = new CounterpartGroup
            {
                Name = model.Name
            };

            ApplySalesChannel(group, model);

            _context.CounterpartGroups.Add(group);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: CounterpartGroups/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var group = await _context.CounterpartGroups
                .AsNoTracking()
                .Include(x => x.SalesChannelLink)
                .FirstOrDefaultAsync(x => x.Id == id.Value);

            if (group is null)
            {
                return NotFound();
            }

            var model = new CounterpartGroupFormViewModel
            {
                Id = group.Id,
                Name = group.Name,
                SelectedSalesChannelId =
                    group.SalesChannelLink?.SalesChannelId
            };

            await LoadSalesChannelOptionsAsync(model);

            return View(model);
        }

        // POST: CounterpartGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CounterpartGroupFormViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            await ValidateSalesChannelAsync(model);

            model.Name = model.Name?.Trim() ?? string.Empty;

            if (!ModelState.IsValid)
            {
                await LoadSalesChannelOptionsAsync(model);

                return View(model);
            }

            var group = await _context.CounterpartGroups
                .Include(x => x.SalesChannelLink)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (group is null)
            {
                return NotFound();
            }

            group.Name = model.Name;

            ApplySalesChannel(group, model);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: CounterpartGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var counterpartGroup = await _context.CounterpartGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (counterpartGroup == null)
            {
                return NotFound();
            }

            return View(counterpartGroup);
        }

        // POST: CounterpartGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var counterpartGroup = await _context.CounterpartGroups.FindAsync(id);
            if (counterpartGroup != null)
            {
                _context.CounterpartGroups.Remove(counterpartGroup);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CounterpartGroupExists(int id)
        {
            return _context.CounterpartGroups.Any(e => e.Id == id);
        }

        private async Task LoadSalesChannelOptionsAsync(CounterpartGroupFormViewModel model)
        {
            model.SalesChannelOptions = await _context.SalesChannels
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToListAsync();
        }

        private async Task ValidateSalesChannelAsync(CounterpartGroupFormViewModel model)
        {
            if (!model.SelectedSalesChannelId.HasValue)
            {
                return;
            }

            var exists = await _context.SalesChannels
                .AnyAsync(x =>
                    x.Id == model.SelectedSalesChannelId.Value);

            if (!exists)
            {
                ModelState.AddModelError(
                    nameof(model.SelectedSalesChannelId),
                    "Выбранный канал продаж не существует.");
            }
        }

        private void ApplySalesChannel( CounterpartGroup group,
                                        CounterpartGroupFormViewModel model)
        {
            if (model.SelectedSalesChannelId.HasValue)
            {
                if (group.SalesChannelLink is null)
                {
                    group.SalesChannelLink =
                        new CounterpartGroupSalesChannelLink
                        {
                            SalesChannelId =
                                model.SelectedSalesChannelId.Value
                        };
                }
                else
                {
                    group.SalesChannelLink.SalesChannelId =
                        model.SelectedSalesChannelId.Value;
                }
            }
            else if (group.SalesChannelLink is not null)
            {
                _context.CounterpartGroupSalesChannelLinks.Remove(
                    group.SalesChannelLink);

                group.SalesChannelLink = null;
            }
        }
    }
}
