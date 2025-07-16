//using System;
//using System.Collections.Generic;
//using System.Data;

//namespace Ncms.Bl.Mappers
//{
//    public class DataRowToDictionary : BaseMapper<DataRow, Dictionary<string, object>>
//    {
//        public override Dictionary<string, object> Map(DataRow source)
//        {
//            var dictionary = new Dictionary<string, object>();

//            foreach (DataColumn c in source.Table.Columns)
//            {
//                var retVal = source[c];
//                dictionary[c.ColumnName] = retVal;
//            }

//            return dictionary;
//        }

//        public override DataRow MapInverse(Dictionary<string, object> source)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
