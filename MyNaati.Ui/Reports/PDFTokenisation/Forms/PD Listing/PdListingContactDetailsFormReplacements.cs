//using System.Collections.Generic;
//using System.Linq;
//using MyNaati.Ui.ViewModels.PDListing;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms.PD_Listing
//{
//    public class PdListingContactDetailsFormReplacements : BasicGridRenderer
//    {
//        private WizardModel mWizardModel;
//        private List<string[]> mItemList = null;
//        private string[] mFieldTitles = null;

//        public PdListingContactDetailsFormReplacements(WizardModel wizardModel)
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

//                    foreach (var phone in mWizardModel.Phones)
//                    {
//                        mItemList.Add(new string[]
//                                          {
//                                              phone.Contact, phone.Contact
//                                          });
//                    }

//                    foreach (var email in mWizardModel.Emails)
//                    {
//                        mItemList.Add(new string[]
//                                          {
//                                              email.Contact, email.Contact
//                                          });
//                    }
//                }
//                return mItemList.Take(5).ToList();
//            }
//        }

//        protected override string[] FieldTitles
//        {
//            get
//            {
//                if(mFieldTitles == null)
//                {
//                    mFieldTitles = new List<string>()
//	                                   {
//                                           "PDListingContactType", "PDListingContactDetails"
//	                                   }.ToArray();
//                }
//                return mFieldTitles;
//            }
//        }
//    }
//}