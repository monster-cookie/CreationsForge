namespace SFRecordCompareEngine.Core.DTOs.Records;

public class FactionDTO
{
    public required string ModKey { get; set; }
    public required string FormID { get; set; }
    public string? Name { get; set; }
    public string? KeywordFormKey { get; set; }
    public string? Flags { get; set; }
    public int? CrimeValuesArrest { get; set; }
    public int? CrimeValuesMurder { get; set; }
    public int? CrimeValuesAssault { get; set; }
    public int? CrimeValuesTrespass { get; set; }
    public int? CrimeValuesPickpocket { get; set; }
    public double? CrimeValuesStealMultiplier { get; set; }
    public int? CrimeValuesEscape { get; set; }
    public int? CrimeValuesPiracy { get; set; }
    public double? CrimeValuesSmuggleMultiplier { get; set; }
    public int? VendorValuesStartHour { get; set; }
    public int? VendorValuesEndHour { get; set; }
    public int? VendorValuesBuysStolenItems { get; set; }
    public int? VendorValuesBuysNonStolenItems { get; set; }
    public required string ImportedAtUtc { get; set; }
}
