

//using NHibernate.Validator.Engine;

namespace F1Solutions.Naati.Common.Dal.Portal
{
    //public interface IValidator
    //{
    //    bool IsValid(object value);
    //    ICollection<IValidationResult> ValidationResultsFor(object value);
    //}

    //public class Validator : IValidator
    //{
    //    private static readonly ValidatorEngine mValidatorEngine;

    //    static Validator()
    //    {
    //        mValidatorEngine = new ValidatorEngine();
    //    }

    //    public bool IsValid(object value)
    //    {
    //        if (value == null)
    //            throw new ArgumentNullException("value", "value may not be null");

    //        return mValidatorEngine.IsValid(value);
    //    }

    //    public ICollection<IValidationResult> ValidationResultsFor(object value)
    //    {
    //        if (value == null)
    //            throw new ArgumentNullException("value", "value may not be null");

    //        var invalidValues = mValidatorEngine.Validate(value);

    //        return ParseValidationResultsFrom(invalidValues);
    //    }

    //    private static ICollection<IValidationResult> ParseValidationResultsFrom(IEnumerable<InvalidValue> invalidValues)
    //    {
    //        return invalidValues
    //            .Select(i => new ValidationResult(i))
    //            .Cast<IValidationResult>()
    //            .ToList();
    //    }
    //}
}
