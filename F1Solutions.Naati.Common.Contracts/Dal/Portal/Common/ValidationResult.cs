using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal.Common
{
    public interface IValidationResult
    {
        // Properties
        object AttemptedValue { get; }
        Type ClassContext { get; }
        string Message { get; }
        string PropertyName { get; }
    }

    
    public class ValidationResult : IValidationResult
    {
        public ValidationResult(string message)
        {
            Message = message;
        }
      

        
        public object AttemptedValue { get; private set; }

        
        public Type ClassContext { get; private set; }

        
        public string Message { get; private set; }

        
        public string PropertyName { get; private set; }
    }
}
