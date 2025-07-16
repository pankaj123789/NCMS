//using System.Collections.Generic;
//using MyNaati.Ui.ViewModels.PDListing;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms.PD_Listing
//{
//    public class PdListingCredentialDetailsFormReplacements : BasicGridRenderer
//    {
//        private WizardModel mWizardModel;
//        private List<string[]> mItemList = null;
//        private string[] mFieldTitles = null;

//        public PdListingCredentialDetailsFormReplacements(WizardModel wizardModel)
//        {
//            mWizardModel = wizardModel;
//        }

//        protected override List<string[]> ItemList
//        {
//            get
//            {
//                if(mItemList == null)
//                {
//                    mItemList = new List<string[]>();

//                    foreach (var credential in mWizardModel.Credentials)
//                    {
//                        mItemList.Add(new string[]
//                                          {
//                                              credential.Skill,
//                                              credential.Level,
//                                              credential.Direction,
//                                              credential.Expiry.HasValue ? credential.Expiry.Value.ToString("dd MMM yyyy") : string.Empty
//                                          });
//                    }
//                }
//                return mItemList;
//            }
//        }

//        protected override string[] FieldTitles
//        {
//            get
//            {
//                if (mFieldTitles == null)
//                {
//                    mFieldTitles = new List<string>()
//	                                   {
//                                            "PDListingCredentialSkill", "PDListingCredentialLevel", "PDListingCredentialDirection", 
//                                            "PDListingCredentialExpiry"
//	                                   }.ToArray();
//                }
//                return mFieldTitles;
//            }
//        }
//    }
//}