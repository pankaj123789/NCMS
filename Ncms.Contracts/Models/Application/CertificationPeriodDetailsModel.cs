using System;

namespace Ncms.Contracts.Models.Application
{
    public class CertificationPeriodDetailsModel
    {
        public int Id { get; set; }
      
    
        public DateTime EndDate { get; set; }
 
        public DateTime OriginalEndDate { get; set; }
  
        public int? SubmittedRecertificationApplicationId { get; set; }
    
        public DateTime? RecertificationEnteredDate { get; set; }
    }
}
