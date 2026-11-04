using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Verle.Data;
using Verle.Models;

namespace Verle.Controllers
{
    public class PositionsController : Controller
    {
        private readonly VerleContext _context;

        public PositionsController(VerleContext context)
        {
            _context = context;
        }

        // GET: Positions
        public async Task<IActionResult> Index()
        {
            var positions = await _context.Positions
                .AsNoTracking()
                .Include(x => x.Division)
                    .ThenInclude(x => x.Department)
                .OrderBy(x => x.Division.Department.Name)
                .ThenBy(x => x.Division.Name)
                .ThenBy(x => x.Name)
                .ToListAsync();

            return View(positions);
        }

        // GET: Positions/Create
        public async Task<IActionResult> Create()
        {
            var model = new PositionFormViewModel();

            await LoadOptionsAsync(model);

            return View(model);
        }

        // POST: Positions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,DepartmentId,DivisionId")]
            PositionFormViewModel model)
        {
            model.Name = (model.Name ?? string.Empty).Trim();

            await ValidateFormAsync(model);

            if (!ModelState.IsValid)
            {
                await LoadOptionsAsync(model);
                return View(model);
            }

            var position = new Position
            {
                Name = model.Name,
                DivisionId = model.DivisionId!.Value
            };

            _context.Positions.Add(position);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Не удалось сохранить должность. Возможно, такая " +
                    "должность уже добавлена другим пользователем " +
                    "или выбранный отдел был удалён. " +
                    "Проверьте данные и повторите попытку.");

                await LoadOptionsAsync(model);
                return View(model);
            }

            TempData["SuccessMessage"] = "Должность добавлена.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Positions/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var position = await _context.Positions
                .AsNoTracking()
                .Include(x => x.Division)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (position == null)
            {
                return NotFound();
            }

            var model = new PositionFormViewModel
            {
                Id = position.Id,
                Name = position.Name,
                DepartmentId = position.Division.DepartmentId,
                DivisionId = position.DivisionId
            };

            await LoadOptionsAsync(model);

            return View(model);
        }

        // POST: Positions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,DepartmentId,DivisionId")]
            PositionFormViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .FirstOrDefaultAsync(x => x.Id == id);

            if (position == null)
            {
                return NotFound();
            }

            model.Name = (model.Name ?? string.Empty).Trim();

            await ValidateFormAsync(model);

            if (!ModelState.IsValid)
            {
                await LoadOptionsAsync(model);
                return View(model);
            }

            position.Name = model.Name;
            position.DivisionId = model.DivisionId!.Value;

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
                    "Не удалось сохранить изменения. Возможно, такая " +
                    "должность уже существует в выбранном отделе " +
                    "или связанные данные изменились. " +
                    "Проверьте данные и повторите попытку.");

                await LoadOptionsAsync(model);
                return View(model);
            }

            TempData["SuccessMessage"] = "Изменения сохранены.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Positions/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var position = await _context.Positions
                .AsNoTracking()
                .Include(x => x.Division)
                    .ThenInclude(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (position == null)
            {
                return NotFound();
            }

            return View(position);
        }

        // POST: Positions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var position = await _context.Positions
                .Include(x => x.Division)
                    .ThenInclude(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (position == null)
            {
                return NotFound();
            }

            _context.Positions.Remove(position);

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
                    "Не удалось удалить должность. Возможно, она " +
                    "используется в других данных.");

                return View("Delete", position);
            }

            TempData["SuccessMessage"] = "Должность удалена.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Positions/GetDivisions?departmentId=1
        // Используется зависимым списком отделов.
        [HttpGet]
        public async Task<IActionResult> GetDivisions(int departmentId)
        {
            var divisions = await _context.Divisions
                .AsNoTracking()
                .Where(x => x.DepartmentId == departmentId)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new
                {
                    id = x.Id,
                    name = x.Name
                })
                .ToListAsync();

            return Json(divisions);
        }

        private async Task LoadOptionsAsync(
            PositionFormViewModel model)
        {
            model.DepartmentOptions = await _context.Departments
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToListAsync();

            model.DivisionOptions = new List<SelectListItem>();

            if (model.DepartmentId.HasValue)
            {
                model.DivisionOptions = await _context.Divisions
                    .AsNoTracking()
                    .Where(x =>
                        x.DepartmentId == model.DepartmentId.Value)
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.Id)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    })
                    .ToListAsync();
            }
        }

        private async Task ValidateFormAsync(
            PositionFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                AddFieldError(
                    nameof(model.Name),
                    "Введите название должности.");
            }

            if (model.DepartmentId is > 0)
            {
                var departmentExists = await _context.Departments
                    .AnyAsync(x => x.Id == model.DepartmentId.Value);

                if (!departmentExists)
                {
                    AddFieldError(
                        nameof(model.DepartmentId),
                        "Выбранный блок не найден.");
                }
            }

            if (model.DivisionId is > 0 &&
                model.DepartmentId is > 0)
            {
                var divisionMatches = await _context.Divisions
                    .AnyAsync(x =>
                        x.Id == model.DivisionId.Value &&
                        x.DepartmentId == model.DepartmentId.Value);

                if (!divisionMatches)
                {
                    AddFieldError(
                        nameof(model.DivisionId),
                        "Выбранный отдел не относится к указанному " +
                        "блоку или был удалён.");
                }

                if (!string.IsNullOrWhiteSpace(model.Name))
                {
                    var nameExists = await _context.Positions
                        .AnyAsync(x =>
                            x.DivisionId == model.DivisionId.Value &&
                            x.Name == model.Name &&
                            x.Id != model.Id);

                    if (nameExists)
                    {
                        AddFieldError(
                            nameof(model.Name),
                            "В этом отделе уже есть должность " +
                            "с таким названием.");
                    }
                }
            }
        }

        private void AddFieldError(string field, string message)
        {
            // Не дублируем сообщение, если поле уже не прошло
            // проверку атрибутов или привязку значения.
            if (ModelState.TryGetValue(field, out var entry) &&
                entry.Errors.Count > 0)
            {
                return;
            }

            ModelState.AddModelError(field, message);
        }
    }
}