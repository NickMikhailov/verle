using System;
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
    public class EmployeesController : Controller
    {
        private readonly VerleContext _context;

        public EmployeesController(VerleContext context)
        {
            _context = context;
        }

        // Список сотрудников
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .AsNoTracking()
                .Include(x => x.Position)
                    .ThenInclude(x => x.Division)
                        .ThenInclude(x => x.Department)
                .OrderBy(x => x.Surname)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Patronymic)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return View(employees);
        }

        // Форма создания
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new EmployeeFormViewModel();

            await PopulateListsAsync(model);

            return View(model);
        }

        // Создание
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            EmployeeFormViewModel model)
        {
            NormalizeNames(model);

            await ValidateSelectionAsync(model);

            if (!ModelState.IsValid)
            {
                await PopulateListsAsync(model);
                return View(model);
            }

            var employee = new Employee
            {
                Surname = model.Surname,
                Name = model.Name,
                Patronymic = model.Patronymic,
                HireDate = model.HireDate!.Value,
                PositionId = model.PositionId!.Value
            };

            _context.Employees.Add(employee);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Не удалось сохранить сотрудника. " +
                    "Возможно, выбранная должность была удалена. " +
                    "Проверьте данные и повторите попытку.");

                await PopulateListsAsync(model);
                return View(model);
            }

            TempData["Success"] = "Сотрудник добавлен.";

            return RedirectToAction(nameof(Index));
        }

        // Форма редактирования
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .Include(x => x.Position)
                    .ThenInclude(x => x.Division)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
                return NotFound();

            var model = new EmployeeFormViewModel
            {
                Id = employee.Id,
                DepartmentId = employee.Position.Division.DepartmentId,
                DivisionId = employee.Position.DivisionId,
                PositionId = employee.PositionId,
                Surname = employee.Surname,
                Name = employee.Name,
                Patronymic = employee.Patronymic,
                HireDate = employee.HireDate
            };

            await PopulateListsAsync(model);

            return View(model);
        }

        // Редактирование
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            EmployeeFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
                return NotFound();

            NormalizeNames(model);

            await ValidateSelectionAsync(model);

            if (!ModelState.IsValid)
            {
                await PopulateListsAsync(model);
                return View(model);
            }

            employee.Surname = model.Surname;
            employee.Name = model.Name;
            employee.Patronymic = model.Patronymic;
            employee.HireDate = model.HireDate!.Value;
            employee.PositionId = model.PositionId!.Value;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.Employees
                    .AnyAsync(x => x.Id == id);

                if (!exists)
                    return NotFound();

                ModelState.AddModelError(
                    string.Empty,
                    "Данные сотрудника были изменены. " +
                    "Обновите страницу и повторите попытку.");

                await PopulateListsAsync(model);
                return View(model);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Не удалось сохранить изменения. " +
                    "Проверьте выбранную должность и повторите попытку.");

                await PopulateListsAsync(model);
                return View(model);
            }

            TempData["Success"] = "Данные сотрудника обновлены.";

            return RedirectToAction(nameof(Index));
        }

        // Подтверждение удаления
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .Include(x => x.Position)
                    .ThenInclude(x => x.Division)
                        .ThenInclude(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // Удаление
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees
                .Include(x => x.Position)
                    .ThenInclude(x => x.Division)
                        .ThenInclude(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
                return NotFound();

            _context.Employees.Remove(employee);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Не удалось удалить сотрудника. " +
                    "Возможно, с ним уже связаны другие записи.");

                return View("Delete", employee);
            }

            TempData["Success"] = "Сотрудник удалён.";

            return RedirectToAction(nameof(Index));
        }

        // Отделы только выбранного блока
        [HttpGet]
        public async Task<IActionResult> GetDivisions(int departmentId)
        {
            var divisions = await _context.Divisions
                .AsNoTracking()
                .Where(x => x.DepartmentId == departmentId)
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    id = x.Id,
                    name = x.Name
                })
                .ToListAsync();

            return Json(divisions);
        }

        // Должности только выбранного отдела в выбранном блоке
        [HttpGet]
        public async Task<IActionResult> GetPositions(
            int departmentId,
            int divisionId)
        {
            var positions = await _context.Positions
                .AsNoTracking()
                .Where(x =>
                    x.DivisionId == divisionId &&
                    x.Division.DepartmentId == departmentId)
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    id = x.Id,
                    name = x.Name
                })
                .ToListAsync();

            return Json(positions);
        }

        private static void NormalizeNames(
            EmployeeFormViewModel model)
        {
            model.Surname = (model.Surname ?? string.Empty).Trim();
            model.Name = (model.Name ?? string.Empty).Trim();

            model.Patronymic =
                string.IsNullOrWhiteSpace(model.Patronymic)
                    ? null
                    : model.Patronymic.Trim();
        }

        // Серверная проверка всей цепочки
        private async Task ValidateSelectionAsync(
            EmployeeFormViewModel model)
        {
            if (!model.DepartmentId.HasValue ||
                model.DepartmentId.Value <= 0)
            {
                return;
            }

            var departmentExists = await _context.Departments
                .AnyAsync(x => x.Id == model.DepartmentId.Value);

            if (!departmentExists)
            {
                ModelState.AddModelError(
                    nameof(model.DepartmentId),
                    "Выбранный блок не существует.");

                return;
            }

            if (!model.DivisionId.HasValue ||
                model.DivisionId.Value <= 0)
            {
                return;
            }

            var divisionExists = await _context.Divisions
                .AnyAsync(x =>
                    x.Id == model.DivisionId.Value &&
                    x.DepartmentId == model.DepartmentId.Value);

            if (!divisionExists)
            {
                ModelState.AddModelError(
                    nameof(model.DivisionId),
                    "Выбранный отдел не относится к указанному блоку.");

                return;
            }

            if (!model.PositionId.HasValue ||
                model.PositionId.Value <= 0)
            {
                return;
            }

            var positionExists = await _context.Positions
                .AnyAsync(x =>
                    x.Id == model.PositionId.Value &&
                    x.DivisionId == model.DivisionId.Value &&
                    x.Division.DepartmentId == model.DepartmentId.Value);

            if (!positionExists)
            {
                ModelState.AddModelError(
                    nameof(model.PositionId),
                    "Выбранная должность не относится к указанному отделу.");
            }
        }

        // Заполнение списков для создания, редактирования
        // и повторного показа формы при ошибках
        private async Task PopulateListsAsync(
            EmployeeFormViewModel model)
        {
            var departments = await _context.Departments
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new { x.Id, x.Name })
                .ToListAsync();

            model.Departments = departments
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();

            model.Divisions.Clear();
            model.Positions.Clear();

            if (model.DepartmentId.HasValue)
            {
                var divisions = await _context.Divisions
                    .AsNoTracking()
                    .Where(x =>
                        x.DepartmentId == model.DepartmentId.Value)
                    .OrderBy(x => x.Name)
                    .Select(x => new { x.Id, x.Name })
                    .ToListAsync();

                model.Divisions = divisions
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    })
                    .ToList();
            }

            if (model.DepartmentId.HasValue &&
                model.DivisionId.HasValue)
            {
                var positions = await _context.Positions
                    .AsNoTracking()
                    .Where(x =>
                        x.DivisionId == model.DivisionId.Value &&
                        x.Division.DepartmentId ==
                            model.DepartmentId.Value)
                    .OrderBy(x => x.Name)
                    .Select(x => new { x.Id, x.Name })
                    .ToListAsync();

                model.Positions = positions
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    })
                    .ToList();
            }
        }
    }
}