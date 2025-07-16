namespace MyNaati.Contracts.BackOffice.PersonalDetails
{
    
    public class PersonalDetailsCreateNonCandidateResponse
    {
        
        public int NaatiNumber { get; set; }

        
        public int[] NaatiNumbers { get; set; }

        
        public bool IsDuplicate { get; set; }

        
        private bool IsDuplicateCredentialed { get; set; }

        public bool Credentialed
        {
            get
            {
                return IsDuplicate ? IsDuplicateCredentialed : false;
            }
            set { IsDuplicateCredentialed = value; }
        }
    }
}
