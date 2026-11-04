using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Verle.Data;
using Verle.Services;
using Verle.ViewModels;

namespace Verle.Controllers;

public class ProductionSpecificationsController : Controller
{
    private readonly VerleContext _context;
    private readonly ProductionSpecificationImportService _importService;
    private readonly ILogger<ProductionSpecificationsController> _logger;

    public ProductionSpecificationsController(
        VerleContext context,
        ProductionSpecificationImportService importService,
        ILogger<ProductionSpecificationsController> logger)
    {
        _context = context;
        _importService = importService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var items = await _context.ProductionSpecifications
            .AsNoTracking()
            .Include(x => x.NomenclaturaOutput)
            .Include(x => x.NomenclaturaInput)
            .OrderBy(x => x.NomenclaturaOutput.Name)
            .ThenBy(x => x.NomenclaturaInput.Name)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Import(
        CancellationToken cancellationToken)
    {
        var model = new ProductionSpecificationImportViewModel();

        await FillOptionsAsync(model, cancellationToken);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Import(
        IFormFile? file,
        string? command,
        CancellationToken cancellationToken)
    {
        ProductionSpecificationImportViewModel model;

        try
        {
            if (command == "validate")
            {
                model = await _importService.ValidateAsync(
                    file,
                    cancellationToken);
            }
            else if (command == "import")
            {
                model = await _importService.ImportAsync(
                    file,
                    cancellationToken);
            }
            else
            {
                return BadRequest();
            }
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(
                exception,
                "Ошибка сохранения производственных спецификаций.");

            model = new ProductionSpecificationImportViewModel
            {
                Checked = true
            };

            model.Errors.Add(
                "Не удалось сохранить спецификации. " +
                "Ни одна строка этого импорта не сохранена. " +
                "Возможно, связанные справочники изменились. " +
                "Повторно выберите файл и выполните проверку.");
        }
        catch (IOException exception)
        {
            _logger.LogError(
                exception,
                "Ошибка чтения CSV производственных спецификаций.");

            model = new ProductionSpecificationImportViewModel
            {
                Checked = true
            };

            model.Errors.Add(
                "Не удалось прочитать загруженный файл. " +
                "Выберите его повторно.");
        }

        await FillOptionsAsync(model, cancellationToken);

        return View(model);
    }

    private async Task FillOptionsAsync(
        ProductionSpecificationImportViewModel model,
        CancellationToken cancellationToken)
    {
        var classes = await _context.NomenclaturaClasses
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(x => new { x.Id, x.Name })
            .ToListAsync(cancellationToken);

        model.Options.ClassOptions =
            new SelectList(classes, "Id", "Name");

        var skuClasses = await _context.SkuClasses
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(x => new { x.Id, x.Name })
            .ToListAsync(cancellationToken);

        model.Options.SkuClassOptions =
            new SelectList(skuClasses, "Id", "Name");

        var productTypes = await _context.ProductTypes
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(x => new { x.Id, x.Name })
            .ToListAsync(cancellationToken);

        model.Options.ProductTypeOptions =
            new SelectList(productTypes, "Id", "Name");

        var brands = await _context.Brands
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(x => new { x.Id, x.Name })
            .ToListAsync(cancellationToken);

        model.Options.BrandOptions =
            new SelectList(brands, "Id", "Name");
    }
}