//using System;
//using F1Solutions.NAATI.ePortal.Data.Repositories;
//using F1Solutions.NAATI.ePortal.Domain;
//using F1Solutions.NAATI.ePortal.ServiceContracts;
//using IApplicationService = F1Solutions.NAATI.ePortal.ServiceContracts.IApplicationService;

//namespace F1Solutions.NAATI.ePortal.Services
//{
//    public class ApplicationService : IApplicationService
//    {
//        private readonly IApplicationRepository mApplicationRepository;

//        public ApplicationService(IApplicationRepository applicationRepository)
//        {
//            this.mApplicationRepository = applicationRepository;
//        }

//        public int StoreApplication(ApplicationStorageRequest applicationStorageRequest)
//        {
//            var application = new Application
//            {
//                NAATINumber = applicationStorageRequest.NAATINumber,
//                Data = applicationStorageRequest.Data,
//                Date = DateTime.Now,
//                IsApplicationByTesting = applicationStorageRequest.IsApplicationByTesting
//            };

//            this.mApplicationRepository.SaveOrUpdate(application);
//            return application.Id;
//        }

//        public MatchingApplicationFindResponse FindMatchingApplication(MatchingApplicationFindRequest request)
//        {
//            Application application = this.mApplicationRepository.Get(request.ApplicationId);

//            if (application == null)
//            {
//                return new MatchingApplicationFindResponse
//                {
//                    Found = false
//                };
//            }

//            return new MatchingApplicationFindResponse
//            {
//                Data = application.Data,
//                Date = application.Date,
//                Found = true,
//                IsApplicationByTesting = application.IsApplicationByTesting,
//                NAATINumber = application.NAATINumber
//            };
//        }
//    }
//}
