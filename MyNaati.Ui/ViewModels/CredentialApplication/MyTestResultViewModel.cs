using System;
using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class MyTestResultViewModelList
	{
        public List<MyTestResultViewModel> MyTestResultList { get; set; }
        public MyTestResultViewModelList()
        {
			MyTestResultList = new List<MyTestResultViewModel>();
        }        
        
    }

    public class MyTestResultViewModel
	{
        public int TestSittingId { get; set; }
        public DateTime TestDate { get; set; }
        public string CredentialTypeDisplayName { get; set; }
        public string VenueName { get; set; }
        public string State { get; set; }
        public string TestLocationName { get; set; }
        public string SkillDisplayName { get; set; }
        public string OverallResult { get; set; }
		public bool EligibleForAPaidTestReview { get; set; }
		public bool EligibleForASupplementaryTest{ get; set; }
	    public bool Supplementary { get; set; }
	}
}
