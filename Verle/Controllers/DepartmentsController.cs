using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Verle.Data;
using Verle.Models;

namespace Verle.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly VerleContext _context;

        public DepartmentsController(VerleContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();

            return View(departments);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            return View(new Department());
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name")] Department department)
        {
            department.Name = (department.Name ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(department.Name))
            {
                ModelState.AddModelError(
                    nameof(Department.Name),
                    "Введите название блока.");
            }

            if (!ModelState.IsValid)
            {
                return View(department);
            }

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Блок добавлен.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name")] Department model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            model.Name = (model.Name ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(
                    nameof(Department.Name),
                    "Введите название блока.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(x => x.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            department.Name = model.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Departments
                    .AnyAsync(x => x.Id == id))
                {
                    return NotFound();
                }

                throw;
            }

            TempData["SuccessMessage"] = "Изменения сохранены.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(x => x.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);

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
                    "Не удалось удалить блок. Возможно, он используется " +
                    "в других данных. Проверьте связанные записи.");

                return View("Delete", department);
            }

            TempData["SuccessMessage"] = "Блок удалён.";

            return RedirectToAction(nameof(Index));
        }
    }
}