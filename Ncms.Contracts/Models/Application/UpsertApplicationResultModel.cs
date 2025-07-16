using System.Collections.Generic;

namespace Ncms.Contracts.Models.Application
{ 
    public class UpsertApplicationResultModel
    {
        public int CredentialApplicationId { get; set; }
        public IEnumerable<int> CredentialApplicationFieldDataIds { get; set; }
        public IEnumerable<int> CredentialRequestIds { get; set; }
        public IEnumerable<int> NoteIds { get; set; }
    }


    //public class SytemUpsertResult<TUpsertResultModel> 
    //{
   
    //}
}
