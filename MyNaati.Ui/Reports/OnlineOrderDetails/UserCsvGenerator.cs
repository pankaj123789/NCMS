//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using F1Solutions.Naati.Common.Contracts.Dal.Portal;
//using MyNaati.Ui.LINQtoCSV;

//namespace MyNaati.Ui.Reports.OnlineOrderDetails
//{
//    public class UserCsvGenerator : IUserCsvGenerator
//    {
//        public byte[] GenerateUserCsv(IEnumerable<ePortalSamUser> users)
//        {
//            var userCsvs = users.Select(user => new UserCsv()
//            {
//                NaatiNumber = user.NaatiNumber,
//                FullName = user.FullName.Trim(),
//                Email = user.Email.Trim(),
//                CreationDate = (user.WebAccountCreateDate == null) 
//                    ? "" 
//                    : user.WebAccountCreateDate.Value.ToString("dd MMM yyyy hh:mm:ss tt")
//            }).ToList();
            
//            var outPutFileDescription = new CsvFileDescription
//            {
//                QuoteAllFields = false,
//                SeparatorChar = ',',
//                FirstLineHasColumnNames = true
//            };

//            var cc = new CsvContext();

//            using (var tStream = new StringWriter())
//            {
//                cc.Write(userCsvs, tStream, outPutFileDescription);
//                return Encoding.UTF8.GetBytes(tStream.GetStringBuilder().ToString());
//            }

//        }
//    }
//}