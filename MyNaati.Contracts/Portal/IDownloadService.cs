//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.ServiceModel;
//using System.Text;
//using F1Solutions.NAATI.ePortal.Domain;

//namespace F1Solutions.NAATI.ePortal.ServiceContracts
//{
//    
//    public interface IDownloadService
//    {
//        
//        void FormDownloaded(FormDownloadedRequest download);        
//    }

//    
//    public class FormDownloadedRequest
//    {
//        
//        public int NAATINumber { get; set; }
//        
//        public DateTime DownloadDate { get; set; }
//        
//        public string[] Products { get; set; }
//        
//        public string ReferenceNumber { get; set; }
//        
//        public Order FinalisedOrder { get; set; }
//    }
//}
