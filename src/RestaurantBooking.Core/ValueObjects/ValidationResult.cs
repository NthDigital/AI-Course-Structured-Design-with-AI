namespace RestaurantBooking.Core.ValueObjects;

public class ValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyList<string> ValidationErrors { get; }

    public ValidationResult(bool isValid, IEnumerable<string>? validationErrors = null)
    {
        IsValid = isValid;
        ValidationErrors = validationErrors?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
    }

    public static ValidationResult Success() => new(true);
    
    public static ValidationResult Failure(params string[] errors) => new(false, errors);
    
    public static ValidationResult Failure(IEnumerable<string> errors) => new(false, errors);
}
