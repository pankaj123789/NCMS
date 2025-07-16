//using System.Linq;
//using System.Reflection;
//using AutoMapper;
//using F1Solutions.Naati.Common.Bl.AutoMappingProfiles;
//using F1Solutions.Naati.Common.Contracts.Bl;

//namespace MyNaati.Ui.AutoMapperRegistry
//{
//    public class AutoMapperRegister
//    {
//        public static void Configure()
//        {
//            Mapper.Reset();

//            Assembly assembly = typeof(ProductSpecificationProfile).Assembly;

//            var profiles = assembly.GetExportedTypes()
//                .Where(t => t.IsSubclassOf(typeof(Profile)))
//                .Select(ServiceLocator.GetService);

//            Mapper.AllowNullDestinationValues = true;

//            foreach (Profile profile in profiles)
//            {
//                Mapper.AddProfile(profile);
//            }

//            Mapper.AssertConfigurationIsValid();
//        }
//    }
//}