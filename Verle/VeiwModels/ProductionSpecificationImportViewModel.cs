using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Verle.ViewModels;

public class ProductionSpecificationImportViewModel
{
    public bool Checked { get; set; }

    public int RowCount { get; set; }

    public int? ImportedCount { get; set; }

    public List<string> Errors { get; set; } = new();

    public List<MissingSpecificationNomenclatura> MissingNomenclaturas
    { get; set; } = new();

    [ValidateNever]
    public NomenclaturaFormViewModel Options { get; set; } = new();

    public bool CanImport =>
        Checked &&
        ImportedCount == null &&
        RowCount > 0 &&
        Errors.Count == 0 &&
        MissingNomenclaturas.Count == 0;
}

public class MissingSpecificationNomenclatura
{
    public string Id1C { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
}