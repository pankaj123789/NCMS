//using System;
//using System.Data;
//using Ncms.Contracts.Models.Person;

//namespace Ncms.Bl.Mappers
//{
//    public class DataRowToPersonModelMapper : BaseMapper<DataRow, PersonModel>
//    {
//        public override PersonModel Map(DataRow source)
//        {
//            return new PersonModel
//            {
//                PersonId = Convert.ToInt32(source["PersonId"]),
//                EntityId = Convert.ToInt32(source["EntityId"]),
//                NaatiNumber = source["NAATINumber"] as int?,
//                NaatiNumberDisplay = source["NAATINumberDisplay"] as string,
//                Name = source["Name"] as string,
//                PostcodeId = source["PostcodeId"] as int?,
//                StateId = source["StateId"] as int?,
//                Email = source["Email"] as string,
//                EmailId = source["EmailId"] as int?,
//                Number = source["Number"] as string,
//                GstApplies = Convert.ToBoolean(source["GSTApplies"]),
//                BirthDate = source["BirthDate"] as DateTime?,
//                DoNotSendCorrespondence = Convert.ToBoolean(source["DoNotSendCorrespondence"]),
//                PersonAddress = source["PersonAddress"] as string,
//                EntityTypeId = source["EntityTypeId"] as int?,
//                StateName = source["StateName"] as string,
//                Deceased = source["Deceased"] as bool?,
//            };
//        }

//        public override DataRow MapInverse(PersonModel source)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
