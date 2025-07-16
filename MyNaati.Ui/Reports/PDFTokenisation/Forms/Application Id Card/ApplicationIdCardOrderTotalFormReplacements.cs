//using System.Collections.Generic;
//using MyNaati.Ui.ViewModels.Shared;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms.Application_Id_Card
//{
//    public class ApplicationIdCardOrderTotalFormReplacements : BasicGridRenderer
//    {
//        private BasePurchaseWizardModel mBasePurchaseWizardModel;
//        private List<string[]> mItemList = null;
//        private string[] mFieldTitles = null;

//        public ApplicationIdCardOrderTotalFormReplacements(BasePurchaseWizardModel basePurchaseWizardModel)
//        {
//            mBasePurchaseWizardModel = basePurchaseWizardModel;
//        }

//        protected override List<string[]> ItemList
//        {
//            get
//            {
//                if (mItemList == null)
//                {
//                    mItemList = new List<string[]>();

//                    foreach (var orderItem in mBasePurchaseWizardModel.OrderTotalModel.Items)
//                    {
//                        mItemList.Add(new string[]
//                                          {
//                                              orderItem.Product, orderItem.Quantity.ToString(),
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
//                                            "ProductDescription", "OrderQty", "PricePerUnit", "ProductTotal"
//	                                   }.ToArray();
//                }
//                return mFieldTitles;
//            }
//        }
//    }
//}