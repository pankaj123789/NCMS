//using System.Collections.Generic;
//using MyNaati.Ui.ViewModels.Shared;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms.Application_Stamp
//{
//    public class ApplicationStampOrderTotalFormReplacements : BasicGridRenderer
//    {
//        private OrderTotalViewModel mOrderTotalViewModel;
//        private List<string[]> mItemList = null;
//        private string[] mFieldTitles = null;

//        public ApplicationStampOrderTotalFormReplacements(OrderTotalViewModel orderTotalViewModel)
//        {
//            mOrderTotalViewModel = orderTotalViewModel;
//        }

//        protected override List<string[]> ItemList
//        {
//            get
//            {
//                if (mItemList == null)
//                {
//                    mItemList = new List<string[]>();

//                    foreach (var orderItem in mOrderTotalViewModel.Items)
//                    {
//                        mItemList.Add(new string[]
//                                          {
//                                              orderItem.Product, 
//                                              orderItem.Skill,
//                                              orderItem.Level,
//                                              orderItem.Direction,
//                                              orderItem.Expiry.HasValue ? orderItem.Expiry.Value.ToString("dd MMM yyyy") : string.Empty,
//                                              orderItem.Quantity.ToString(),
//                                              string.Format("{0:C}", orderItem.UnitPrice),
//                                              string.Format("{0:C}", orderItem.TotalPrice)
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
//                                            "StampProductDescription", "StampCredentialSkill", "StampCredentialLevel", 
//                                            "StampCredentialDirection", "StampCredentialExpiry",
//                                            "StampOrderQty", "StampPricePerUnit",
//                                            "StampProductTotal"
//	                                   }.ToArray();
//                }
//                return mFieldTitles;
//            }
//        }
//    }
//}