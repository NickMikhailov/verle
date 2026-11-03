using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Verle.Data;
using Verle.Models;
using Verle.ViewModels;

namespace Verle.Controllers
{
    public class CounterpartsController : Controller
    {
        private readonly VerleContext _context;

        public CounterpartsController(VerleContext context)
        {
            _context = context;
        }

        // GET: Counterparts
        public async Task<IActionResult> Index()
        {
            var counterparts = await _context.Counterparts
                .AsNoTracking()
                .Include(x => x.CounterpartGroupLink)
                    .ThenInclude(x => x!.CounterpartGroup)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return View(counterparts);
        }

        // GET: Counterparts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var counterpart = await _context.Counterparts
                .AsNoTracking()
                .Include(x => x.CounterpartGroupLink)
                    .ThenInclude(x => x!.CounterpartGroup)
                .FirstOrDefaultAsync(x => x.Id == id.Value);

            if (counterpart is null)
            {
                return NotFound();
            }

            return View(counterpart);
        }

        // GET: Counterparts/Create
        [HttpGet]
        public async Task<IActionResult> Create(
            string? id1C,
            string? name)
        {
            var model = new CounterpartFormViewModel
            {
                Id1C = id1C?.Trim() ?? string.Empty,
                Name = name?.Trim() ?? string.Empty
            };

            await LoadCounterpartGroupOptionsAsync(model);

            return View(model);
        }

        // POST: Counterparts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CounterpartFormViewModel model)
        {
            NormalizeModel(model);

            await ValidateCounterpartGroupAsync(model);

            ValidateRequiredFields(model);

            var id1CExists = await _context.Counterparts
                .AnyAsync(x => x.Id1C == model.Id1C);

            if (id1CExists)
            {
                ModelState.AddModelError(
                    nameof(model.Id1C),
                    "Такой ID из 1С уже существует.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCounterpartGroupOptionsAsync(model);

                return View(model);
            }

            var counterpart = new Counterpart
            {
                Id1C = model.Id1C,
                INN = model.INN,
                Name = model.Name
            };

            ApplyCounterpartGroup(counterpart, model);

            _context.Counterparts.Add(counterpart);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    nameof(model.Id1C),
                    "Не удалось сохранить контрагента. " +
                    "Возможно, такой ID уже существует.");

                await LoadCounterpartGroupOptionsAsync(model);

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Counterparts/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var counterpart = await _context.Counterparts
                .AsNoTracking()
                .Include(x => x.CounterpartGroupLink)
                .FirstOrDefaultAsync(x => x.Id == id.Value);

            if (counterpart is null)
            {
                return NotFound();
            }

            var model = new CounterpartFormViewModel
            {
                Id = counterpart.Id,
                Id1C = counterpart.Id1C,
                INN = counterpart.INN,
                Name = counterpart.Name,
                SelectedCounterpartGroupId =
                    counterpart.CounterpartGroupLink?
                        .CounterpartGroupId
            };

            await LoadCounterpartGroupOptionsAsync(model);

            return View(model);
        }

        // POST: Counterparts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            CounterpartFormViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            NormalizeModel(model);

            await ValidateCounterpartGroupAsync(model);

            ValidateRequiredFields(model);

            var id1CExists = await _context.Counterparts
                .AnyAsync(x =>
                    x.Id1C == model.Id1C &&
                    x.Id != model.Id);

            if (id1CExists)
            {
                ModelState.AddModelError(
                    nameof(model.Id1C),
                    "Такой ID из 1С уже используется другим контрагентом.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCounterpartGroupOptionsAsync(model);

                return View(model);
            }

            // Здесь нельзя использовать AsNoTracking(),
            // так как объект будет изменяться и сохраняться.
            var counterpart = await _context.Counterparts
                .Include(x => x.CounterpartGroupLink)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (counterpart is null)
            {
                return NotFound();
            }

            counterpart.Id1C = model.Id1C;
            counterpart.INN = model.INN;
            counterpart.Name = model.Name;

            ApplyCounterpartGroup(counterpart, model);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.Counterparts
                    .AnyAsync(x => x.Id == id);

                if (!exists)
                {
                    return NotFound();
                }

                ModelState.AddModelError(
                    string.Empty,
                    "Контрагент был изменён другим пользователем. " +
                    "Обновите страницу и повторите попытку.");

                await LoadCounterpartGroupOptionsAsync(model);

                return View(model);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    nameof(model.Id1C),
                    "Не удалось сохранить изменения. " +
                    "Возможно, такой ID уже существует.");

                await LoadCounterpartGroupOptionsAsync(model);

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Counterparts/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var counterpart = await _context.Counterparts
                .AsNoTracking()
                .Include(x => x.CounterpartGroupLink)
                    .ThenInclude(x => x!.CounterpartGroup)
                .FirstOrDefaultAsync(x => x.Id == id.Value);

            if (counterpart is null)
            {
                return NotFound();
            }

            return View(counterpart);
        }

        // POST: Counterparts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var counterpart = await _context.Counterparts
                .Include(x => x.CounterpartGroupLink)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (counterpart is null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Явно удаляем связь с группой.
            // Саму группу не удаляем.
            if (counterpart.CounterpartGroupLink is not null)
            {
                _context.CounterpartCounterpartGroupLinks.Remove(
                    counterpart.CounterpartGroupLink);
            }

            _context.Counterparts.Remove(counterpart);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Контрагента нельзя удалить, потому что он " +
                    "используется в связанных записях.");

                var counterpartForView = await _context.Counterparts
                    .AsNoTracking()
                    .Include(x => x.CounterpartGroupLink)
                        .ThenInclude(x => x!.CounterpartGroup)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (counterpartForView is null)
                {
                    return RedirectToAction(nameof(Index));
                }

                return View("Delete", counterpartForView);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Counterparts/CreateFromImport
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromImport(
            [Bind("Id1C,Name,SelectedCounterpartGroupId")]
    CounterpartFormViewModel model,
            CancellationToken cancellationToken)
        {
            NormalizeModel(model);

            ValidateRequiredFields(model);

            await ValidateCounterpartGroupAsync(model);

            var id1CExists = await _context.Counterparts
                .AnyAsync(
                    x => x.Id1C == model.Id1C,
                    cancellationToken);

            if (id1CExists)
            {
                ModelState.AddModelError(
                    nameof(model.Id1C),
                    "Такой ID из 1С уже существует. " +
                    "Повторно проверьте файл продаж: " +
                    "возможно, контрагент уже был добавлен.");
            }

            if (!ModelState.IsValid)
            {
                return CounterpartImportValidationError();
            }

            var counterpart = new Counterpart
            {
                Id1C = model.Id1C,
                Name = model.Name,

                // В файле импорта ИНН не передаётся.
                INN = null
            };

            // Используем существующую логику связи с группой.
            ApplyCounterpartGroup(counterpart, model);

            _context.Counterparts.Add(counterpart);

            try
            {
                // Контрагент и связь с группой сохраняются
                // одним вызовом SaveChangesAsync.
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Не удалось сохранить контрагента. " +
                    "Возможно, такой ID уже добавлен другим пользователем " +
                    "или выбранная группа была изменена либо удалена. " +
                    "Повторно проверьте файл. Если ошибка сохраняется, " +
                    "обратитесь к администратору.");

                return CounterpartImportValidationError();
            }

            return Json(new
            {
                success = true,
                id = counterpart.Id
            });
        }

        private IActionResult CounterpartImportValidationError()
        {
            var errors = ModelState
                .Where(entry => entry.Value is { Errors.Count: > 0 })
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value!.Errors
                        .Select(error =>
                            string.IsNullOrWhiteSpace(error.ErrorMessage)
                                ? "Некорректное значение поля."
                                : error.ErrorMessage)
                        .Distinct()
                        .ToArray());

            return BadRequest(new
            {
                success = false,
                errors
            });
        }
        private async Task LoadCounterpartGroupOptionsAsync(
            CounterpartFormViewModel model)
        {
            model.CounterpartGroupOptions =
                await _context.CounterpartGroups
                    .AsNoTracking()
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.Id)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    })
                    .ToListAsync();
        }

        private async Task ValidateCounterpartGroupAsync(
            CounterpartFormViewModel model)
        {
            if (!model.SelectedCounterpartGroupId.HasValue)
            {
                return;
            }

            var exists = await _context.CounterpartGroups
                .AnyAsync(x =>
                    x.Id == model.SelectedCounterpartGroupId.Value);

            if (!exists)
            {
                ModelState.AddModelError(
                    nameof(model.SelectedCounterpartGroupId),
                    "Выбранная группа контрагентов не существует.");
            }
        }

        private void ApplyCounterpartGroup(
            Counterpart counterpart,
            CounterpartFormViewModel model)
        {
            if (model.SelectedCounterpartGroupId is int groupId)
            {
                if (counterpart.CounterpartGroupLink is null)
                {
                    counterpart.CounterpartGroupLink =
                        new CounterpartCounterpartGroupLink
                        {
                            Counterpart = counterpart,
                            CounterpartGroupId = groupId
                        };
                }
                else
                {
                    counterpart.CounterpartGroupLink
                        .CounterpartGroupId = groupId;
                }
            }
            else if (counterpart.CounterpartGroupLink is not null)
            {
                _context.CounterpartCounterpartGroupLinks.Remove(
                    counterpart.CounterpartGroupLink);

                counterpart.CounterpartGroupLink = null;
            }
        }

        private static void NormalizeModel(
            CounterpartFormViewModel model)
        {
            model.Id1C = model.Id1C?.Trim() ?? string.Empty;

            model.INN = string.IsNullOrWhiteSpace(model.INN)
                ? null
                : model.INN.Trim();

            model.Name = model.Name?.Trim() ?? string.Empty;
        }

        private void ValidateRequiredFields(
            CounterpartFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Id1C) &&
                !HasModelErrors(nameof(model.Id1C)))
            {
                ModelState.AddModelError(
                    nameof(model.Id1C),
                    "Введите ID контрагента из 1С.");
            }

            if (string.IsNullOrWhiteSpace(model.Name) &&
                !HasModelErrors(nameof(model.Name)))
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "Введите наименование контрагента.");
            }
        }

        private bool HasModelErrors(string key)
        {
            return ModelState[key]?.Errors.Count > 0;
        }
    }
}