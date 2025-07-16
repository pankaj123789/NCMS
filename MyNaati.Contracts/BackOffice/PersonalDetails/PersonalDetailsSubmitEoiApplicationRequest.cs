namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsSubmitEoiApplicationRequest
    {
        
        public int NaatiNumber { get; set; }

        
        public int LanguageId { get; set; }

        
        public int LevelId { get; set; }

        
        public int CategoryId { get; set; }

        
        public string PrefferedTestCentre { get; set; }

        
        public bool? ToEnglish { get; set; }
    }
}