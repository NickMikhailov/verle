using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Verle.Data;
using Verle.Models;

namespace Verle.Services;

public class SalesImportService
{
    private readonly VerleContext _context;

    public SalesImportService(VerleContext context)
    {
        _context = context;
    }

    // Только проверка файла и справочников.
    // Ничего не сохраняет в базу.
    public async Task<SalesImportViewModel> ValidateAsync(
        IFormFile? file,
        CancellationToken cancellationToken = default)
    {
        var prepared = await PrepareAsync(
            file,
            cancellationToken);

        return prepared.Validation;
    }

    // Повторная актуальная проверка и сохранение всех строк.
    public async Task ImportAsync(
    IFormFile file,
    CancellationToken cancellationToken = default)
    {
        var prepared = await PrepareAsync(
            file,
            cancellationToken);

        if (!prepared.Validation.CanImport)
        {
            throw new InvalidOperationException(
                "Импорт невозможен. Исправьте ошибки файла " +
                "или добавьте отсутствующие записи справочников.");
        }

        // Все строки могли оказаться уже загруженными.
        // Это успешный повторный импорт без новых записей.
        if (prepared.Rows.Count == 0)
        {
            return;
        }

        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    cancellationToken);

            foreach (var row in prepared.Rows)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var counterpartId =
                    prepared.CounterpartIds[row.CounterpartId1C];

                var nomenclaturaId =
                    prepared.NomenclaturaIds[row.NomenclaturaId1C];

                // Значения передаются параметрами, а не
                // подставляются напрямую в текст SQL.
                //
                // Если такой ключ появился после проверки,
                // например из параллельного импорта,
                // PostgreSQL просто пропустит запись.
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"""
                INSERT INTO public.sales
                (
                    "counterpartId",
                    "counterpartId1C",
                    "counterpartName1C",
                    "nomenclaturaId",
                    "nomenclaturaId1C",
                    "nomenclaturaName1C",
                    "date",
                    "document",
                    "quantity",
                    "revenue",
                    "cost",
                    "grossProfit",
                    "margin"
                )
                VALUES
                (
                    {counterpartId},
                    {row.CounterpartId1C},
                    {row.CounterpartName1C},
                    {nomenclaturaId},
                    {row.NomenclaturaId1C},
                    {row.NomenclaturaName1C},
                    {row.Date},
                    {row.Document},
                    {row.Quantity},
                    {row.Revenue},
                    {row.Cost},
                    {row.GrossProfit},
                    {row.Margin}
                )
                ON CONFLICT
                (
                    "counterpartId1C",
                    "document",
                    "nomenclaturaId1C"
                )
                DO NOTHING;
                """,
                    cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        });
    }

    private async Task<PreparedImport> PrepareAsync(
    IFormFile? file,
    CancellationToken cancellationToken)
    {
        var prepared = new PreparedImport();
        var result = prepared.Validation;

        cancellationToken.ThrowIfCancellationRequested();

        if (file is null || file.Length == 0)
        {
            result.Errors.Add("Файл не выбран или он пустой.");
            return prepared;
        }

        // ReadRowsAsync:
        // 1. Записывает общее количество строк CSV в result.RowCount.
        // 2. Пропускает повторные строки и продажи, уже существующие в базе.
        // 3. Возвращает только новые строки, прошедшие ParseRow.
        prepared.Rows = await ReadRowsAsync(
            file,
            result,
            cancellationToken);

        // Не присваиваем result.RowCount = prepared.Rows.Count:
        // количество новых продаж может быть меньше числа строк CSV
        // или равняться нулю при повторной загрузке.
        if (result.Errors.Count > 0)
        {
            return prepared;
        }

        if (result.RowCount == 0)
        {
            result.Errors.Add("В файле нет строк продаж.");
            return prepared;
        }

        // Все строки пропущены как уже существующие.
        // Ошибки нет: добавлять и проверять нечего.
        if (prepared.Rows.Count == 0)
        {
            return prepared;
        }

        var counterpartCodes = prepared.Rows
            .Select(row => row.CounterpartId1C)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        var nomenclaturaCodes = prepared.Rows
            .Select(row => row.NomenclaturaId1C)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        // Проверяем справочники только для новых продаж.
        var counterparts = await _context.Counterparts
            .AsNoTracking()
            .Where(item => counterpartCodes.Contains(item.Id1C))
            .Select(item => new
            {
                item.Id,
                item.Id1C
            })
            .ToListAsync(cancellationToken);

        var nomenclaturas = await _context.Nomenclaturas
            .AsNoTracking()
            .Where(item => nomenclaturaCodes.Contains(item.Id1C))
            .Select(item => new
            {
                item.Id,
                item.Id1C
            })
            .ToListAsync(cancellationToken);

        // Код 1С должен однозначно определять запись справочника.
        var duplicateCounterpartCodes = counterparts
            .GroupBy(item => item.Id1C, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .OrderBy(code => code, StringComparer.Ordinal)
            .ToList();

        foreach (var code in duplicateCounterpartCodes)
        {
            result.Errors.Add(
                $"В справочнике контрагентов код 1С " +
                $"«{code}» указан у нескольких записей.");
        }

        var duplicateNomenclaturaCodes = nomenclaturas
            .GroupBy(item => item.Id1C, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .OrderBy(code => code, StringComparer.Ordinal)
            .ToList();

        foreach (var code in duplicateNomenclaturaCodes)
        {
            result.Errors.Add(
                $"В справочнике номенклатуры код 1С " +
                $"«{code}» указан у нескольких записей.");
        }

        // При неоднозначных кодах нельзя создавать словари
        // и определять внутренние идентификаторы продаж.
        if (result.Errors.Count > 0)
        {
            return prepared;
        }

        prepared.CounterpartIds = counterparts
            .ToDictionary(
                item => item.Id1C,
                item => item.Id,
                StringComparer.Ordinal);

        prepared.NomenclaturaIds = nomenclaturas
            .ToDictionary(
                item => item.Id1C,
                item => item.Id,
                StringComparer.Ordinal);

        result.UnknownCounterparts = prepared.Rows
            .Where(row =>
                !prepared.CounterpartIds.ContainsKey(
                    row.CounterpartId1C))
            .GroupBy(
                row => row.CounterpartId1C,
                StringComparer.Ordinal)
            .Select(group => new UnknownReferenceViewModel
            {
                Id1C = group.Key,
                Name1C = group.First().CounterpartName1C
            })
            .OrderBy(item => item.Id1C, StringComparer.Ordinal)
            .ToList();

        result.UnknownNomenclaturas = prepared.Rows
            .Where(row =>
                !prepared.NomenclaturaIds.ContainsKey(
                    row.NomenclaturaId1C))
            .GroupBy(
                row => row.NomenclaturaId1C,
                StringComparer.Ordinal)
            .Select(group => new UnknownReferenceViewModel
            {
                Id1C = group.Key,
                Name1C = group.First().NomenclaturaName1C
            })
            .OrderBy(item => item.Id1C, StringComparer.Ordinal)
            .ToList();

        return prepared;
    }

    private async Task<List<SaleImportRow>> ReadRowsAsync(
    IFormFile file,
    SalesImportViewModel result,
    CancellationToken cancellationToken)
    {
        var rows = new List<SaleImportRow>();
        var candidates = new List<RawCandidate>();

        // Record сравнивает ключи по значениям.
        // Строки сравниваются с учётом регистра.
        var seenKeys = new HashSet<ImportKey>();

        result.RowCount = 0;

        var configuration = new CsvConfiguration(
            CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            IgnoreBlankLines = true,
            ExceptionMessagesContainRawData = false
        };

        try
        {
            using var stream = file.OpenReadStream();

            using var reader = new StreamReader(
                stream,
                new UTF8Encoding(
                    encoderShouldEmitUTF8Identifier: false,
                    throwOnInvalidBytes: true),
                detectEncodingFromByteOrderMarks: true);

            using var csv = new CsvReader(
                reader,
                configuration);

            csv.Context.RegisterClassMap<SaleImportRawMap>();

            cancellationToken.ThrowIfCancellationRequested();

            if (!await csv.ReadAsync())
            {
                result.Errors.Add(
                    "CSV-файл не содержит заголовка.");

                return rows;
            }

            csv.ReadHeader();
            csv.ValidateHeader<SaleImportRaw>();

            while (await csv.ReadAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();

                result.RowCount++;

                var lineNumber = csv.Parser.RawRow;
                var raw = csv.GetRecord<SaleImportRaw>();

                if (raw is null)
                {
                    result.Errors.Add(
                        $"Строка {lineNumber}: " +
                        "не удалось прочитать запись.");

                    continue;
                }

                try
                {
                    // Сначала проверяем только поля ключа.
                    var key = new ImportKey(
                        Required(
                            raw.CounterpartId1C,
                            "counterpartId1C",
                            100),
                        Required(
                            raw.Document,
                            "document",
                            500),
                        Required(
                            raw.NomenclaturaId1C,
                            "nomenclaturaId1C",
                            100));

                    // Ключ отмечается до проверки остальных полей.
                    // Поэтому более поздняя строка никогда
                    // не заменит первую, даже если первая ошибочная.
                    if (!seenKeys.Add(key))
                    {
                        continue;
                    }

                    candidates.Add(
                        new RawCandidate(
                            raw,
                            lineNumber,
                            key));
                }
                catch (FormatException exception)
                {
                    result.Errors.Add(
                        $"Строка {lineNumber}: {exception.Message}");
                }
            }
        }
        catch (HeaderValidationException)
        {
            result.Errors.Add(
                "Неверный заголовок CSV. Ожидаются колонки: " +
                "counterpartId1C;counterpartName1C;date;document;" +
                "nomenclaturaId1C;nomenclaturaName1C;quantity;" +
                "revenue;cost;grossProfit;margin.");
        }
        catch (CsvHelperException)
        {
            result.Errors.Add(
                "Некорректная структура CSV. " +
                "Проверьте разделитель «;», " +
                "число полей и правильность кавычек.");
        }
        catch (DecoderFallbackException)
        {
            result.Errors.Add(
                "Не удалось прочитать кодировку файла. " +
                "Сохраните CSV в UTF-8 и повторите загрузку.");
        }
        catch (IOException)
        {
            result.Errors.Add(
                "Не удалось прочитать файл. Повторите загрузку.");
        }

        if (result.Errors.Count > 0 || candidates.Count == 0)
        {
            return rows;
        }

        var existingKeys = new HashSet<ImportKey>();

        // Проверяем наличие продаж пакетами,
        // а не отдельным запросом на каждую строку.
        foreach (var batch in candidates.Chunk(300))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var counterpartCodes = batch
                .Select(x => x.Key.CounterpartId1C)
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            var documents = batch
                .Select(x => x.Key.Document)
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            var nomenclaturaCodes = batch
                .Select(x => x.Key.NomenclaturaId1C)
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            var found = await _context.Sales
                .AsNoTracking()
                .Where(x =>
                    counterpartCodes.Contains(x.CounterpartId1C)
                    && documents.Contains(x.Document)
                    && nomenclaturaCodes.Contains(x.NomenclaturaId1C))
                .Select(x => new
                {
                    x.CounterpartId1C,
                    x.Document,
                    x.NomenclaturaId1C
                })
                .ToListAsync(cancellationToken);

            // Запрос может вернуть дополнительные сочетания
            // из перечисленных значений. Для проверки оставляем
            // только точные ключи текущего пакета.
            var batchKeys = batch
                .Select(x => x.Key)
                .ToHashSet();

            foreach (var item in found)
            {
                var key = new ImportKey(
                    item.CounterpartId1C,
                    item.Document,
                    item.NomenclaturaId1C);

                if (batchKeys.Contains(key))
                {
                    existingKeys.Add(key);
                }
            }
        }

        foreach (var candidate in candidates)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Продажа уже есть в базе:
            // не проверяем остальные поля и не обновляем запись.
            if (existingKeys.Contains(candidate.Key))
            {
                continue;
            }

            try
            {
                rows.Add(ParseRow(candidate.Raw));
            }
            catch (FormatException exception)
            {
                result.Errors.Add(
                    $"Строка {candidate.LineNumber}: " +
                    exception.Message);
            }
        }

        return rows;
    }

    private sealed record ImportKey(
    string CounterpartId1C,
    string Document,
    string NomenclaturaId1C);

    private sealed record RawCandidate(
        SaleImportRaw Raw,
        int LineNumber,
        ImportKey Key);
    private static SaleImportRow ParseRow(SaleImportRaw raw)
    {
        var counterpartId1C = Required(
            raw.CounterpartId1C,
            "counterpartId1C",
            100);

        var counterpartName1C = Required(
            raw.CounterpartName1C,
            "counterpartName1C",
            255);

        var nomenclaturaId1C = Required(
            raw.NomenclaturaId1C,
            "nomenclaturaId1C",
            100);

        var nomenclaturaName1C = Required(
            raw.NomenclaturaName1C,
            "nomenclaturaName1C",
            255);

        var document = Required(
            raw.Document,
            "document",
            500);

        if (!DateOnly.TryParseExact(
                raw.Date?.Trim(),
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
        {
            throw new FormatException(
                "Дата должна иметь формат ДД.ММ.ГГГГ.");
        }

        // Количество может быть целым или дробным.
        // Поддерживаются запятая и точка в дробной части.
        var quantity = ParseDouble(raw.Quantity, "quantity");

        return new SaleImportRow
        {
            CounterpartId1C = counterpartId1C,
            CounterpartName1C = counterpartName1C,
            NomenclaturaId1C = nomenclaturaId1C,
            NomenclaturaName1C = nomenclaturaName1C,
            Document = document,
            Date = date,
            Quantity = quantity,
            Revenue = ParseDouble(raw.Revenue, "revenue"),
            Cost = ParseDouble(raw.Cost, "cost"),
            GrossProfit = ParseDouble(raw.GrossProfit, "grossProfit"),
            Margin = ParseDouble(raw.Margin, "margin")
        };
    }

    private static string Required(
        string? value,
        string fieldName,
        int maxLength)
    {
        var result = value?.Trim();

        if (string.IsNullOrWhiteSpace(result))
        {
            throw new FormatException(
                $"Поле {fieldName} обязательно.");
        }

        if (result.Length > maxLength)
        {
            throw new FormatException(
                $"Поле {fieldName} не должно превышать " +
                $"{maxLength} символов.");
        }

        return result;
    }

    private static double ParseDouble(
        string? value,
        string fieldName)
    {
        var normalized = value?
            .Trim()
            .Replace(" ", "")
            .Replace("\u00A0", "")
            .Replace("\u202F", "")
            .Replace(",", ".");

        if (!double.TryParse(
                normalized,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var result)
            || double.IsNaN(result)
            || double.IsInfinity(result))
        {
            throw new FormatException(
                $"Поле {fieldName} должно быть конечным числом.");
        }

        return result;
    }

    private sealed class PreparedImport
    {
        public SalesImportViewModel Validation { get; } = new();

        public List<SaleImportRow> Rows { get; set; } = new();

        public Dictionary<string, int> CounterpartIds { get; set; } =
            new(StringComparer.Ordinal);

        public Dictionary<string, int> NomenclaturaIds { get; set; } =
            new(StringComparer.Ordinal);
    }
}

public sealed class SaleImportRawMap : ClassMap<SaleImportRaw>
{
    public SaleImportRawMap()
    {
        Map(x => x.CounterpartId1C)
            .Name("counterpartId1C");

        Map(x => x.CounterpartName1C)
            .Name("counterpartName1C");

        Map(x => x.Date)
            .Name("date");

        Map(x => x.Document)
            .Name("document");

        Map(x => x.NomenclaturaId1C)
            .Name("nomenclaturaId1C");

        Map(x => x.NomenclaturaName1C)
            .Name("nomenclaturaName1C");

        Map(x => x.Quantity)
            .Name("quantity");

        Map(x => x.Revenue)
            .Name("revenue");

        Map(x => x.Cost)
            .Name("cost");

        Map(x => x.GrossProfit)
            .Name("grossProfit");

        Map(x => x.Margin)
            .Name("margin");
    }
}