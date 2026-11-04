using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Verle.Data;
using Verle.Models;

namespace Verle.Controllers
{
    public class DivisionsController : Controller
    {
        private readonly VerleContext _context;

        public DivisionsController(VerleContext context)
        {
            _context = context;
        }

        // GET: Divisions
        public async Task<IActionResult> Index()
        {
            var divisions = await _context.Divisions
                .AsNoTracking()
                .Include(x => x.Department)
                .OrderBy(x => x.Department!.Name)
                .ThenBy(x => x.Name)
                .ToListAsync();

            return View(divisions);
        }

        // GET: Divisions/Create
        public async Task<IActionResult> Create()
        {
            await LoadDepartmentsAsync();

            return View(new Division());
        }

        // POST: Divisions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,DepartmentId")] Division division)
        {
            division.Name = (division.Name ?? string.Empty).Trim();

            await ValidateDivisionAsync(division);
            await ValidateDivisionNameAsync(division);

            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync(division.DepartmentId);

                return View(division);
            }

            _context.Divisions.Add(division);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Отдел добавлен.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Divisions/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var division = await _context.Divisions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (division == null)
            {
                return NotFound();
            }

            await LoadDepartmentsAsync(division.DepartmentId);

            return View(division);
        }

        // POST: Divisions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,DepartmentId")] Division model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            model.Name = (model.Name ?? string.Empty).Trim();

            await ValidateDivisionAsync(model);
            await ValidateDivisionNameAsync(model);

            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync(model.DepartmentId);

                return View(model);
            }

            var division = await _context.Divisions
                .FirstOrDefaultAsync(x => x.Id == id);

            if (division == null)
            {
                return NotFound();
            }

            division.Name = model.Name;
            division.DepartmentId = model.DepartmentId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Divisions.AnyAsync(x => x.Id == id))
                {
                    return NotFound();
                }

                throw;
            }

            TempData["SuccessMessage"] = "Изменения сохранены.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Divisions/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var division = await _context.Divisions
                .AsNoTracking()
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (division == null)
            {
                return NotFound();
            }

            return View(division);
        }

        // POST: Divisions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var division = await _context.Divisions
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (division == null)
            {
                return NotFound();
            }

            _context.Divisions.Remove(division);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Не удалось удалить отдел. Возможно, он используется " +
                    "в других данных. Проверьте связанные записи.");

                return View("Delete", division);
            }

            TempData["SuccessMessage"] = "Отдел удалён.";

            return RedirectToAction(nameof(Index));
        }

        // Заполнение выпадающего списка блоков.
        private async Task LoadDepartmentsAsync(int? selectedId = null)
        {
            var departments = await _context.Departments
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .ToListAsync();

            ViewBag.Departments = new SelectList(
                departments,
                "Id",
                "Name",
                selectedId);
        }

        // Дополнительная серверная проверка.
        private async Task ValidateDivisionAsync(Division division)
        {
            if (string.IsNullOrWhiteSpace(division.Name) &&
                !HasFieldErrors(nameof(Division.Name)))
            {
                ModelState.AddModelError(
                    nameof(Division.Name),
                    "Введите название отдела.");
            }

            if (!HasFieldErrors(nameof(Division.DepartmentId)))
            {
                var departmentExists = await _context.Departments
                    .AnyAsync(x => x.Id == division.DepartmentId);

                if (!departmentExists)
                {
                    ModelState.AddModelError(
                        nameof(Division.DepartmentId),
                        "Выбранный блок не найден. Выберите другой блок.");
                }
            }
        }
        
        //Проверка уникальности при управлении отделами
        private async Task ValidateDivisionNameAsync(Division model)
        {
            model.Name = (model.Name ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                if (!ModelState.TryGetValue(
                        nameof(Division.Name), out var entry) ||
                    entry.Errors.Count == 0)
                {
                    ModelState.AddModelError(
                        nameof(Division.Name),
                        "Введите название отдела.");
                }

                return;
            }

            var nameExists = await _context.Divisions
                .AnyAsync(x =>
                    x.DepartmentId == model.DepartmentId &&
                    x.Name == model.Name &&
                    x.Id != model.Id);

            if (nameExists)
            {
                ModelState.AddModelError(
                    nameof(Division.Name),
                    "В выбранном блоке уже есть отдел с таким названием.");
            }
        }
        
        private bool HasFieldErrors(string fieldName)
        {
            return ModelState.TryGetValue(fieldName, out var entry)
                && entry.Errors.Count > 0;
        }
    }
}