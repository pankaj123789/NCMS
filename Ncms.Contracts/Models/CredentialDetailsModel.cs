using System;

namespace Ncms.Contracts.Models
{
    public class CredentialDetailsModel
    {
        public int Id { get; set; }
       
        public string Skill { get; set; }
       
        public string Language { get; set; }
       
        public string ToLanguage { get; set; }
       
        public string Direction { get; set; }
       
        public bool IsIndigenous { get; set; }
       
        public DateTime? TerminationDate { get; set; }
       
        public string WorkPracticeUnits { get; set; }
       
        public bool IncludeOD { get; set; }

       
        public int? WorkPracticePoints { get; set; }
       
        public int CredentialRequestId { get; set; }

        public int CredentialApplicationTypeCategoryId { get; set; }
       
        public DateTime CredentialApplicationEnteredDate { get; set; }
       
        public DateTime? EndDate { get; set; }
       
        public DateTime StartDate { get; set; }
       
        public string CredentialType { get; set; }
       
        public int SkillId { get; set; }
       
        public int CredentialTypeId { get; set; }
       
        public int CategoryId { get; set; }
       
        public string CredentialTypeSkillName
        {
            get => CredentialType + " - " + Skill;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
            }
        }
       
        public string CredentialStatus { get; set; }
       
        public string RecertificationStatus { get; set; }
    }
}
