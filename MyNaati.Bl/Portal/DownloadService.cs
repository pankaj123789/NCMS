//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Castle.Services.Transaction;
//using F1Solutions.NAATI.ePortal.Domain;
//using F1Solutions.NAATI.ePortal.ServiceContracts;
//using F1Solutions.NAATI.ePortal.Data;

//namespace F1Solutions.NAATI.ePortal.Services
//{
//    public class DownloadService : IDownloadService
//    {
//        private IDownloadRepository mDownloadRepository;
//        private IOrderRepository mOrderRepository;

//        public DownloadService(IDownloadRepository downloadRepository, IOrderRepository orderRepository)
//        {
//            mDownloadRepository = downloadRepository;
//            mOrderRepository = orderRepository;
//        }

//        [Transaction]
//        public void FormDownloaded(FormDownloadedRequest formDownloadedRequest)
//        {
//            var existingItems = mDownloadRepository.GetDownloadsForReferenceNumber(formDownloadedRequest.ReferenceNumber);

//            foreach (var download in existingItems)
//            {
//                mDownloadRepository.DeleteWithFlush(download);
//            }

//            mOrderRepository.UpdateWithFlush(formDownloadedRequest.FinalisedOrder);

//            foreach (var product in formDownloadedRequest.Products.Distinct())
//            {
//                var download = new Download()
//                                   {
//                                       DownloadDate = formDownloadedRequest.DownloadDate, 
//                                       NAATINumber = formDownloadedRequest.NAATINumber, 
//                                       Product = product, 
//                                       ReferenceNumber = formDownloadedRequest.ReferenceNumber
//                                   };
                
//                mDownloadRepository.SaveOrUpdate(download);                
//            }            
//        }
//    }
//}
