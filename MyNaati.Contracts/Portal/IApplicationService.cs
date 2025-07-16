using System;

namespace MyNaati.Contracts.Portal
{
    
    public interface IApplicationService
    {
        /// <summary>
        /// Store an application's serialisation in the database
        /// </summary>
        /// <param name="applicationStorageRequest">the request containing the data and Customer number to store</param>
        /// <returns>a unique identifier for the application (aka its reference number)</returns>
        
        int StoreApplication(ApplicationStorageRequest applicationStorageRequest);

        /// <summary>
        /// Finds a matching application from the database for use in SAM.
        /// </summary>
        /// <param name="request">An object describing the application</param>
        /// <returns>Data on the application</returns>
        
        MatchingApplicationFindResponse FindMatchingApplication(MatchingApplicationFindRequest request);
        
    }

    public class ApplicationStorageRequest
    {
        
        public int? NAATINumber { get; set; }

        
        public string Data { get; set; }

        
        public bool IsApplicationByTesting { get; set; }
    }

    
    public class MatchingApplicationFindRequest
    {
        
        public int ApplicationId { get; set; }
    }

    
    public class MatchingApplicationFindResponse
    {
        
        public bool Found { get; set; }

        
        public int? NAATINumber { get; set; }

        
        public DateTime Date { get; set; }

        
        public bool IsApplicationByTesting { get; set; }

        
        public string Data { get; set; }
    }
}
