//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using MyNaati.Contracts.Portal.Users;
//using MyNaati.Ui.LINQtoCSV;

//namespace MyNaati.Ui.Reports.OnlineOrderDetails
//{
//    public class RegistrationUserRequestCsvGenerator : IRegistrationUserRequestCsvGenerator
//    {
//        public byte[] GenerateRegistrationUserRequestCsv(IEnumerable<RegistrationRequestSearchResult> userRequests)
//        {
//            var userRequestCsvs = new List<RegistrationUserRequestCsv>();

//            foreach (var userRequest in userRequests)
//            {
//                RegistrationUserRequestCsv userRequestCsv = new RegistrationUserRequestCsv();
//                userRequestCsv.NaatiNumber = userRequest.NaatiNumber;
//                userRequestCsv.FullName = string.Concat(userRequest.FamilyName, ", ", userRequest.GivenName);
//                userRequestCsv.DateOfBirth = userRequest.DateOfBirth.ToString("dd MMM yyyy");
//                userRequestCsv.Email = userRequest.EmailAddress;
//                userRequestCsv.DateRequested = (userRequest.DateRequested == null ? "" : ((DateTime)userRequest.DateRequested).ToString("dd/MMM/yyyy hh:mm:ss tt"));

//                userRequestCsvs.Add(userRequestCsv);
//            }

//            var outPutFileDescrption = new CsvFileDescription
//            {
//                QuoteAllFields = false,
//                SeparatorChar = ',',
//                FirstLineHasColumnNames = true
//            };

//            var cc = new CsvContext();

//            using (var tStream = new StringWriter())
//            {
//                cc.Write(userRequestCsvs, tStream, outPutFileDescrption);
//                return Encoding.UTF8.GetBytes(tStream.GetStringBuilder().ToString());
//            }
//        }
//    }
//}