using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Verle.Data;
using Verle.Models;
using Verle.ViewModels;

namespace Verle.Controllers
{
    public class BrandsController : Controller
    {
        private readonly VerleContext _context;

        public BrandsController(VerleContext context)
        {
            _context = context;
        }

        // GET: Brands
        public async Task<IActionResult> Index()
        {
            var brands = await _context.Brands
                .AsNoTracking()
                .Include(x => x.BrandGroupLink)
                    .ThenInclude(x => x!.BrandGroup)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return View(brands);
        }

        // GET: Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .AsNoTracking()
                .Include(x => x.BrandGroupLink)
                    .ThenInclude(x => x!.BrandGroup)
                .FirstOrDefaultAsync(x => x.Id == id.Value);

            if (brand is null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // GET: Brands/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new BrandFormViewModel();

            await LoadBrandGroupOptionsAsync(model);

            return View(model);
        }

        // POST: Brands/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            BrandFormViewModel model)
        {
            ValidateBrandName(model);

            await ValidateBrandGroupAsync(model);

            if (!ModelState.IsValid)
            {
                await LoadBrandGroupOptionsAsync(model);

                return View(model);
            }

            var brand = new Brand
            {
                Name = model.Name.Trim()
            };

            ApplyBrandGroup(brand, model);

            _context.Brands.Add(brand);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Не удалось создать бренд. " +
                    "Обновите страницу и повторите попытку.");

                await LoadBrandGroupOptionsAsync(model);

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Brands/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .AsNoTracking()
                .Include(x => x.BrandGroupLink)
                .FirstOrDefaultAsync(x => x.Id == id.Value);

            if (brand is null)
            {
                return NotFound();
            }

            var model = new BrandFormViewModel
            {
                Id = brand.Id,
                Name = brand.Name,
                SelectedBrandGroupId =
                    brand.BrandGroupLink?.BrandGroupId
            };

            await LoadBrandGroupOptionsAsync(model);

            return View(model);
        }

        // POST: Brands/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            BrandFormViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            ValidateBrandName(model);

            await ValidateBrandGroupAsync(model);

            if (!ModelState.IsValid)
            {
                await LoadBrandGroupOptionsAsync(model);

                return View(model);
            }

            // AsNoTracking здесь не используем.
            // Сущность должна отслеживаться EF Core.
            var brand = await _context.Brands
                .Include(x => x.BrandGroupLink)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (brand is null)
            {
                return NotFound();
            }

            brand.Name = model.Name.Trim();

            ApplyBrandGroup(brand, model);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.Brands
                    .AnyAsync(x => x.Id == id);

                if (!exists)
                {
                    return NotFound();
                }

                ModelState.AddModelError(
                    string.Empty,
                    "Бренд был изменён другим пользователем. " +
                    "Обновите страницу и повторите попытку.");

                await LoadBrandGroupOptionsAsync(model);

                return View(model);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Не удалось сохранить изменения. " +
                    "Обновите страницу и повторите попытку.");

                await LoadBrandGroupOptionsAsync(model);

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Brands/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .AsNoTracking()
                .Include(x => x.BrandGroupLink)
                    .ThenInclude(x => x!.BrandGroup)
                .FirstOrDefaultAsync(x => x.Id == id.Value);

            if (brand is null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brand = await _context.Brands
                .FirstOrDefaultAsync(x => x.Id == id);

            if (brand is null)
            {
                return RedirectToAction(nameof(Index));
            }

            _context.Brands.Remove(brand);

            try
            {
                // Связь BrandBrandGroupLink должна удаляться каскадно.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Не удалось удалить бренд. " +
                    "Проверьте, используется ли он другими записями.");

                var brandForView = await _context.Brands
                    .AsNoTracking()
                    .Include(x => x.BrandGroupLink)
                        .ThenInclude(x => x!.BrandGroup)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (brandForView is null)
                {
                    return RedirectToAction(nameof(Index));
                }

                return View("Delete", brandForView);
            }

            return RedirectToAction(nameof(Index));
        }

        // Заполняет выпадающий список групп брендов.
        private async Task LoadBrandGroupOptionsAsync(
            BrandFormViewModel model)
        {
            model.BrandGroupOptions = await _context.BrandGroups
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

        // Проверяет, что выбранная группа существует.
        private async Task ValidateBrandGroupAsync(
            BrandFormViewModel model)
        {
            if (!model.SelectedBrandGroupId.HasValue)
            {
                return;
            }

            var exists = await _context.BrandGroups
                .AnyAsync(x =>
                    x.Id == model.SelectedBrandGroupId.Value);

            if (!exists)
            {
                ModelState.AddModelError(
                    nameof(model.SelectedBrandGroupId),
                    "Выбранная группа брендов не существует.");
            }
        }

        // Проверяет название бренда, включая строку из пробелов.
        private void ValidateBrandName(
            BrandFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                var nameState = ModelState[nameof(model.Name)];

                if (nameState is null || nameState.Errors.Count == 0)
                {
                    ModelState.AddModelError(
                        nameof(model.Name),
                        "Введите название бренда.");
                }
            }
        }

        // Создаёт, изменяет или удаляет связь бренда с группой.
        private void ApplyBrandGroup(
            Brand brand,
            BrandFormViewModel model)
        {
            if (model.SelectedBrandGroupId is int groupId)
            {
                if (brand.BrandGroupLink is null)
                {
                    brand.BrandGroupLink = new BrandBrandGroupLink
                    {
                        Brand = brand,
                        BrandGroupId = groupId
                    };
                }
                else
                {
                    brand.BrandGroupLink.BrandGroupId = groupId;
                }
            }
            else if (brand.BrandGroupLink is not null)
            {
                _context.BrandBrandGroupLinks.Remove(
                    brand.BrandGroupLink);

                brand.BrandGroupLink = null;
            }
        }
    }
}