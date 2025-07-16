using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal.Common
{
    
    public abstract class BaseUpdateResponse
    {
        protected BaseUpdateResponse()
        {
            ValidationMessages = new List<ValidationResult>();
            Success = true;
        }

        
        public IList<ValidationResult> ValidationMessages { get; set; }

        
        public bool Success { get; set; }
    }
}
