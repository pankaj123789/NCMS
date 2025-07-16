using System;

namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsGetPersonResponse
    {
        
        public PersonalEditPerson Person { get; set; }
    }

    
    public class PersonalEditPerson
    {
        
        public int NaatiNumber { get; set; }

        
        public bool Deceased { get; set; }

        
        public string GivenName { get; set; }

        
        public string Surname { get; set; }

        
        public string FullName { get; set; }

        
        public bool IsEportalActive { get; set; }

        
        public string Title { get; set; }

        
        public bool AllowVerifyOnline { get; set; }

        
        public bool ShowPhotoOnline { get; set; }

        
        public int EntityTypeId { get; set; }

        
        public DateTime BirthDate { get; set; }

        
        public string Email { get; set; }

        
        public byte[] Photo{ get; set; }

        
        public string PractitionerNumber { get; set; }

        
        public bool IsFormerPractitioner { get; set; }

        
        public bool IsExaminer { get; set; }

        
        public bool IsPractitioner { get; set; }

        
        public bool IsApplicant { get; set; }

        
        public bool IsFuturePractitioner { get; set; }

        
        public string Country { get; set; }
    }
}