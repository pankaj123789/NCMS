namespace MyNaati.Contracts.BackOffice.Language
{
    
    public class EoiLanguagesGetResponse
    {
        
        public EoiLanguageTransferObject[] EoiLanguages { get; set; }
    }

    
    public class EoiLanguageTransferObject
    {
        
        public int CategoryId { get; set; }

        
        public int LevelId { get; set; }

        
        public int LanguageId { get; set; }

        
        public bool? ToEnglish { get; set; }

        
        public bool Active { get; set; }

        
        public string LanguageDisplay { get; set; }
    }
}
