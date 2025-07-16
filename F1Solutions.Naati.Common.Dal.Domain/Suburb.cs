using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Suburb : EntityBase
    {
        public Suburb()
        {
            mPostcodes = new List<Postcode>();
        }

        private IList<Postcode> mPostcodes;

        public virtual IEnumerable<Postcode> Postcodes { get { return mPostcodes;} }

        public virtual void AddPostcode(Postcode postcode)
        {
            postcode.Suburb = this;
            mPostcodes.Add(postcode);
        }

        public virtual void RemovePostcode(Postcode postcode)
        {
            var result = (from p in mPostcodes where p.Id == postcode.Id select p).FirstOrDefault();
            if(result != null)
            {
                result.Suburb = null;
                mPostcodes.Remove(result);
            }
        }

        public virtual string Name { get; set; }
        public virtual State State { get; set; }
    }
}
