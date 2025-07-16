using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.PersonalDetails
{
    public interface IAddressGoogleModel
    {
        int Id { get; set; }        
        
        string StreetDetails { get; set; }
     
        string SuburbName { get; set; }
       
        string Postcode { get; set; }

        string State { get; set; }

        string CountryName { get; set; }
        
        bool IsFromAustralia{ get; set; }

        bool IsPreferred { get; set; }

        bool Success { get; set; }

        List<string> Errors { get; set; }
    }
}
