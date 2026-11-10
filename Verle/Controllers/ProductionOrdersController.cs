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
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace Verle.Controllers
{
    public class ProductionOrdersController : Controller
    {
        private const int DraftStatusId = 1;
        private const int CreatedStatusId = 2;
        private const int InExecutionStatusId = 3;
        private const int MasterApprovalStatusId = 4;
        private const int OperatorApprovalStatusId = 5;
        private const int CompletedStatusId = 6;
        private const int FactCorrectionStatusId = 7;

        // ID класса SKU «Готовая продукция».
        private const int ProductSkuClassId = 1;

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
            ViewBag.StatusFilters = await _context.Set<OrderStatus>()
                .AsNoTracking()
                .OrderBy(x => x.Id)
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

            if (order.StatusId != DraftStatusId &&
                order.StatusId != CreatedStatusId)
            {
                TempData["ErrorMessage"] =
                    "После запуска исходные данные заказа " +
                    "недоступны для редактирования.";

                return RedirectToAction(nameof(Index));
            }

            var model = new ProductionOrderFormViewModel
            {
                Id = order.Id,
                Id1C = order.Id1C,
                OrderDate = order.OrderDate,
                NomenclaturaId = order.NomenclaturaId,
                ShiftId = order.ShiftId,
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

                await using var transaction =
                await _context.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.ReadCommitted);

            var order = await LockOrderAsync(id);

            if (order == null)
                    {
                        return NotFound();
                    }

            if (order.StatusId != DraftStatusId &&
                order.StatusId != CreatedStatusId)
            {
                TempData["ErrorMessage"] =
                    "После запуска исходные данные заказа " +
                    "недоступны для редактирования.";

                return RedirectToAction(nameof(Index));
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
                await transaction.CommitAsync();
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

            // Просмотр разрешён для статусов:
            // 2 — Создан;
            // 3 — На исполнении;
            // 4 — На утверждении мастера;
            // 5 — На утверждении операционистом.
            // 6 — Заказ завершен.
            if (order.StatusId != CreatedStatusId &&
                order.StatusId != InExecutionStatusId &&
                order.StatusId != MasterApprovalStatusId &&
                order.StatusId != OperatorApprovalStatusId &&
                order.StatusId != FactCorrectionStatusId &&
                order.StatusId != CompletedStatusId)
                        {
                TempData["ErrorMessage"] =
                    "Форма выполнения недоступна для этого статуса заказа.";

                return RedirectToAction(nameof(Index));
            }

            if (!HasCompleteOrderData(order))
            {
                return RedirectForIncompleteOrder(order);
            }

            // Передаём текущий статус в Execute.cshtml.
            ViewBag.OrderStatusId = order.StatusId;
            ViewBag.OrderStatusName = order.Status?.Name;

            // Редактировать исполнение можно только в статусе 3.
            ViewBag.ExecutionStarted =
                order.StatusId == InExecutionStatusId;

            ViewBag.MasterApproval =
                order.StatusId == MasterApprovalStatusId;

            ViewBag.OperatorApproval =
                order.StatusId == OperatorApprovalStatusId;

            var model = new ProductionOrderExecutionViewModel
            {
                Id = order.Id,
                Actual = order.Execution?.Actual,
                Comment = order.Execution?.Comment
            };

            FillExecutionOrderDetails(model, order);
            await FillExecutionHistoryAsync(model, order.Id);
            return View(model);
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
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.ReadCommitted);

            var lockedOrder = await LockOrderAsync(id);

            if (lockedOrder == null)
            {
                return NotFound();
            }
            var order = await GetOrderForExecutionAsync(id);

            if (order == null)
            {
                return NotFound();
            }
            var canEditActual =
                order.StatusId == InExecutionStatusId ||
                order.StatusId == FactCorrectionStatusId;   

            if (!canEditActual)
            {
                TempData["ErrorMessage"] =
                    order.StatusId == CreatedStatusId
                        ? "Сначала нажмите кнопку «Старт»."
                        : "Изменение фактических данных доступно только " +
                          "во время исполнения или корректировки факта.";

                return RedirectToAction(
                    nameof(Execute),
                    new { id = order.Id });
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
                ViewBag.OrderStatusId = order.StatusId;

                ViewBag.OrderStatusName = order.Status?.Name;

                ViewBag.ExecutionStarted = order.StatusId == InExecutionStatusId;

                ViewBag.MasterApproval = order.StatusId == MasterApprovalStatusId;

                FillExecutionOrderDetails(model, order);
                await FillExecutionHistoryAsync(model, order.Id);
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
            await transaction.CommitAsync();

            TempData["SuccessMessage"] =
                "Данные исполнения сохранены.";

            return RedirectToAction(
                nameof(Execute),
                new { id = order.Id });
        }

        // POST: ProductionOrders/Start/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Start(
            int id,
            long? expectedLastEventId)
        {
            return ChangeExecutionStateAsync(
                id,
                ExecutionStatusIds.InProgress,
                expectedLastEventId,
                isStart: true);
        }

        // POST: ProductionOrders/Downtime/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Downtime(
            int id,
            long? expectedLastEventId)
        {
            return ChangeExecutionStateAsync(
                id,
                ExecutionStatusIds.Downtime,
                expectedLastEventId);
        }

        // POST: ProductionOrders/Resume/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Resume(
            int id,
            long? expectedLastEventId)
        {
            return ChangeExecutionStateAsync(
                id,
                ExecutionStatusIds.InProgress,
                expectedLastEventId);
        }

        // POST: ProductionOrders/Finish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Finish(
            int id,
            long? expectedLastEventId)
        {
            return ChangeExecutionStateAsync(
                id,
                ExecutionStatusIds.Finished,
                expectedLastEventId);
        }
        private async Task<IActionResult> ChangeExecutionStateAsync(
    int id,
    int nextExecutionStatusId,
    long? expectedLastEventId,
    bool isStart = false)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.ReadCommitted);

            var order = await LockOrderAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // События читаем после получения блокировки заказа.
            var lastEvent = await _context.ProductionOrderExecutionEvents
                .AsNoTracking()
                .Where(x => x.OrderId == id)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            // Защищает и от двойного нажатия, и от действия
            // из вкладки, которая показывает устаревшее состояние.
            if (lastEvent?.Id != expectedLastEventId)
            {
                return RedirectToExecutionWithError(
                    id,
                    "Состояние выполнения уже изменилось. " +
                    "Страница обновлена — проверьте состояние и повторите действие.");
            }

            string successMessage;

            if (isStart)
            {
                if (order.StatusId != CreatedStatusId)
                {
                    return RedirectToExecutionWithError(
                        id,
                        "Запустить можно только заказ со статусом «Создан».");
                }

                if (lastEvent != null)
                {
                    return RedirectToExecutionWithError(
                        id,
                        "У заказа уже есть история выполнения. " +
                        "Повторный запуск недоступен.");
                }

                // Загружаем справочники для существующей проверки полноты.
                var orderWithReferences =
                    await GetOrderForExecutionAsync(id);

                if (orderWithReferences == null)
                {
                    return NotFound();
                }

                if (!HasCompleteOrderData(orderWithReferences))
                {
                    return RedirectForIncompleteOrder(orderWithReferences);
                }

                order.StatusId = InExecutionStatusId;

                successMessage = "Выполнение заказа начато.";
            }
            else
            {
                if (order.StatusId != InExecutionStatusId)
                {
                    return RedirectToExecutionWithError(
                        id,
                        "Действие доступно только для заказа " +
                        "со статусом «На исполнении».");
                }

                var currentState = lastEvent?.ExecutionStatusId;

                // Для заказов, запущенных до внедрения журнала,
                // состояние может быть неизвестно.
                var hasUnknownLegacyState = lastEvent == null;

                switch (nextExecutionStatusId)
                {
                    case ExecutionStatusIds.Downtime:
                        if (!hasUnknownLegacyState &&
                            currentState != ExecutionStatusIds.InProgress)
                        {
                            return RedirectToExecutionWithError(
                                id,
                                "Перевести в простой можно только работающий заказ.");
                        }

                        successMessage = "Начало простоя зафиксировано.";
                        break;

                    case ExecutionStatusIds.InProgress:
                        if (currentState != ExecutionStatusIds.Downtime)
                        {
                            return RedirectToExecutionWithError(
                                id,
                                "Снять с простоя можно только заказ, " +
                                "находящийся в простое.");
                        }

                        successMessage =
                            "Простой завершён. Выполнение заказа продолжено.";
                        break;

                    case ExecutionStatusIds.Finished:
                        if (!hasUnknownLegacyState &&
                            currentState != ExecutionStatusIds.InProgress &&
                            currentState != ExecutionStatusIds.Downtime)
                        {
                            return RedirectToExecutionWithError(
                                id,
                                "Выполнение заказа уже завершено " +
                                "или текущее состояние не допускает завершение.");
                        }

                        order.StatusId = MasterApprovalStatusId;

                        successMessage =
                            "Выполнение завершено. " +
                            "Заказ передан на утверждение мастера.";
                        break;

                    default:
                        return BadRequest();
                }
            }

            _context.ProductionOrderExecutionEvents.Add(
                new ProductionOrderExecutionEvent
                {
                    OrderId = order.Id,
                    OccurredAt = DateTime.UtcNow,
                    ExecutionStatusId = nextExecutionStatusId
                });

            // Событие и изменение статуса сохраняются одной транзакцией.
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["SuccessMessage"] = successMessage;

            return RedirectToAction(
                nameof(Execute),
                new { id });
        }
        // POST: ProductionOrders/ApproveByMaster/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> ApproveByMaster(int id)
        {
            return ChangeWorkflowStatusAsync(
                id: id,
                expectedStatusId: MasterApprovalStatusId,
                nextStatusId: OperatorApprovalStatusId,
                successMessage:
                    "Заказ утверждён мастером и передан " +
                    "на утверждение операционистом.",
                openExecution: false);
        }

        // POST: ProductionOrders/RejectByMaster/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> RejectByMaster(int id)
        {
            return ChangeWorkflowStatusAsync(
                id: id,
                expectedStatusId: MasterApprovalStatusId,
                nextStatusId: FactCorrectionStatusId,
                successMessage:
                    "Заказ возвращён на корректировку факта. " +
                    "Физическое выполнение остаётся завершённым.",
                openExecution: true);
        }
        // POST: ProductionOrders/SubmitCorrections/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> SubmitCorrections(int id)
        {
            return ChangeWorkflowStatusAsync(
                id: id,
                expectedStatusId: FactCorrectionStatusId,
                nextStatusId: MasterApprovalStatusId,
                successMessage:
                    "Исправленные данные переданы на утверждение мастера.",
                openExecution: true);
        }
        // Вызывать только внутри открытой транзакции.
        //
        // Блокировка строки заказа сериализует действия над одним заказом:
        // второй запрос ждёт окончания первого и затем проверяет новое состояние.
        private async Task<ProductionOrder?> LockOrderAsync(int id)
        {
            var orders = await _context.ProductionOrders
                .FromSqlInterpolated($@"
            SELECT *
            FROM ""productionOrder""
            WHERE ""id"" = {id}
            FOR UPDATE
        ")
                .ToListAsync();

            return orders.SingleOrDefault();
        }

        private IActionResult RedirectToExecutionWithError(
            int id,
            string message)
        {
            TempData["ErrorMessage"] = message;

            return RedirectToAction(
                nameof(Execute),
                new { id });
        }

        // Заполняет только данные отображения.
        // Факт и введённый комментарий не изменяет.
        private async Task FillExecutionHistoryAsync(
            ProductionOrderExecutionViewModel model,
            int orderId)
        {
            var events = await _context.ProductionOrderExecutionEvents
                .AsNoTracking()
                .Where(x => x.OrderId == orderId)
                .OrderBy(x => x.Id)
                .Select(x => new ProductionOrderExecutionEventViewModel
                {
                    Id = x.Id,
                    OccurredAt = x.OccurredAt,
                    ExecutionStatusId = x.ExecutionStatusId,
                    StatusName = x.ExecutionStatus.Name,
                    Comment = x.Comment
                })
                .ToListAsync();

            model.Events = events;

            var lastEvent = events.LastOrDefault();

            model.CurrentExecutionStatusId =
                lastEvent?.ExecutionStatusId;

            model.LastExecutionEventId =
                lastEvent?.Id;

            var firstEvent = events.FirstOrDefault();

            // Для новых заказов первое событие — Старт.
            // У старых заказов журнал может начинаться с простоя/финиша.
            // Не выдаём время возобновления за первоначальный старт.
            model.StartedAt =
                firstEvent?.ExecutionStatusId == ExecutionStatusIds.InProgress
                    ? firstEvent.OccurredAt
                    : null;
        }


        // Меняет только этап обработки заказа.
        // Журнал физического выполнения здесь не изменяется.
        private async Task<IActionResult> ChangeWorkflowStatusAsync(
            int id,
            int expectedStatusId,
            int nextStatusId,
            string successMessage,
            bool openExecution)
        {
            // Одна атомарная команда:
            // проверка текущего статуса и его изменение.
            var affectedRows =
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE ""productionOrder""
            SET ""statusId"" = {nextStatusId}
            WHERE ""id"" = {id}
              AND ""statusId"" = {expectedStatusId}
        ");

            if (affectedRows == 0)
            {
                var exists = await _context.ProductionOrders
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == id);

                if (!exists)
                {
                    return NotFound();
                }

                TempData["ErrorMessage"] =
                    "Действие недоступно для текущего статуса заказа. " +
                    "Возможно, статус уже был изменён.";

                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = successMessage;

            if (openExecution)
            {
                return RedirectToAction(
                    nameof(Execute),
                    new { id });
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: ProductionOrders/SaveExecutionEventComment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveExecutionEventComment(
            int id,
            long eventId,
            string? comment)
        {
            if (comment != null && comment.Length > 1000)
            {
                return RedirectToExecutionWithError(
                    id,
                    "Комментарий не должен превышать 1000 символов.");
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.ReadCommitted);

            // Используем ту же блокировку строки заказа,
            // что и при изменении состояния выполнения.
            var order = await LockOrderAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Завершение физической работы не запрещает корректировку
            // комментариев, если заказ возвращён на корректировку факта.
            var canEditEventComments =
                order.StatusId == InExecutionStatusId ||
                order.StatusId == FactCorrectionStatusId;

            if (!canEditEventComments)
            {
                return RedirectToExecutionWithError(
                    id,
                    "Комментарии журнала можно изменять только " +
                    "в статусах «На исполнении» " +
                    "и «На корректировке факта».");
            }

            // Запись должна принадлежать именно этому заказу.
            var executionEvent =
                await _context.ProductionOrderExecutionEvents
                    .SingleOrDefaultAsync(x =>
                        x.Id == eventId &&
                        x.OrderId == id);

            if (executionEvent == null)
            {
                return NotFound();
            }

            // Изменяем только комментарий существующей записи.
            // Дата, состояние, принадлежность заказу и состав
            // записей журнала остаются неизменными.
            executionEvent.Comment =
                string.IsNullOrWhiteSpace(comment)
                    ? null
                    : comment.Trim();

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["SuccessMessage"] =
                "Комментарий к записи журнала сохранён.";

            return RedirectToAction(
                nameof(Execute),
                new { id });
        }
        
        // POST: ProductionOrders/RejectByOperator/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> RejectByOperator(int id)
        {
            return ChangeWorkflowStatusAsync(
                id: id,
                expectedStatusId: OperatorApprovalStatusId,
                nextStatusId: FactCorrectionStatusId,
                successMessage:
                    "Заказ возвращён на корректировку факта. " +
                    "Физическое выполнение остаётся завершённым.",
                openExecution: true);
        }
        // POST: ProductionOrders/CloseOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> CloseOrder(int id)
        {
            return ChangeWorkflowStatusAsync(
                id: id,
                expectedStatusId: OperatorApprovalStatusId,
                nextStatusId: CompletedStatusId,
                successMessage:
                    "Заказ закрыт и переведён в статус «Выполнен». " +
                    "Дальнейшее редактирование недоступно.",
                openExecution: true);
        }
        // POST: ProductionOrders/UpdateOrderDate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderDate(
            [FromForm] int id,
            [FromForm] string? orderDate,
            [FromForm] string? expectedOrderDate,
            [FromForm] int expectedStatusId)
        {
            if (!ModelState.IsValid || id <= 0)
            {
                return BadRequest(new
                {
                    message = "Некорректные параметры запроса."
                });
            }

            // Принимаем только yyyy-MM-dd или пустое значение.
            // Пустое значение будет дополнительно проверено по статусу.
            static bool TryReadDate(string? text, out DateOnly? value)
            {
                value = null;

                if (string.IsNullOrEmpty(text))
                {
                    return true;
                }

                if (!DateOnly.TryParseExact(
                        text,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var parsed))
                {
                    return false;
                }

                value = parsed;
                return true;
            }

            if (!TryReadDate(orderDate, out var newDate))
            {
                return BadRequest(new
                {
                    message = "Укажите корректную дату заказа."
                });
            }

            if (!TryReadDate(expectedOrderDate, out var expectedDate))
            {
                return BadRequest(new
                {
                    message = "Некорректная исходная дата. Обновите страницу."
                });
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.ReadCommitted);

            var order = await LockOrderAsync(id);

            if (order == null)
            {
                return NotFound(new
                {
                    message = "Заказ не найден. Обновите страницу."
                });
            }

            if (order.StatusId != DraftStatusId &&
                order.StatusId != CreatedStatusId)
            {
                return Conflict(new
                {
                    message =
                        "Дата недоступна для редактирования: " +
                        "статус заказа уже изменился. Обновите страницу."
                });
            }

            // Не перезаписываем дату, изменённую в другой вкладке
            // или другим пользователем после загрузки таблицы.
            if (order.StatusId != expectedStatusId ||
                order.OrderDate != expectedDate)
            {
                return Conflict(new
                {
                    message =
                        "Дата или статус заказа уже изменены. " +
                        "Обновите страницу и повторите действие."
                });
            }

            if (order.StatusId == CreatedStatusId && !newDate.HasValue)
            {
                return BadRequest(new
                {
                    message =
                        "У заказа со статусом «Создан» дата обязательна."
                });
            }

            // Меняем только дату. Остальные поля и статус сохраняются.
            order.OrderDate = newDate;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Json(new
            {
                orderDate = order.OrderDate?.ToString(
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture) ?? "",

                displayDate = order.OrderDate?.ToString(
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture) ?? "—",

                sortDate = order.OrderDate?.ToString(
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture) ?? ""
            });
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePlan(
    [FromBody] UpdateProductionOrderPlanRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Некорректные данные запроса."
                });
            }

            // Если ваш DbContext называется иначе,
            // замените _context на имя вашего поля.
            var order = await _context.ProductionOrders
                .SingleOrDefaultAsync(x => x.Id == request.Id);

            if (order == null)
            {
                return NotFound(new
                {
                    message = "Заказ не найден."
                });
            }

            // Проверяем статус независимо от наличия кнопки в представлении.
            if (order.StatusId != 1 && order.StatusId != 2)
            {
                return BadRequest(new
                {
                    message = "План можно изменять только у редактируемого или созданного заказа."
                });
            }

            decimal? plan = null;

            if (!string.IsNullOrWhiteSpace(request.Plan))
            {
                var normalizedPlan = request.Plan
                    .Trim()
                    .Replace(',', '.');

                var numberStyles =
                    NumberStyles.AllowDecimalPoint |
                    NumberStyles.AllowLeadingSign;

                if (!decimal.TryParse(
                        normalizedPlan,
                        numberStyles,
                        CultureInfo.InvariantCulture,
                        out var parsedPlan))
                {
                    return BadRequest(new
                    {
                        message = "Введите корректное количество."
                    });
                }

                if (parsedPlan < 0)
                {
                    return BadRequest(new
                    {
                        message = "План не может быть отрицательным."
                    });
                }

                plan = parsedPlan;
            }

            // Меняем только план. Статус, факт и дату не трогаем.
            order.Plan = plan;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                rawValue = order.Plan?.ToString(
                    CultureInfo.InvariantCulture) ?? "",

                displayValue = order.Plan?.ToString("N0") ?? "—"
            });
        }
        // МЕТКА1 Общий запрос со связанными данными.
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
                .Include(x => x.Status)
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
                    (x.SkuClassLink.SkuClass.Id
                        == SemiproductSkuClassId ||
                    x.SkuClassLink.SkuClass.Id
                        ==ProductSkuClassId));
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
                        "с классом SKU «Готовая продукция» или «Полуфабрикат».");
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
                && order.Plan.HasValue
                && order.Nomenclatura != null
                && order.Shift != null;
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