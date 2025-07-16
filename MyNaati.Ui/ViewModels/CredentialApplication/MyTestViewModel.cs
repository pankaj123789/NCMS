using System;
using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class MyTestViewModelList
    {
        public List<MyTestViewModel> MyTestList { get; set; }
        public string ErrorMessage { get; set; }

        public MyTestViewModelList()
        {
            MyTestList = new List<MyTestViewModel>();
        }        
        
    }

    public class MyTestViewModel
    {
        public int CredentialRequestId { get; set; }
        public int CredentialApplicationId { get; set; }
        public int? TestSittingId { get; set; }
        public DateTime? TestDate { get; set; }
        public string ApplicationTypeDisplayName { get; set; }
        public string CredentialTypeDisplayName { get; set; }
        public string VenueName { get; set; }
        public string SkillDisplayName { get; set; }
        public string Status { get; set; }
        public string OverallResult { get; set; }
        public bool CanOpenDetails { get; set; }
        public bool CanSelectTestSession { get; set; }
		public bool EligibleForAPaidTestReview { get; set; }
        public bool CanRequestRefund { get; set; }
    }


    public class AvailableTestSessionViewModelList
    {

        public int CredentialRequestId { get; set; }
        public int CredentialApplicationId { get; set; }
        public List<AvailableTestSessionViewModel> AvailableTestSessionList { get; set; }
        public AvailableTestSessionViewModel AllocatedTestSession { get; set; }
        public int Index { get; set; }
        public AvailableTestSessionViewModelList()
        {
            AvailableTestSessionList = new List<AvailableTestSessionViewModel>();
            Index = 1;
        }

    }

    public class MyTestsModel
    {
        public int CredentialRequestId { get; set; }
        public int CredentialApplicationId { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class AvailableTestSessionViewModel
    {
        public string TestSessionId { get; set; }
        public int Id { get; set; }
        public string TestDate { get; set; }
        public string TestStart { get; set; }
        public string TestLocation { get; set; }
        public string ExpectedCompletion { get; set; }
        public string VenueName { get; set; }
        public string VenueAddress { get; set; }
        public string Availability { get; set; }
        public string AvailabilityClass { get; set; }
        
        public bool HasAllocatedTestSession { get; set; }
        public bool IsPreferedLocation { get; set; }
        public bool TestFeePaid { get; set; }

        public Decimal? Longitude { get; set; }
        public Decimal? Latitude { get; set; }

        public int CredentialRequestId { get; set; }
        public int CredentialApplicationId { get; set; }
    }
}
