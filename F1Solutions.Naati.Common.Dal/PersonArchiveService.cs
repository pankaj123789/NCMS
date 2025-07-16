using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using F1Solutions.Naati.Common.ServiceContracts.Services;
using NAATI.WebService.NHibernate.DataAccess;
using NHibernate;
using NHibernate.Linq;
using SharpArch.Data.NHibernate;
using NaatiDomain = NAATI.Domain;

namespace F1Solutions.NAATI.SAM.WebService.ExposedServices
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PersonArchiveService : IPersonArchiveService
    {
        // this is the class definition for the object template for the csv reader to populate
        class ArchiveRecord
        {
            public string naatiNumber { get; set; }
            public string name { get; set; }
            public string createdDate { get; set; }
            public string language { get; set; }
            public string level { get; set; }
            public string category { get; set; }
            public string method { get; set; }
            public string enteredDate { get; set; }
            public string resultDate { get; set; }
        }

        // this is the mapper class which defines which columns the properties use
        sealed class CsvMap : CsvClassMap<ArchiveRecord>
        {
            public CsvMap()
            {
                Map(m => m.naatiNumber).Name("Naati Number");
                Map(m => m.name).Name("Name");
                Map(m => m.createdDate).Name("Created Date");
                Map(m => m.language).Name("Language");
                Map(m => m.level).Name("Level");
                Map(m => m.category).Name("Category");
                Map(m => m.method).Name("Method");
                Map(m => m.enteredDate).Name("Entered Date");
                Map(m => m.resultDate).Name("Result Date");
            }
        }

        public ArchiveResponse ArchivePeople(string file, DateTime archiveDate, bool updateRecordsOnException)
        {
            try
            {
                var trimmedFile = trimCsvString(file);
                var sr = new StringReader(trimmedFile);
                var csv = new CsvReader(sr);
                csv.Configuration.IsHeaderCaseSensitive = false;
                csv.Configuration.RegisterClassMap(new CsvMap());
                var records = csv.GetRecords<ArchiveRecord>().ToList();

                int number;

                var naatiNumbers =
                    records.Where(f => int.TryParse(f.naatiNumber, out number))
                        .Select(r => int.Parse(r.naatiNumber))
                        .ToArray();
                var firstNames = records.Where(f => int.TryParse(f.naatiNumber, out number))
                    .Select(r => r.name.Replace("\"", "").Split(new[] {", "}, 2, StringSplitOptions.None)[0])
                    .ToArray();
                var surNames = records.Where(f => int.TryParse(f.naatiNumber, out number))
                    .Select(r => r.name.Replace("\"", "").Split(new[] {", "}, 2, StringSplitOptions.None).Length > 1 ? 
                        r.name.Replace("\"", "").Split(new[] {", "}, 2, StringSplitOptions.None)[1] : "")
                    .ToArray();
                var createdDates =
                    records.Where(f => int.TryParse(f.naatiNumber, out number))
                        .Select(r => r.createdDate)
                        .ToArray();
                var result = ArchivePeople(naatiNumbers, firstNames, surNames, createdDates, archiveDate, updateRecordsOnException);

                return result;
            }
            catch (Exception ex)
            {
                return new ArchiveResponse()
                {
                    ErrorMessage = ex.Message,
                    Success = false,
                    UpdatedNaatiNumbers = 0,
                    ErrorNaatiNumbers = new PersonError[0]
                };
            }
        }

        private ArchiveResponse ArchivePeople(int[] naatiNumbers, string[] firstNames, string[] surNames, string[] createdDates, DateTime archiveDate, bool updateRecordsOnException)
        {
            List<int> updatedNaatiNumbers = new List<int>();
            List<PersonError> errorNaatiNumbers = new List<PersonError>();
            var errorMessage = string.Empty;

            // perform validation of naatiNumbers with their data
            for (int i = 0; i < naatiNumbers.Length; i++)
            {
                string validationError;
                if (!isValid(naatiNumbers[i], firstNames[i], surNames[i], createdDates[i], out validationError))
                {
                    errorNaatiNumbers.Add(new PersonError()
                    {
                        naatiNumber = naatiNumbers[i],
                        errorMessage = validationError
                    });
                }
            }

            if (errorNaatiNumbers.Any() && !updateRecordsOnException)
            {
                return new ArchiveResponse
                {
                    Success = false,
                    UpdatedNaatiNumbers = 0,
                    ErrorNaatiNumbers = errorNaatiNumbers.ToArray(),
                    ErrorMessage = "Some of the supplied naati numbers were invalid."
                };
            }
            
            if (errorNaatiNumbers.Any())
            {
                errorMessage = "Some of the supplied naati numbers were invalid but all records were updated.";
            }

            var personQuery = NHibernateSession.Current.Query<NaatiDomain.Person>();

            foreach (int naatiNumber in naatiNumbers)
            {
                NaatiDomain.PersonArchiveHistory row;

                var person = personQuery.First(p => p.Entity.NaatiNumber == naatiNumber);

                if (person != null)
                {

                    row = new NaatiDomain.PersonArchiveHistory()
                    {
                        Person = person,
                        ArchiveDate = archiveDate
                    };

                    NHibernateSession.Current.Save(row);

                    updatedNaatiNumbers.Add(naatiNumber);
                }
            }

            NHibernateSession.Current.Flush();

            return new ArchiveResponse
            {
                Success = (!errorNaatiNumbers.Any() || (updateRecordsOnException && updatedNaatiNumbers.Count == naatiNumbers.Length)),
                UpdatedNaatiNumbers = updatedNaatiNumbers.Count,
                ErrorNaatiNumbers = errorNaatiNumbers.ToArray(),
                ErrorMessage = errorMessage
            };
        }

        private bool isValid(int naatiNumber, string firstName, string surName, string createdDate, out string errorString)
        {
            try
            {
                var personList = NHibernateSession.Current.Query<NaatiDomain.Person>().Where(p => p.Entity.NaatiNumber == naatiNumber);

                if (!personList.Any())
                {
                    errorString = "naati number not found.";
                    return false;
                }

                bool any = false;
                DateTime personEnteredDate;
                if (!DateTime.TryParseExact(createdDate, "d/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out personEnteredDate))
                {
                    errorString = string.Format("Failure to parse Created Date from file ({0}) into a valid date.", createdDate);
                    return false;
                }

                foreach (var person in personList)
                {
                    if (person.GivenName == firstName &&
                        person.Surname == surName &&
                        person.EnteredDate.Date == personEnteredDate.Date)
                    {
                        any = true;
                    }
                    
                }
                if (any)
                {
                    errorString = "";
                    return true;
                }

                if (personList.Count() == 1)
                {
                    string error = "";

                    var person = personList.First();

                    if (person.GivenName != firstName)
                    {
                        error += string.Format("First name in file ({0}) does not match database ({1}). ", firstName, person.GivenName);
                    }
                    if (person.Surname != surName)
                    {
                        error += string.Format("Surname in file ({0}) does not match database ({1}). ", surName, person.Surname);
                    }
                    if (person.EnteredDate.Date != personEnteredDate.Date)
                    {
                        error += string.Format("Created date in file ({0}) does not match database ({1}). ", personEnteredDate.Date, person.EnteredDate.Date);
                    }
                    errorString =  error;
                    return false;
                }

                errorString = "multiple records match naati number, no matchs for full details.";
                return false;
            }
            catch
            {
                errorString = "an exception was thrown during validation.";
                return false;
            }
        }

        private string trimCsvString(string csvString)
        {
            var trimDex = csvString.IndexOf("Naati Number", StringComparison.CurrentCultureIgnoreCase);
            if (trimDex != -1)
            {
                return csvString.Remove(0, trimDex);
            }

            return string.Empty;
        }
    }
}
