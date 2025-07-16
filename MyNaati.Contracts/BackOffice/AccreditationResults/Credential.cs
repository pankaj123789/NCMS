using System;
using System.Text;

namespace MyNaati.Contracts.BackOffice.AccreditationResults
{
    
    public class Credential
    {
        
        public int AccreditationResultId { get; set; }

        
        public string Skill { get; set; }

        
        public string Level { get; set; }

        
        public string Language { get; set; }

        
        public string ToLanguage { get; set; }

        
        public string Direction { get; set; }

        
        public DateTime? ExpiryDate { get; set; }

        
        public DateTime ResultDate { get; set; }

        
        public bool IsIndigenous { get; set; }

        
        public DateTime? StartDate { get; set; }

        
        // Helper methods:
        private const string EnglishLanguage = "English";

        private const string Both = "B";
        private const string ToEnglish = "E";
        private const string ToOther = "O";

        public static string GetDirection(Credential credential)
        {
            StringBuilder direction = new StringBuilder();

            if (credential.Direction.Equals(Both))
            {
                if (string.IsNullOrEmpty(credential.Language))
                    direction.Append(EnglishLanguage);
                else
                    direction.Append(credential.Language);

                direction.Append(" to/from ");

                if (string.IsNullOrEmpty(credential.ToLanguage))
                    direction.Append(EnglishLanguage);
                else
                    direction.Append(credential.ToLanguage);
            }
            else if (credential.Direction.Equals(ToEnglish))
            {
                direction.Append(credential.Language).Append(" to ").Append(EnglishLanguage);

            }
            else if (credential.Direction.Equals(ToOther))
            {
                if (string.IsNullOrEmpty(credential.ToLanguage))
                {
                    direction.Append(EnglishLanguage).Append(" to ").Append(credential.Language);
                }
                else
                {
                    direction.Append(credential.Language).Append(" to ").Append(credential.ToLanguage);
                }
            }
            else
            {
                throw new Exception("Unknown language direction: " + credential.Direction);
            }

            return direction.ToString();
        }
    }
}
