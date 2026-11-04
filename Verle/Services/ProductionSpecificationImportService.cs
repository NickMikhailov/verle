using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Verle.Data;
using Verle.Models;
using Verle.ViewModels;

namespace Verle.Services;

public class ProductionSpecificationImportService
{
    private const long MaxFileSize = 5 * 1024 * 1024;
    private const int MaxRows = 10_000;

    private const decimal MaxQuantity = 999999999999.999999m;

    private static readonly string[] RequiredHeaders =
    {
        "nomenclaturaId1COutput",
        "nameOutput",
        "nomenclaturaId1CInput",
        "nameInput",
        "quantityInput",
        "quantityOutput"
    };

    private readonly VerleContext _context;

    public ProductionSpecificationImportService(
        VerleContext context)
    {
        _context = context;
    }

    // Проверка без сохранения.
    public async Task<ProductionSpecificationImportViewModel>
        ValidateAsync(
            IFormFile? file,
            CancellationToken cancellationToken = default)
    {
        var prepared = await PrepareAsync(
            file,
            cancellationToken);

        return prepared.Validation;
    }

    // Повторная проверка и сохранение всех строк.
    public async Task<ProductionSpecificationImportViewModel>
        ImportAsync(
            IFormFile? file,
            CancellationToken cancellationToken = default)
    {
        var prepared = await PrepareAsync(
            file,
            cancellationToken);

        if (!prepared.Validation.CanImport)
        {
            return prepared.Validation;
        }

        var entities = prepared.Rows
            .Select(row => new ProductionSpecification
            {
                NomenclaturaIdOutput =
                    prepared.NomenclaturaIds[row.OutputCode],

                NomenclaturaIdInput =
                    prepared.NomenclaturaIds[row.InputCode],

                QuantityInput = row.QuantityInput,
                QuantityOutput = row.QuantityOutput
            })
            .ToList();

        _context.ProductionSpecifications.AddRange(entities);

        // Один SaveChanges: все строки сохраняются
        // в одной транзакции EF Core.
        await _context.SaveChangesAsync(cancellationToken);

        prepared.Validation.ImportedCount = entities.Count;

        return prepared.Validation;
    }

    private async Task<PreparedImport> PrepareAsync(
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        var prepared = new PreparedImport();

        var validation = prepared.Validation;
        validation.Checked = true;

        if (file == null || file.Length == 0)
        {
            AddError(validation, "Выберите непустой CSV-файл.");
            return prepared;
        }

        if (file.Length > MaxFileSize)
        {
            AddError(
                validation,
                "Размер CSV-файла не должен превышать 5 МБ.");

            return prepared;
        }

        if (!string.Equals(
                Path.GetExtension(file.FileName),
                ".csv",
                StringComparison.OrdinalIgnoreCase))
        {
            AddError(validation, "Нужен файл с расширением .csv.");
            return prepared;
        }

        // Общий список кодов и названий из обеих ролей.
        var namesByCode = new Dictionary<string, string>(
            StringComparer.Ordinal);

        var configuration = new CsvConfiguration(
            CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true,
            IgnoreBlankLines = true,
            TrimOptions = TrimOptions.Trim
        };

        try
        {
            await using var stream = file.OpenReadStream();

            using var reader = new StreamReader(
                stream,
                new UTF8Encoding(
                    encoderShouldEmitUTF8Identifier: false,
                    throwOnInvalidBytes: true),
                detectEncodingFromByteOrderMarks: true);

            using var csv = new CsvReader(reader, configuration);

            if (!await csv.ReadAsync())
            {
                AddError(validation, "CSV-файл пуст.");
                return prepared;
            }

            csv.ReadHeader();

            var headers = csv.HeaderRecord?
                .Select(x => x.Trim().TrimStart('\uFEFF'))
                .ToArray()
                ?? Array.Empty<string>();

            foreach (var required in RequiredHeaders)
            {
                var occurrences = headers.Count(
                    x => string.Equals(
                        x,
                        required,
                        StringComparison.Ordinal));

                if (occurrences == 0)
                {
                    AddError(
                        validation,
                        $"Отсутствует столбец «{required}».");
                }
                else if (occurrences > 1)
                {
                    AddError(
                        validation,
                        $"Столбец «{required}» указан несколько раз.");
                }
            }

            if (validation.Errors.Count > 0)
            {
                return prepared;
            }

            var indexes = RequiredHeaders.ToDictionary(
                name => name,
                name => Array.IndexOf(headers, name),
                StringComparer.Ordinal);

            while (await csv.ReadAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();

                validation.RowCount++;

                if (validation.RowCount > MaxRows)
                {
                    AddError(
                        validation,
                        $"В одном файле допускается не более {MaxRows} строк.");

                    break;
                }

                var line = csv.Context.Parser.Row;
                var errorsBefore = validation.Errors.Count;

                string Field(string name)
                {
                    return (
                        csv.GetField(indexes[name]) ?? string.Empty
                    ).Trim();
                }

                var outputCode = Field("nomenclaturaId1COutput");
                var outputName = Field("nameOutput");

                var inputCode = Field("nomenclaturaId1CInput");
                var inputName = Field("nameInput");

                RegisterNomenclatura(
                    outputCode,
                    outputName,
                    "output",
                    line,
                    namesByCode,
                    validation);

                RegisterNomenclatura(
                    inputCode,
                    inputName,
                    "input",
                    line,
                    namesByCode,
                    validation);

                var inputValid = TryReadQuantity(
                    Field("quantityInput"),
                    out var quantityInput);

                if (!inputValid)
                {
                    AddError(
                        validation,
                        $"Строка {line}: quantityInput должен быть " +
                        "положительным числом в пределах numeric(18,6), " +
                        "без необходимости округления.");
                }

                var outputValid = TryReadQuantity(
                    Field("quantityOutput"),
                    out var quantityOutput);

                if (!outputValid)
                {
                    AddError(
                        validation,
                        $"Строка {line}: quantityOutput должен быть " +
                        "положительным числом в пределах numeric(18,6), " +
                        "без необходимости округления.");
                }

                if (validation.Errors.Count != errorsBefore)
                {
                    continue;
                }

                prepared.Rows.Add(new ImportRow
                {
                    OutputCode = outputCode,
                    InputCode = inputCode,
                    QuantityInput = quantityInput,
                    QuantityOutput = quantityOutput
                });
            }
        }
        catch (CsvHelperException)
        {
            AddError(
                validation,
                "Не удалось прочитать CSV. Проверьте разделитель «;», " +
                "количество столбцов и парность кавычек.");
        }
        catch (DecoderFallbackException)
        {
            AddError(
                validation,
                "Не удалось прочитать кодировку файла. " +
                "Сохраните CSV в UTF-8.");
        }

        if (validation.RowCount == 0 &&
            validation.Errors.Count == 0)
        {
            AddError(
                validation,
                "В файле есть заголовки, но нет строк данных.");
        }

        // Загружаем только номенклатуры, упомянутые в CSV.
        // Разбиваем коды на небольшие группы.
        foreach (var codes in namesByCode.Keys.Chunk(500))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var existing = await _context.Nomenclaturas
                .AsNoTracking()
                .Where(x => codes.Contains(x.Id1C))
                .Select(x => new
                {
                    x.Id,
                    x.Id1C
                })
                .ToListAsync(cancellationToken);

            foreach (var item in existing)
            {
                prepared.NomenclaturaIds[item.Id1C] = item.Id;
            }
        }

        validation.MissingNomenclaturas = namesByCode
            .Where(x => !prepared.NomenclaturaIds.ContainsKey(x.Key))
            .OrderBy(x => x.Key, StringComparer.Ordinal)
            .Select(x => new MissingSpecificationNomenclatura
            {
                Id1C = x.Key,
                Name = x.Value
            })
            .ToList();

        return prepared;
    }

    private static void RegisterNomenclatura(
        string code,
        string name,
        string role,
        int line,
        Dictionary<string, string> namesByCode,
        ProductionSpecificationImportViewModel validation)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            AddError(
                validation,
                $"Строка {line}: не заполнен код 1С для {role}.");

            return;
        }

        if (code.Length > 100)
        {
            AddError(
                validation,
                $"Строка {line}: код 1С для {role} " +
                "не должен превышать 100 символов.");

            return;
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            AddError(
                validation,
                $"Строка {line}: не заполнено название для {role}.");

            return;
        }

        if (name.Length > 200)
        {
            AddError(
                validation,
                $"Строка {line}: название для {role} " +
                "не должно превышать 200 символов.");

            return;
        }

        if (namesByCode.TryGetValue(code, out var previousName))
        {
            if (!string.Equals(
                    previousName,
                    name,
                    StringComparison.Ordinal))
            {
                AddError(
                    validation,
                    $"Строка {line}: для кода «{code}» " +
                    "в файле указаны разные названия.");
            }

            return;
        }

        namesByCode.Add(code, name);
    }

    private static bool TryReadQuantity(
        string text,
        out decimal value)
    {
        value = 0;

        // Разделители тысяч и экспоненциальная запись
        // намеренно не поддерживаются.
        var normalized = text.Trim().Replace(',', '.');

        var styles =
            NumberStyles.AllowLeadingSign |
            NumberStyles.AllowDecimalPoint;

        if (!decimal.TryParse(
                normalized,
                styles,
                CultureInfo.InvariantCulture,
                out value))
        {
            return false;
        }

        return value > 0 &&
               value <= MaxQuantity &&
               decimal.Round(value, 6) == value;
    }

    private static void AddError(
        ProductionSpecificationImportViewModel validation,
        string message)
    {
        validation.Errors.Add(message);
    }

    private sealed class PreparedImport
    {
        public ProductionSpecificationImportViewModel Validation
        { get; } = new();

        public List<ImportRow> Rows { get; } = new();

        public Dictionary<string, int> NomenclaturaIds { get; }
            = new(StringComparer.Ordinal);
    }

    private sealed class ImportRow
    {
        public string OutputCode { get; set; } = string.Empty;

        public string InputCode { get; set; } = string.Empty;

        public decimal QuantityInput { get; set; }

        public decimal QuantityOutput { get; set; }
    }
}