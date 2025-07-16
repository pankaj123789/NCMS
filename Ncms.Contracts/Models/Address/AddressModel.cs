using System.Collections.Generic;
using System.Text;

namespace Ncms.Contracts.Models.Address
{
    public class AddressModel
    {
        public string StreetName { get; set; }
        public string SubPremise { get; set; }
        public string StreetNumber { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public List<string> Types { get; set; }

        public bool IsFromAustralia
        {
            get { return Country == "Australia"; }
        }

        public string Address
        {
            get
            {
                var stringBuild = new StringBuilder();

                stringBuild.Append(SubPremise).Append(StreetNumber).Append(" ").Append(StreetName);

                return stringBuild.ToString();
            }
        }

        public string LatitudeLongitude
        {
            get
            {
                var stringBuild = new StringBuilder();
                stringBuild.Append(Latitude).Append(" ").Append(Longitude);

                return stringBuild.ToString();
            }
        }

        public string SuburbAndPostCode
        {
            get
            {
                var stringBuild = new StringBuilder();
                stringBuild.Append(Suburb).Append(" ").Append(PostCode);

                return stringBuild.ToString();
            }
        }

        public string FullAddress
        {
            get
            {
                var stringBuild = new StringBuilder();

                stringBuild.Append(SubPremise);
                stringBuild.Append(StreetNumber).Append(" ");
                stringBuild.Append(StreetName).Append(" ");
                stringBuild.Append(Suburb).Append(" ");
                stringBuild.Append(State).Append(" ");
                stringBuild.Append(PostCode).Append(" ");
                stringBuild.Append(Country);

                return stringBuild.ToString();
            }
        }
    }
}
