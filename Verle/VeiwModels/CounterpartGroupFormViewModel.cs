using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Verle.ViewModels;

public class CounterpartGroupFormViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int? SelectedSalesChannelId { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> SalesChannelOptions { get; set; }
        = Array.Empty<SelectListItem>();
}