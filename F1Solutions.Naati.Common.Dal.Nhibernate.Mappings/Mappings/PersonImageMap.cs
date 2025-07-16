using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class PersonImageMap : IAutoMappingOverride<PersonImage>
    {
        #region IAutoMappingOverride<Person> Members

        public void Override(AutoMapping<PersonImage> mapping)
        {
            //This shouldn't be necessary but this version of nhibernate has a bug
            mapping.Map(p => p.ApplicationFirstPage).CustomType("BinaryBlob").Length(int.MaxValue).Nullable();
            mapping.Map(p => p.ApplicationLastPage).CustomType("BinaryBlob").Length(int.MaxValue).Nullable();
            mapping.Map(p => p.Photo).CustomType("BinaryBlob").Length(int.MaxValue).Nullable();
            mapping.Map(p => p.Signature).CustomType("BinaryBlob").Length(int.MaxValue).Nullable();
        }

        #endregion
    }
}