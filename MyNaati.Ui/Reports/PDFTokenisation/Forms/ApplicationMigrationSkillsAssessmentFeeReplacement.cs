//using MyNaati.Contracts.BackOffice.Products;
//using MyNaati.Contracts.Portal;
//using MyNaati.Ui.Common;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationMigrationSkillsAssessmentFeeReplacement : ITokenReplacement
//    {
//        protected ILookupProvider mLookupProvider;
//        private IProductSpecificationService mProductSpecificationService;

//        public ApplicationMigrationSkillsAssessmentFeeReplacement(IProductSpecificationService productSpecificationService, ILookupProvider lookupProvider)
//        {
//            mProductSpecificationService = productSpecificationService;
//            mLookupProvider = lookupProvider;
//        }

//        public string GetReplacement(string title)
//        {
//            switch (title)
//            {
//                case "AssessmentFee":
//                    decimal assessmentFee = GetAssessmentFee();
//                    return assessmentFee.ToString("C2");
//            }

//            return null;
//        }

//        private decimal GetAssessmentFee()
//        {
//            var request = new ProductSpecificationGetProductSpecificationRequest()
//            {
//                ProductSpecificationId = mLookupProvider.SystemValues.FormMFeeId
//            };
//            ProductSpecificationGetProductSpecificationResponse response = mProductSpecificationService.GetProductSpecification(request);

//            return response.ProductSpecification.CostPerUnit;
//        }
//    }
//}