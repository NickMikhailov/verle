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
    public class ProductionOrdersController : Controller
    {
        private const int DraftStatusId = 1;
        private const int CreatedStatusId = 2;
        private const int InExecutionStatusId = 3;

        // ID класса SKU «Полуфабрикат».
        private const int SemiproductSkuClassId = 2;

        private readonly VerleContext _context;

        public ProductionOrdersController(VerleContext context)
        {
            _context = context;
        }

        // GET: ProductionOrders
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await QueryWithReferences()
                .AsNoTracking()
                .OrderByDescending(x => x.OrderDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync();

            return View(orders);
        }

        // GET: ProductionOrders/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await GetOrderWithReferencesAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET ничего не создаёт.
        [HttpGet]
        public IActionResult Create()
        {
            return RedirectToAction(nameof(Index));
        }

        // POST: ProductionOrders/Create
        // Создаёт пустой черновик и открывает редактирование.
        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDraft()
        {
            var order = new ProductionOrder
            {
                StatusId = DraftStatusId
            };

            _context.ProductionOrders.Add(order);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Черновик создан.";

            return RedirectToAction(
                nameof(Edit),
                new { id = order.Id });
        }

        // GET: ProductionOrders/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.ProductionOrders
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.StatusId == InExecutionStatusId)
            {
                TempData["ErrorMessage"] =
                    "Заказ находится на исполнении " +
                    "и недоступен для редактирования.";

                return RedirectToAction(
                    nameof(Execute),
                    new { id = order.Id });
            }

            var model = new ProductionOrderFormViewModel
            {
                Id = order.Id,
                Id1C = order.Id1C,
                OrderDate = order.OrderDate,
                NomenclaturaId = order.NomenclaturaId,
                ShiftId = order.ShiftId,
                FusoId = order.EquipmentId,
                Comment = order.Comment,
                Plan = order.Plan
            };

            await PrepareEditFormAsync(
                isDraft: order.StatusId == DraftStatusId);

            return View(model);
        }

        // POST: ProductionOrders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    ProductionOrderFormViewModel model,
    string submitAction)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var order = await _context.ProductionOrders
                .SingleOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.StatusId == InExecutionStatusId)
            {
                TempData["ErrorMessage"] =
                    "Заказ находится на исполнении и недоступен для редактирования.";

                return RedirectToAction(
                    nameof(Execute),
                    new { id = order.Id });
            }

            var originalStatusId = order.StatusId;

            var isCreateAction = string.Equals(
                submitAction,
                "create",
                StringComparison.OrdinalIgnoreCase);

            var isSaveAction = string.Equals(
                submitAction,
                "save",
                StringComparison.OrdinalIgnoreCase);

            if (!isCreateAction && !isSaveAction)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Не удалось определить действие формы.");
            }

            Normalize(model);

            await ValidateFormAsync(
                model,
                requireComplete: isCreateAction);

            if (!ModelState.IsValid)
            {
                await PrepareEditFormAsync(
                    isDraft: originalStatusId == DraftStatusId);

                return View(model);
            }

            // Перенос данных из модели формы в заказ.
            order.Id1C = string.IsNullOrWhiteSpace(model.Id1C)
                ? null
                : model.Id1C.Trim();

            order.OrderDate = model.OrderDate;
            order.NomenclaturaId = model.NomenclaturaId;
            order.ShiftId = model.ShiftId;
            order.EquipmentId = model.FusoId;
            order.Comment = model.Comment;
            order.Plan = model.Plan;

            // Изменение статуса зависит только от нажатой кнопки.
            if (isCreateAction)
            {
                order.StatusId = CreatedStatusId;
            }
            else
            {
                order.StatusId = originalStatusId == CreatedStatusId
                    ? DraftStatusId
                    : originalStatusId;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.ProductionOrders
                    .AnyAsync(x => x.Id == id);

                if (!exists)
                {
                    return NotFound();
                }

                throw;
            }

            if (isCreateAction)
            {
                TempData["SuccessMessage"] =
                    "Заказ проверен и сохранён со статусом «Создан».";
            }
            else if (originalStatusId == CreatedStatusId)
            {
                TempData["SuccessMessage"] =
                    "Изменения сохранены. Заказ переведён в статус «Редактируется».";
            }
            else
            {
                TempData["SuccessMessage"] =
                    "Черновик сохранён.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ProductionOrders/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await GetOrderWithReferencesAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: ProductionOrders/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.ProductionOrders
                .SingleOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            _context.ProductionOrders.Remove(order);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.ProductionOrders
                    .AnyAsync(x => x.Id == id);

                if (!exists)
                {
                    return NotFound();
                }

                throw;
            }
            catch (DbUpdateException exception)
                when (IsForeignKeyViolation(exception))
            {
                _context.Entry(order).State = EntityState.Unchanged;

                ModelState.AddModelError(
                    string.Empty,
                    "Нельзя удалить заказ: " +
                    "на него ссылаются другие записи.");

                var orderWithReferences =
                    await GetOrderWithReferencesAsync(id);

                if (orderWithReferences == null)
                {
                    return NotFound();
                }

                return View("Delete", orderWithReferences);
            }

            TempData["SuccessMessage"] = "Заказ удалён.";

            return RedirectToAction(nameof(Index));
        }

        // GET: ProductionOrders/Execute/5
        // Открывает форму, но не изменяет статус.
        [HttpGet]
        public async Task<IActionResult> Execute(int id)
        {
            var order = await GetOrderForExecutionAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.StatusId != CreatedStatusId &&
                order.StatusId != InExecutionStatusId)
            {
                TempData["ErrorMessage"] =
                    "Форма исполнения доступна только для созданного " +
                    "или исполняемого заказа.";

                return RedirectToAction(nameof(Index));
            }

            if (!HasCompleteOrderData(order))
            {
                return RedirectForIncompleteOrder(order);
            }

            ViewBag.ExecutionStarted =
                order.StatusId == InExecutionStatusId;

            var model = new ProductionOrderExecutionViewModel
            {
                Id = order.Id,
                Actual = order.Execution?.Actual,
                Comment = order.Execution?.Comment
            };

            FillExecutionOrderDetails(model, order);

            return View(model);
        }

        // POST: ProductionOrders/Start/5
        // Переводит созданный заказ в статус «На исполнении».
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(int id)
        {
            var order = await GetOrderForExecutionAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Повторное нажатие «Старт» не меняет статус.
            if (order.StatusId == InExecutionStatusId)
            {
                TempData["ErrorMessage"] =
                    "Заказ уже находится на исполнении.";

                return RedirectToAction(
                    nameof(Execute),
                    new { id = order.Id });
            }

            if (order.StatusId != CreatedStatusId)
            {
                TempData["ErrorMessage"] =
                    "Запустить можно только заказ " +
                    "со статусом «Создан».";

                return RedirectToAction(nameof(Index));
            }

            if (!HasCompleteOrderData(order))
            {
                return RedirectForIncompleteOrder(order);
            }

            order.StatusId = InExecutionStatusId;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Заказ переведён в статус «На исполнении».";

            return RedirectToAction(
                nameof(Execute),
                new { id = order.Id });
        }

        // POST: ProductionOrders/Execute/5
        // Сохраняет факт и комментарий только после старта.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Execute(
            int id,
            ProductionOrderExecutionViewModel model)
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

            if (order.StatusId != InExecutionStatusId)
            {
                if (order.StatusId == CreatedStatusId)
                {
                    TempData["ErrorMessage"] =
                        "Сначала нажмите кнопку «Старт».";

                    return RedirectToAction(
                        nameof(Execute),
                        new { id = order.Id });
                }

                TempData["ErrorMessage"] =
                    "Сохранение исполнения доступно только " +
                    "для заказа со статусом «На исполнении».";

                return RedirectToAction(nameof(Index));
            }

            if (!HasCompleteOrderData(order))
            {
                return RedirectForIncompleteOrder(order);
            }

            model.Comment = string.IsNullOrWhiteSpace(model.Comment)
                ? null
                : model.Comment.Trim();

            if (!model.Actual.HasValue)
            {
                AddModelErrorIfMissing(
                    nameof(model.Actual),
                    "Укажите фактическое количество.");
            }
            else
            {
                if (model.Actual.Value < 0)
                {
                    AddModelErrorIfMissing(
                        nameof(model.Actual),
                        "Фактическое количество не может быть отрицательным.");
                }

                if (decimal.Round(model.Actual.Value, 3)
                    != model.Actual.Value)
                {
                    AddModelErrorIfMissing(
                        nameof(model.Actual),
                        "Укажите факт с точностью не более " +
                        "трёх знаков после запятой.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ExecutionStarted = true;

                FillExecutionOrderDetails(model, order);

                return View(model);
            }

            if (order.Execution == null)
            {
                var execution = new ProductionOrderExecution
                {
                    OrderId = order.Id,
                    Actual = model.Actual!.Value,
                    Comment = model.Comment
                };

                _context.ProductionOrderExecutions.Add(execution);
            }
            else
            {
                order.Execution.Actual = model.Actual!.Value;
                order.Execution.Comment = model.Comment;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Данные исполнения сохранены.";

            return RedirectToAction(nameof(Index));
        }

        // Общий запрос со связанными данными.
        private IQueryable<ProductionOrder> QueryWithReferences()
        {
            return _context.ProductionOrders
                .Include(x => x.Nomenclatura)
                .Include(x => x.Shift)
                .Include(x => x.Equipment)
                .Include(x => x.Execution)
                .Include(x => x.Status);
        }

        private Task<ProductionOrder?> GetOrderWithReferencesAsync(
            int id)
        {
            return QueryWithReferences()
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        // Для сохранения исполнения нужен отслеживаемый объект.
        private Task<ProductionOrder?> GetOrderForExecutionAsync(
            int id)
        {
            return QueryWithReferences()
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        private async Task PrepareEditFormAsync(bool isDraft)
        {
            ViewData["IsDraft"] = isDraft;

            await LoadReferencesAsync();
        }

        // Единый фильтр номенклатуры для списка и проверки.
        private IQueryable<Nomenclatura> AllowedNomenclaturas()
        {
            return _context.Set<Nomenclatura>()
                .Where(x =>
                    x.SkuClassLink != null &&
                    x.SkuClassLink.SkuClass.Id
                        == SemiproductSkuClassId);
        }

        // Справочники для формы редактирования.
        private async Task LoadReferencesAsync()
        {
            var nomenclaturas = await AllowedNomenclaturas()
                .AsNoTracking()
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

            ViewData["NomenclaturaInfo"] = nomenclaturas;

            var shifts = await _context.Set<Shift>()
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .ToListAsync();

            ViewBag.Shifts = shifts
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();

            var fusos = await _context.Set<Equipment>()
                .AsNoTracking()
                .OrderBy(x => x.EquipmentName)
                .Select(x => new
                {
                    x.Id,
                    x.EquipmentName
                })
                .ToListAsync();

            ViewBag.Fusos = fusos
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.EquipmentName
                })
                .ToList();
        }

        private static void Normalize(
            ProductionOrderFormViewModel model)
        {
            // Пустые строки сохраняем в базе как NULL.
            model.Id1C = string.IsNullOrWhiteSpace(model.Id1C)
                ? null
                : model.Id1C.Trim();

            model.Comment = string.IsNullOrWhiteSpace(model.Comment)
                ? null
                : model.Comment.Trim();
        }

        // Обязательность зависит от нажатой кнопки.
        // Заполненные значения проверяются в любом случае.
        private async Task ValidateFormAsync(
            ProductionOrderFormViewModel model,
            bool requireComplete)
        {
            if (requireComplete)
            {
                ValidateRequiredFields(model);
            }

            if (!string.IsNullOrWhiteSpace(model.Id1C))
            {
                var duplicateExists = await _context.ProductionOrders
                    .AnyAsync(x =>
                        x.Id1C == model.Id1C &&
                        x.Id != model.Id);

                if (duplicateExists)
                {
                    AddModelErrorIfMissing(
                        nameof(model.Id1C),
                        "Заказ с таким ID в 1С уже существует.");
                }
            }

            if (model.NomenclaturaId.HasValue)
            {
                var isAllowed = await AllowedNomenclaturas()
                    .AnyAsync(x =>
                        x.Id == model.NomenclaturaId.Value);

                if (!isAllowed)
                {
                    AddModelErrorIfMissing(
                        nameof(model.NomenclaturaId),
                        "Выберите номенклатуру " +
                        "с классом SKU «Полуфабрикат».");
                }
            }

            if (model.ShiftId.HasValue)
            {
                var exists = await _context.Set<Shift>()
                    .AnyAsync(x => x.Id == model.ShiftId.Value);

                if (!exists)
                {
                    AddModelErrorIfMissing(
                        nameof(model.ShiftId),
                        "Выбранная смена не найдена.");
                }
            }

            if (model.FusoId.HasValue)
            {
                var exists = await _context.Set<Equipment>()
                    .AnyAsync(x => x.Id == model.FusoId.Value);

                if (!exists)
                {
                    AddModelErrorIfMissing(
                        nameof(model.FusoId),
                        "Выбранная машина FUSO не найдена.");
                }
            }

            if (model.Plan.HasValue)
            {
                if (model.Plan.Value < 0 ||
                    model.Plan.Value > 999999999999999.999m)
                {
                    AddModelErrorIfMissing(
                        nameof(model.Plan),
                        "План должен быть неотрицательным " +
                        "и помещаться в numeric(18,3).");
                }

                if (decimal.Round(model.Plan.Value, 3)
                    != model.Plan.Value)
                {
                    AddModelErrorIfMissing(
                        nameof(model.Plan),
                        "Укажите не более трёх знаков после запятой.");
                }
            }
        }

        private void ValidateRequiredFields(
            ProductionOrderFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Id1C))
            {
                AddModelErrorIfMissing(
                    nameof(model.Id1C),
                    "Укажите ID заказа в 1С.");
            }

            if (!model.OrderDate.HasValue)
            {
                AddModelErrorIfMissing(
                    nameof(model.OrderDate),
                    "Укажите дату заказа.");
            }

            if (!model.NomenclaturaId.HasValue)
            {
                AddModelErrorIfMissing(
                    nameof(model.NomenclaturaId),
                    "Выберите номенклатуру.");
            }

            if (!model.ShiftId.HasValue)
            {
                AddModelErrorIfMissing(
                    nameof(model.ShiftId),
                    "Выберите смену.");
            }

            if (!model.FusoId.HasValue)
            {
                AddModelErrorIfMissing(
                    nameof(model.FusoId),
                    "Выберите машину FUSO.");
            }

            if (!model.Plan.HasValue)
            {
                AddModelErrorIfMissing(
                    nameof(model.Plan),
                    "Укажите плановое количество.");
            }
        }

        private void AddModelErrorIfMissing(
            string key,
            string message)
        {
            if (ModelState.TryGetValue(key, out var entry) &&
                entry.Errors.Count > 0)
            {
                return;
            }

            ModelState.AddModelError(key, message);
        }

        // Проверяет заполненность заказа и наличие связанных данных.
        // Вызывается для заказа, загруженного через
        // GetOrderForExecutionAsync.
        private static bool HasCompleteOrderData(
            ProductionOrder order)
        {
            return !string.IsNullOrWhiteSpace(order.Id1C)
                && order.OrderDate.HasValue
                && order.NomenclaturaId.HasValue
                && order.ShiftId.HasValue
                && order.EquipmentId.HasValue
                && order.Plan.HasValue
                && order.Nomenclatura != null
                && order.Shift != null
                && order.Equipment != null;
        }

        // Не отправляем исполняемый неполный заказ в Edit:
        // Edit запретит редактирование и вернёт его в Execute.
        private IActionResult RedirectForIncompleteOrder(
            ProductionOrder order)
        {
            if (order.StatusId == CreatedStatusId)
            {
                TempData["ErrorMessage"] =
                    "Заказ заполнен не полностью. " +
                    "Сначала заполните обязательные поля.";

                return RedirectToAction(
                    nameof(Edit),
                    new { id = order.Id });
            }

            TempData["ErrorMessage"] =
                "У исполняемого заказа отсутствуют обязательные " +
                "или связанные данные. Обратитесь к администратору.";

            return RedirectToAction(nameof(Index));
        }

        // Вызывается после успешного HasCompleteOrderData.
        private static void FillExecutionOrderDetails(
            ProductionOrderExecutionViewModel model,
            ProductionOrder order)
        {
            model.Id = order.Id;
            model.Id1C = order.Id1C!;
            model.OrderDate = order.OrderDate!.Value;

            model.NomenclaturaId1C =
                order.Nomenclatura!.Id1C;

            model.NomenclaturaName =
                order.Nomenclatura!.Name;

            model.ShiftName = order.Shift!.Name;

            model.FusoNumber =
                order.Equipment!.EquipmentName;

            model.Plan = order.Plan!.Value;
            model.OrderComment = order.Comment;
        }

        private static bool IsDuplicateId1C(
            DbUpdateException exception)
        {
            return exception.InnerException
                is PostgresException postgres
                && postgres.SqlState
                    == PostgresErrorCodes.UniqueViolation
                && postgres.ConstraintName
                    == "UX_ProductionOrders_id1C";
        }

        private static bool IsForeignKeyViolation(
            DbUpdateException exception)
        {
            return exception.InnerException
                is PostgresException postgres
                && postgres.SqlState
                    == PostgresErrorCodes.ForeignKeyViolation;
        }
    }
}