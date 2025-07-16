//using F1Solutions.Naati.Common.Dal.Portal.Repositories;
//using MyNaati.Contracts.BackOffice.Products;

//namespace MyNaati.Bl.BackOffice
//{
   
//    public class ProductSpecificationService : IProductSpecificationService
//    {

//        private IProductSpecificationRepository mProductSpecificationRepository;

//        public ProductSpecificationService(IProductSpecificationRepository productSpecificationRepository)
//        {
//            mProductSpecificationRepository = productSpecificationRepository;
//        }

//        public ProductSpecificationGetProductSpecificationResponse GetProductSpecification(ProductSpecificationGetProductSpecificationRequest request)
//        {
//            var response = new ProductSpecificationGetProductSpecificationResponse();

//            var domainResponse = mProductSpecificationRepository.Get(request.ProductSpecificationId);

//            response.ProductSpecification = new ProductSpecification
//            {
//                CostPerUnit = domainResponse.CostPerUnit,
//                Description = domainResponse.Description,
//                Name = domainResponse.Name
//            };
                
                
//            /*    Mapper.Map<NaatiDomain.ProductSpecification, ProductSpecification>
//                    (domainResponse);*/

//            return response;
//        }
//    }
//}


