using System.Data.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Verle.Data;
using Verle.Models;
using Verle.Services;

namespace Verle.Controllers;

public class SalesController : Controller
{
    private readonly VerleContext _context;
    private readonly SalesImportService _importService;
    private readonly ILogger<SalesController> _logger;

    public SalesController(
        VerleContext context,
        SalesImportService importService,
        ILogger<SalesController> logger)
    {
        _context = context;
        _importService = importService;
        _logger = logger;
    }

    // GET: Sales
    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var sales = await _context.Sales
            .AsNoTracking()
            .Include(x => x.Counterpart)
            .Include(x => x.Nomenclatura)
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);

        return View(sales);
    }

    // GET: Sales/Import
    [HttpGet]
    public IActionResult Import()
    {
        return View(new SalesImportViewModel());
    }

    // POST: Sales/Import
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        var result = new SalesImportViewModel();

        if (!ModelState.IsValid)
        {
            result.Errors.Add(
                "Не удалось обработать форму загрузки. " +
                "Выберите CSV-файл и повторите отправку.");

            return View(result);
        }

        if (file is null || file.Length == 0)
        {
            result.Errors.Add(
                "Выберите непустой CSV-файл для импорта.");

            return View(result);
        }

        try
        {
            // Проверяем файл и справочники.
            // На этом этапе продажи не сохраняются.
            result = await _importService.ValidateAsync(
                file,
                cancellationToken);

            if (!result.CanImport)
            {
                return View(result);
            }

            // Сервис повторно проверяет актуальность данных
            // и сохраняет строки в транзакции.
            await _importService.ImportAsync(
                file,
                cancellationToken);

            // RowCount — число проверенных строк,
            // а не число фактически добавленных продаж.
            TempData["SuccessMessage"] =
                "Обработка файла продаж успешно завершена. " +
                $"Строк в проверенном файле: {result.RowCount}. " +
                "Если включена защита от повторного импорта, " +
                "уже загруженные записи не добавляются повторно.";

            return RedirectToAction(nameof(Index));
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            // Не превращаем отмену запроса в ошибку импорта.
            throw;
        }
        catch (DbUpdateException exception)
        {
            // Ошибки сохранения через SaveChangesAsync.
            _logger.LogError(
                exception,
                "Ошибка EF Core при обработке файла продаж.");

            result.Errors.Add(
                "Не удалось завершить импорт из-за ошибки " +
                "сохранения данных. Обратитесь к администратору.");
        }
        catch (DbException exception)
        {
            // В том числе ошибки PostgreSQL при выполнении
            // ExecuteSqlInterpolatedAsync.
            _logger.LogError(
                exception,
                "Ошибка базы данных при обработке файла продаж.");

            result.Errors.Add(
                "Не удалось завершить обработку файла " +
                "из-за ошибки базы данных. " +
                "Обратитесь к администратору.");
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogWarning(
                exception,
                "Обработка файла продаж не завершена.");

            result.Errors.Add(
                "Не удалось завершить импорт. " +
                "Данные могли измениться после предварительной " +
                "проверки. Повторите проверку файла. " +
                "Если ошибка сохраняется, " +
                "обратитесь к администратору.");
        }

        return View(result);
    }
}