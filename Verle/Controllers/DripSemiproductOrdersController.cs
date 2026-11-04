using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Verle.Data;
using Verle.Models;
using Verle.ViewModels;

namespace Verle.Controllers
{
    public class DripSemiproductOrdersController : Controller
    {
        private readonly VerleContext _context;

        public DripSemiproductOrdersController(VerleContext context)
        {
            _context = context;
        }

        // Список заказов.
        public async Task<IActionResult> Index()
        {
            var orders = await _context.DripSemiproductOrders
                .AsNoTracking()
                .Include(x => x.Nomenclatura)
                .Include(x => x.Shift)
                .Include(x => x.Fuso)
                .Include(x => x.Execution)
                .OrderByDescending(x => x.OrderDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync();

            return View(orders);
        }

        // Карточка заказа.
        public async Task<IActionResult> Details(int id)
        {
            var order = await GetOrderWithReferencesAsync(id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // Форма создания.
        public async Task<IActionResult> Create()
        {
            await LoadReferencesAsync();

            return View(new DripSemiproductOrderFormViewModel());
        }

        // Сохранение нового заказа.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            DripSemiproductOrderFormViewModel model)
        {
            Normalize(model);
            await ValidateFormAsync(model);

            if (!ModelState.IsValid)
            {
                await LoadReferencesAsync();
                return View(model);
            }

            var order = new DripSemiproductOrder
            {
                Id1C = model.Id1C,
                OrderDate = model.OrderDate!.Value,
                NomenclaturaId = model.NomenclaturaId!.Value,
                ShiftId = model.ShiftId!.Value,
                FusoId = model.FusoId!.Value,
                Comment = model.Comment,
                Plan = model.Plan!.Value
            };

            _context.DripSemiproductOrders.Add(order);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsDuplicateId1C(ex))
            {
                ModelState.AddModelError(
                    nameof(model.Id1C),
                    "Заказ с таким ID в 1С уже существует.");

                await LoadReferencesAsync();
                return View(model);
            }

            TempData["SuccessMessage"] = "Заказ создан.";

            return RedirectToAction(nameof(Index));
        }

        // Форма редактирования.
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.DripSemiproductOrders
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);

            if (order == null)
                return NotFound();

            var model = new DripSemiproductOrderFormViewModel
            {
                Id = order.Id,
                Id1C = order.Id1C,
                OrderDate = order.OrderDate,
                NomenclaturaId = order.NomenclaturaId,
                ShiftId = order.ShiftId,
                FusoId = order.FusoId,
                Comment = order.Comment,
                Plan = order.Plan
            };

            await LoadReferencesAsync();

            return View(model);
        }

        // Сохранение изменений.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            DripSemiproductOrderFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var order = await _context.DripSemiproductOrders
                .SingleOrDefaultAsync(x => x.Id == id);

            if (order == null)
                return NotFound();

            Normalize(model);
            await ValidateFormAsync(model);

            if (!ModelState.IsValid)
            {
                await LoadReferencesAsync();
                return View(model);
            }

            order.Id1C = model.Id1C;
            order.OrderDate = model.OrderDate!.Value;
            order.NomenclaturaId = model.NomenclaturaId!.Value;
            order.ShiftId = model.ShiftId!.Value;
            order.FusoId = model.FusoId!.Value;
            order.Comment = model.Comment;
            order.Plan = model.Plan!.Value;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.DripSemiproductOrders
                    .AnyAsync(x => x.Id == id))
                {
                    return NotFound();
                }

                throw;
            }
            catch (DbUpdateException ex) when (IsDuplicateId1C(ex))
            {
                ModelState.AddModelError(
                    nameof(model.Id1C),
                    "Заказ с таким ID в 1С уже существует.");

                await LoadReferencesAsync();
                return View(model);
            }

            TempData["SuccessMessage"] = "Изменения сохранены.";

            return RedirectToAction(nameof(Index));
        }

        // Подтверждение удаления.
        public async Task<IActionResult> Delete(int id)
        {
            var order = await GetOrderWithReferencesAsync(id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // Удаление выполняется только POST-запросом.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.DripSemiproductOrders
                .SingleOrDefaultAsync(x => x.Id == id);

            if (order == null)
                return NotFound();

            _context.DripSemiproductOrders.Remove(order);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is PostgresException
                {
                    SqlState: PostgresErrorCodes.ForeignKeyViolation
                })
            {
                // Пригодится после добавления связанных таблиц.
                ModelState.AddModelError(
                    string.Empty,
                    "Нельзя удалить заказ: на него ссылаются другие записи.");

                _context.Entry(order).State = EntityState.Unchanged;

                var orderWithReferences =
                    await GetOrderWithReferencesAsync(id);

                if (orderWithReferences == null)
                    return NotFound();

                return View("Delete", orderWithReferences);
            }

            TempData["SuccessMessage"] = "Заказ удалён.";

            return RedirectToAction(nameof(Index));
        }

        private Task<DripSemiproductOrder?> GetOrderWithReferencesAsync(
            int id)
        {
            return _context.DripSemiproductOrders
                .AsNoTracking()
                .Include(x => x.Nomenclatura)
                .Include(x => x.Shift)
                .Include(x => x.Fuso)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        // Загрузка справочников для выпадающих списков.
        private async Task LoadReferencesAsync()
        {
            var nomenclaturas = await _context.Set<Nomenclatura>()
                .AsNoTracking()
                .Where(x =>
                    x.SkuClassLink != null &&
                    x.SkuClassLink.SkuClass.Id == 2)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id1C)
                .Select(x => new
                {
                    id = x.Id,
                    id1C = x.Id1C,
                    name = x.Name
                })
                .ToListAsync();

            ViewBag.Nomenclaturas = nomenclaturas
                .Select(x => new SelectListItem
                {
                    Value = x.id.ToString(),
                    Text = $"{x.id1C} — {x.name}"
                })
                .ToList();

            // Данные для отдельных информационных полей в форме.
            ViewData["NomenclaturaInfo"] = nomenclaturas;

            var shifts = await _context.Set<Shift>()
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync();

            ViewBag.Shifts = shifts
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();

            var fusos = await _context.Set<Fuso>()
                .AsNoTracking()
                .OrderBy(x => x.FusoNumber)
                .ToListAsync();

            ViewBag.Fusos = fusos
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FusoNumber
                })
                .ToList();
        }

        private static void Normalize(
            DripSemiproductOrderFormViewModel model)
        {
            model.Id1C = (model.Id1C ?? string.Empty).Trim();

            model.Comment = string.IsNullOrWhiteSpace(model.Comment)
                ? null
                : model.Comment.Trim();
        }

        // Серверные проверки не зависят от JavaScript в браузере.
        private async Task ValidateFormAsync(
            DripSemiproductOrderFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Id1C))
            {
                ModelState.AddModelError(
                    nameof(model.Id1C),
                    "Укажите ID заказа в 1С.");
            }
            else if (await _context.DripSemiproductOrders
                .AnyAsync(x =>
                    x.Id1C == model.Id1C &&
                    x.Id != model.Id))
            {
                ModelState.AddModelError(
                    nameof(model.Id1C),
                    "Заказ с таким ID в 1С уже существует.");
            }

            if (model.NomenclaturaId.HasValue)
            {
                var isAllowedNomenclatura = await _context.Set<Nomenclatura>()
                    .AnyAsync(x =>
                        x.Id == model.NomenclaturaId.Value &&
                        x.SkuClassLink != null &&
                        x.SkuClassLink.SkuClass.Name == "Полуфабрикат");

                if (!isAllowedNomenclatura)
                {
                    ModelState.AddModelError(
                        nameof(model.NomenclaturaId),
                        "Выберите номенклатуру с классом SKU «Полуфабрикат».");
                }
            }

            if (model.ShiftId.HasValue &&
                !await _context.Set<Shift>()
                    .AnyAsync(x => x.Id == model.ShiftId.Value))
            {
                ModelState.AddModelError(
                    nameof(model.ShiftId),
                    "Выбранная смена не найдена.");
            }

            if (model.FusoId.HasValue &&
                !await _context.Set<Fuso>()
                    .AnyAsync(x => x.Id == model.FusoId.Value))
            {
                ModelState.AddModelError(
                    nameof(model.FusoId),
                    "Выбранная машина FUSO не найдена.");
            }

            if (model.Plan.HasValue &&
                decimal.Round(model.Plan.Value, 3) != model.Plan.Value)
            {
                ModelState.AddModelError(
                    nameof(model.Plan),
                    "Укажите не более трёх знаков после запятой.");
            }
        }

        private static bool IsDuplicateId1C(DbUpdateException exception)
        {
            return exception.InnerException is PostgresException postgres &&
                   postgres.SqlState == PostgresErrorCodes.UniqueViolation &&
                   postgres.ConstraintName ==
                       "UX_DripSemiproductOrders_id1C";
        }
        // GET: DripSemiproductOrders/Execute/5
        [HttpGet]
        public async Task<IActionResult> Execute(int id)
        {
            var order = await GetOrderForExecutionAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            var model = new DripSemiproductOrderExecutionViewModel
            {
                Actual = order.Execution?.Actual,
                Comment = order.Execution?.Comment
            };

            FillExecutionOrderDetails(model, order);

            return View(model);
        }

        // POST: DripSemiproductOrders/Execute/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Execute(
            int id,
            DripSemiproductOrderExecutionViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var order = await GetOrderForExecutionAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            if (model.Actual.HasValue &&
                decimal.Round(model.Actual.Value, 3) != model.Actual.Value)
            {
                ModelState.AddModelError(
                    nameof(model.Actual),
                    "Укажите факт с точностью не более трёх знаков после запятой.");
            }

            if (!ModelState.IsValid)
            {
                // Детали заказа всегда повторно получаем из базы.
                FillExecutionOrderDetails(model, order);

                return View(model);
            }

            if (order.Execution == null)
            {
                var execution = new DripSemiproductOrderExecution
                {
                    OrderId = order.Id,
                    Actual = model.Actual!.Value,
                    Comment = model.Comment
                };

                _context.DripSemiproductOrderExecutions.Add(execution);
            }
            else
            {
                order.Execution.Actual = model.Actual!.Value;
                order.Execution.Comment = model.Comment;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private Task<DripSemiproductOrder?> GetOrderForExecutionAsync(int id)
        {
            return _context.DripSemiproductOrders
                .Include(x => x.Nomenclatura)
                .Include(x => x.Shift)
                .Include(x => x.Fuso)
                .Include(x => x.Execution)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        private static void FillExecutionOrderDetails(
            DripSemiproductOrderExecutionViewModel model,
            DripSemiproductOrder order)
        {
            model.Id = order.Id;
            model.Id1C = order.Id1C;
            model.OrderDate = order.OrderDate;

            model.NomenclaturaId1C = order.Nomenclatura.Id1C;
            model.NomenclaturaName = order.Nomenclatura.Name;

            model.ShiftName = order.Shift.Name;
            model.FusoNumber = order.Fuso.FusoNumber;

            model.Plan = order.Plan;
            model.OrderComment = order.Comment;
        }
    }
}