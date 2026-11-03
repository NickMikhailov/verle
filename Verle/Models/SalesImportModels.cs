namespace Verle.Models;

public class SaleImportRaw
{
    public string? CounterpartId1C { get; set; }

    public string? CounterpartName1C { get; set; }

    public string? Date { get; set; }

    public string? Document { get; set; }

    public string? NomenclaturaId1C { get; set; }

    public string? NomenclaturaName1C { get; set; }

    public string? Quantity { get; set; }

    public string? Revenue { get; set; }

    public string? Cost { get; set; }

    public string? GrossProfit { get; set; }

    public string? Margin { get; set; }
}

public class SaleImportRow
{
    public string CounterpartId1C { get; set; } = string.Empty;

    public string CounterpartName1C { get; set; } = string.Empty;

    public DateOnly Date { get; set; }

    public string Document { get; set; } = string.Empty;

    public string NomenclaturaId1C { get; set; } = string.Empty;

    public string NomenclaturaName1C { get; set; } = string.Empty;

    public double Quantity { get; set; }

    public double Revenue { get; set; }

    public double Cost { get; set; }

    public double GrossProfit { get; set; }

    public double Margin { get; set; }
}

public class UnknownReferenceViewModel
{
    public string Id1C { get; set; } = string.Empty;

    public string Name1C { get; set; } = string.Empty;
}

public class SalesImportViewModel
{
    public IFormFile? File { get; set; }

    public int RowCount { get; set; }

    public List<string> Errors { get; set; } = [];

    public List<UnknownReferenceViewModel> UnknownCounterparts { get; set; } = [];

    public List<UnknownReferenceViewModel> UnknownNomenclaturas { get; set; } = [];

    public bool CanImport =>
        Errors.Count == 0 &&
        UnknownCounterparts.Count == 0 &&
        UnknownNomenclaturas.Count == 0 &&
        RowCount > 0;
}