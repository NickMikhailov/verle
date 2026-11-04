using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Verle.Data;
using Verle.Models;
using Verle.ViewModels;

namespace Verle.Controllers
{
    public class NomenclaturasController : Controller
    {
        private const int GoodsClassId = 1;
        private const int FinishedProductSkuClassId = 1; 
        private readonly VerleContext _context;
        private readonly ILogger<NomenclaturasController> _logger;

        public NomenclaturasController(
            VerleContext context,
            ILogger<NomenclaturasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Запрос для редактирования.
        // Загружаются только связующие записи.
        private IQueryable<Nomenclatura> QueryWithLinks()
        {
            return _context.Nomenclaturas
                .Include(x => x.ClassLink)
                .Include(x => x.SkuClassLink)
                .Include(x => x.ProductTypeLink)
                .Include(x => x.BrandLink);
        }

        // Запрос для списка, просмотра и удаления.
        // Загружаются связующие записи и названия справочников.
        private IQueryable<Nomenclatura> QueryWithClassifications()
        {
            return _context.Nomenclaturas
                .Include(x => x.ClassLink)
                    .ThenInclude(x => x!.NomenclaturaClass)

                .Include(x => x.SkuClassLink)
                    .ThenInclude(x => x!.SkuClass)

                .Include(x => x.ProductTypeLink)
                    .ThenInclude(x => x!.ProductType)

                .Include(x => x.BrandLink)
                    .ThenInclude(x => x!.Brand);
        }

        // GET: Nomenclaturas
        public async Task<IActionResult> Index()
        {
            var items = await QueryWithClassifications()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return View(items);
        }

        // GET: Nomenclaturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var item = await QueryWithClassifications()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id.Value);

            if (item is null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Nomenclaturas/Create
        [HttpGet]
        public async Task<IActionResult> Create(
            string? id1C,
            string? name)
        {
            var model = new NomenclaturaFormViewModel
            {
                Id1C = id1C?.Trim() ?? string.Empty,
                Name = name?.Trim() ?? string.Empty
            };

            // Загружаем классы, классы SKU, типы продукта и бренды.
            await FillOptionsAsync(model);

            return View(model);
        }

        // POST: Nomenclaturas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "Name," +
                "Id1C," +
                "SelectedClassId," +
                "SelectedSkuClassId," +
                "SelectedProductTypeId," +
                "SelectedBrandId")]
            NomenclaturaFormViewModel model)
        {
            await ValidateFormAsync(model);

            if (!ModelState.IsValid)
            {
                await FillOptionsAsync(model);

                return View(model);
            }

            var item = new Nomenclatura
            {
                Name = NormalizeName(model.Name),
                Id1C = NormalizeId1C(model.Id1C)!
            };

            ApplyClassifications(item, model);

            _context.Nomenclaturas.Add(item);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                await AddSaveErrorAsync(
                    exception,
                    model,
                    currentId: null);

                await FillOptionsAsync(model);

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Nomenclaturas/CreateFromImport
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromImport(
            [Bind(
        "Name," +
        "Id1C," +
        "SelectedClassId," +
        "SelectedSkuClassId," +
        "SelectedProductTypeId," +
        "SelectedBrandId")]
        NomenclaturaFormViewModel model)
        {
            // Та же проверка, что используется на странице Create.
            await ValidateFormAsync(model);

            if (!ModelState.IsValid)
            {
                return CreateFromImportErrors(
                    StatusCodes.Status400BadRequest);
            }

            var item = new Nomenclatura
            {
                Name = NormalizeName(model.Name),
                Id1C = NormalizeId1C(model.Id1C)!
            };

            // То же назначение классификаций, что и в Create.
            ApplyClassifications(item, model);

            _context.Nomenclaturas.Add(item);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                // Используем существующую обработку ошибок,
                // включая конфликт при одновременном добавлении.
                await AddSaveErrorAsync(
                    exception,
                    model,
                    currentId: null);

                if (ModelState.IsValid)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Не удалось сохранить номенклатуру.");
                }

                return CreateFromImportErrors(
                    StatusCodes.Status409Conflict);
            }

            // Вместо перехода на другую страницу возвращаем JSON.
            return Json(new
            {
                success = true,
                id = item.Id
            });
        }
        private IActionResult CreateFromImportErrors(int statusCode)
        {
            var errors = ModelState
                .Where(entry =>
                    entry.Value is not null &&
                    entry.Value.Errors.Count > 0)
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value!.Errors
                        .Select(error =>
                            string.IsNullOrWhiteSpace(error.ErrorMessage)
                                ? "Некорректное значение поля."
                                : error.ErrorMessage)
                        .ToArray());

            return StatusCode(statusCode, new
            {
                success = false,
                errors
            });
        }
        // GET: Nomenclaturas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var item = await QueryWithLinks()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id.Value);

            if (item is null)
            {
                return NotFound();
            }

            var model = new NomenclaturaFormViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Id1C = item.Id1C,

                SelectedClassId =
                    item.ClassLink?.NomenclaturaClassId,

                SelectedSkuClassId =
                    item.SkuClassLink?.SkuClassId,

                SelectedProductTypeId =
                    item.ProductTypeLink?.ProductTypeId,

                SelectedBrandId =
                    item.BrandLink?.BrandId
            };

            await FillOptionsAsync(model);

            return View(model);
        }

        // POST: Nomenclaturas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind(
                "Id," +
                "Name," +
                "Id1C," +
                "SelectedClassId," +
                "SelectedSkuClassId," +
                "SelectedProductTypeId," +
                "SelectedBrandId")]
            NomenclaturaFormViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            // Загружаем отслеживаемую сущность,
            // потому что ниже будут изменяться её свойства.
            var item = await QueryWithLinks()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item is null)
            {
                return NotFound();
            }

            await ValidateFormAsync(
                model,
                currentId: id);

            if (!ModelState.IsValid)
            {
                await FillOptionsAsync(model);

                return View(model);
            }

            item.Name = NormalizeName(model.Name);
            item.Id1C = NormalizeId1C(model.Id1C)!;

            ApplyClassifications(item, model);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                _logger.LogWarning(
                    exception,
                    "Конфликт при изменении номенклатуры с Id {Id}.",
                    id);

                var exists = await _context.Nomenclaturas
                    .AnyAsync(x => x.Id == id);

                if (!exists)
                {
                    return NotFound();
                }

                ModelState.AddModelError(
                    string.Empty,
                    "Запись была изменена другим пользователем. " +
                    "Обновите страницу и повторите изменения.");

                await FillOptionsAsync(model);

                return View(model);
            }
            catch (DbUpdateException exception)
            {
                await AddSaveErrorAsync(
                    exception,
                    model,
                    currentId: id);

                await FillOptionsAsync(model);

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Nomenclaturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var item = await QueryWithClassifications()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id.Value);

            if (item is null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Nomenclaturas/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Nomenclaturas
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item is null)
            {
                return RedirectToAction(nameof(Index));
            }

            _context.Nomenclaturas.Remove(item);

            try
            {
                // Связующие записи должны удаляться каскадно
                // благодаря настройке DeleteBehavior.Cascade.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(
                    exception,
                    "Ошибка удаления номенклатуры с Id {Id}.",
                    id);

                ModelState.AddModelError(
                    string.Empty,
                    "Не удалось удалить номенклатуру. " +
                    "Возможно, она используется другими записями.");

                var viewModel = await QueryWithClassifications()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (viewModel is null)
                {
                    return RedirectToAction(nameof(Index));
                }

                return View("Delete", viewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        // Заполняет все выпадающие списки формы.
        private async Task FillOptionsAsync(
            NomenclaturaFormViewModel model)
        {
            var classes = await _context.NomenclaturaClasses
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .ToListAsync();

            model.ClassOptions = new SelectList(
                classes,
                nameof(NomenclaturaClass.Id),
                nameof(NomenclaturaClass.Name),
                model.SelectedClassId);

            var skuClasses = await _context.SkuClasses
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .ToListAsync();

            model.SkuClassOptions = new SelectList(
                skuClasses,
                nameof(SkuClass.Id),
                nameof(SkuClass.Name),
                model.SelectedSkuClassId);

            var productTypes = await _context.ProductTypes
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .ToListAsync();

            model.ProductTypeOptions = new SelectList(
                productTypes,
                nameof(ProductType.Id),
                nameof(ProductType.Name),
                model.SelectedProductTypeId);

            var brands = await _context.Brands
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .ToListAsync();

            model.BrandOptions = new SelectList(
                brands,
                nameof(Brand.Id),
                nameof(Brand.Name),
                model.SelectedBrandId);
        }

        // Проверяет обязательные поля, уникальность Id1C
        // и существование выбранных классификаций.
        // Очищает классификации, неприменимые к выбранным родительским значениям.
        // Используется во всех сценариях создания и редактирования.
        private void NormalizeClassifications(
            NomenclaturaFormViewModel model)
        {
            var isGoods = model.SelectedClassId == GoodsClassId;

            if (!isGoods)
            {
                model.SelectedSkuClassId = null;

                // При повторном отображении формы Tag Helpers берут
                // отправленное значение из ModelState раньше, чем из модели.
                // Удаляем его, чтобы старый SKU не появился снова.
                ModelState.Remove(
                    nameof(NomenclaturaFormViewModel.SelectedSkuClassId));
            }

            var isFinishedProduct =
                isGoods &&
                model.SelectedSkuClassId == FinishedProductSkuClassId;

            if (!isFinishedProduct)
            {
                model.SelectedProductTypeId = null;
                model.SelectedBrandId = null;

                ModelState.Remove(
                    nameof(NomenclaturaFormViewModel.SelectedProductTypeId));

                ModelState.Remove(
                    nameof(NomenclaturaFormViewModel.SelectedBrandId));
            }
        }
        private async Task ValidateFormAsync(
            NomenclaturaFormViewModel model,
            int? currentId = null)
        {
            NormalizeClassifications(model);

            var name = NormalizeName(model.Name);
            var id1C = NormalizeId1C(model.Id1C);

            if (string.IsNullOrWhiteSpace(name))
            {
                AddModelErrorIfMissing(
                    nameof(model.Name),
                    "Введите название.");
            }

            if (string.IsNullOrWhiteSpace(id1C))
            {
                AddModelErrorIfMissing(
                    nameof(model.Id1C),
                    "ID в 1С не может быть пустым.");
            }
            else
            {
                var id1CExists = await IsId1CUsedAsync(
                    id1C,
                    currentId);

                if (id1CExists)
                {
                    AddModelErrorIfMissing(
                        nameof(model.Id1C),
                        "Такой ID в 1С уже используется.");
                }
            }

            if (model.SelectedClassId is int classId)
            {
                var exists = await _context.NomenclaturaClasses
                    .AnyAsync(x => x.Id == classId);

                if (!exists)
                {
                    ModelState.AddModelError(
                        nameof(model.SelectedClassId),
                        "Выбранный класс номенклатуры не существует.");
                }
            }

            if (model.SelectedSkuClassId is int skuClassId)
            {
                var exists = await _context.SkuClasses
                    .AnyAsync(x => x.Id == skuClassId);

                if (!exists)
                {
                    ModelState.AddModelError(
                        nameof(model.SelectedSkuClassId),
                        "Выбранный класс SKU не существует.");
                }
            }

            if (model.SelectedProductTypeId is int productTypeId)
            {
                var exists = await _context.ProductTypes
                    .AnyAsync(x => x.Id == productTypeId);

                if (!exists)
                {
                    ModelState.AddModelError(
                        nameof(model.SelectedProductTypeId),
                        "Выбранный тип продукта не существует.");
                }
            }

            if (model.SelectedBrandId is int brandId)
            {
                var exists = await _context.Brands
                    .AnyAsync(x => x.Id == brandId);

                if (!exists)
                {
                    ModelState.AddModelError(
                        nameof(model.SelectedBrandId),
                        "Выбранный бренд не существует.");
                }
            }
        }

        // Проверка, используется ли Id1C другой записью.
        private async Task<bool> IsId1CUsedAsync(
            string id1C,
            int? currentId)
        {
            var query = _context.Nomenclaturas
                .Where(x => x.Id1C == id1C);

            if (currentId.HasValue)
            {
                query = query.Where(x => x.Id != currentId.Value);
            }

            return await query.AnyAsync();
        }

        // Добавляет классификации при создании,
        // изменяет их при редактировании
        // и удаляет связь при выборе пустого значения.
        private void ApplyClassifications(
            Nomenclatura item,
            NomenclaturaFormViewModel model)
        {
            NormalizeClassifications(model);
            // Основной класс номенклатуры.
            if (model.SelectedClassId is int classId)
            {
                if (item.ClassLink is null)
                {
                    item.ClassLink = new NomenclaturaClassLink
                    {
                        Nomenclatura = item,
                        NomenclaturaClassId = classId
                    };
                }
                else
                {
                    item.ClassLink.NomenclaturaClassId = classId;
                }
            }
            else if (item.ClassLink is not null)
            {
                _context.NomenclaturaClassLinks.Remove(
                    item.ClassLink);

                item.ClassLink = null;
            }

            // Класс SKU.
            if (model.SelectedSkuClassId is int skuClassId)
            {
                if (item.SkuClassLink is null)
                {
                    item.SkuClassLink = new NomenclaturaSkuClassLink
                    {
                        Nomenclatura = item,
                        SkuClassId = skuClassId
                    };
                }
                else
                {
                    item.SkuClassLink.SkuClassId = skuClassId;
                }
            }
            else if (item.SkuClassLink is not null)
            {
                _context.NomenclaturaSkuClassLinks.Remove(
                    item.SkuClassLink);

                item.SkuClassLink = null;
            }

            // Тип продукта.
            if (model.SelectedProductTypeId is int productTypeId)
            {
                if (item.ProductTypeLink is null)
                {
                    item.ProductTypeLink =
                        new NomenclaturaProductTypeLink
                        {
                            Nomenclatura = item,
                            ProductTypeId = productTypeId
                        };
                }
                else
                {
                    item.ProductTypeLink.ProductTypeId =
                        productTypeId;
                }
            }
            else if (item.ProductTypeLink is not null)
            {
                _context.NomenclaturaProductTypeLinks.Remove(
                    item.ProductTypeLink);

                item.ProductTypeLink = null;
            }

            // Бренд.
            if (model.SelectedBrandId is int brandId)
            {
                if (item.BrandLink is null)
                {
                    item.BrandLink = new NomenclaturaBrandLink
                    {
                        Nomenclatura = item,
                        BrandId = brandId
                    };
                }
                else
                {
                    item.BrandLink.BrandId = brandId;
                }
            }
            else if (item.BrandLink is not null)
            {
                _context.NomenclaturaBrandLinks.Remove(
                    item.BrandLink);

                item.BrandLink = null;
            }
        }

        // Обработка ошибки сохранения.
        // Если ошибка возникла из-за уникального Id1C,
        // показываем конкретное сообщение.
        private async Task AddSaveErrorAsync(
            DbUpdateException exception,
            NomenclaturaFormViewModel model,
            int? currentId)
        {
            _logger.LogError(
                exception,
                "Ошибка сохранения номенклатуры.");

            var id1C = NormalizeId1C(model.Id1C);

            if (!string.IsNullOrWhiteSpace(id1C))
            {
                var id1CExists = await IsId1CUsedAsync(
                    id1C,
                    currentId);

                if (id1CExists)
                {
                    ModelState.AddModelError(
                        nameof(model.Id1C),
                        "Такой ID в 1С уже используется.");

                    return;
                }
            }

            ModelState.AddModelError(
                string.Empty,
                "Не удалось сохранить номенклатуру. " +
                "Проверьте введённые данные и повторите попытку.");
        }

        private void AddModelErrorIfMissing(
            string key,
            string message)
        {
            if (!ModelState.TryGetValue(key, out var entry)
                || entry.Errors.Count == 0)
            {
                ModelState.AddModelError(key, message);
            }
        }

        private static string NormalizeName(string? value)
        {
            return value?.Trim() ?? string.Empty;
        }

        private static string? NormalizeId1C(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }
    }
}