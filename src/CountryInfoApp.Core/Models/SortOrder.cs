namespace CountryInfoApp.Core.Models;

/// <summary>
/// Most list operations of the service come in two flavours: sorted by name or by code.
/// </summary>
public enum SortOrder
{
    ByName,
    ByCode,
}
