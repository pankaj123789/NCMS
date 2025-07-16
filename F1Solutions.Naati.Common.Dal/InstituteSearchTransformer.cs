using System.Collections;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using NHibernate.Transform;
using NHibernate.Util;

namespace F1Solutions.Naati.Common.Dal
{
    public class InstituteSearchTransformer : IResultTransformer
    {


        public readonly int ContactIdProperty;
        private readonly int mContactIdIndex;
        private readonly int mInstituteIdIndex;
        private readonly int mEntityIdIndex;
        private readonly int mNaatiNumberIndex;
        private readonly int mPrimaryEmailIndex;
        private readonly int mPrimaryContactNumberIndex;
        private readonly int mNameIndex;

        public InstituteSearchTransformer(IReadOnlyDictionary<string, int> projectionIndices)
        {
            mInstituteIdIndex = projectionIndices[nameof(InstituteSearchDto.InstituteId)];
            mNameIndex = projectionIndices[nameof(InstituteSearchDto.Name)];
            mNaatiNumberIndex = projectionIndices[nameof(InstituteSearchDto.NaatiNumber)];
            mPrimaryEmailIndex = projectionIndices[nameof(InstituteSearchDto.PrimaryEmail)];
            mPrimaryContactNumberIndex = projectionIndices[nameof(InstituteSearchDto.PrimaryContactNo)];
            mContactIdIndex = projectionIndices[nameof(ContactIdProperty)];
            mEntityIdIndex = projectionIndices[nameof(InstituteSearchDto.EntityId)];

        }

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            return tuple;
        }

        public IList TransformList(IList collection)
        {
            var instituteDictionary = new Dictionary<int, InstituteSearchDto>();
            foreach (object[] tuple in collection)
            {
                InstituteSearchDto institute;
                if (!instituteDictionary.TryGetValue((int) tuple[mInstituteIdIndex], out institute))
                {
                    institute = new InstituteSearchDto
                    {
                        InstituteId = (int) tuple[mInstituteIdIndex],
                        EntityId = (int) tuple[mEntityIdIndex],
                        NaatiNumber = tuple[mNaatiNumberIndex]==null?(int?)null:(int) tuple[mNaatiNumberIndex],
                        Name = tuple[mNameIndex] as string,
                        ContactIds = new HashSet<int>()
                        
                    };

                    instituteDictionary[institute.InstituteId] = institute;
                }

                UpdateContactIdList(institute, tuple);

                SetPrimaryEmail(institute, tuple);
                SetPrimaryContactNumber(institute, tuple);
            }

            foreach(var value in instituteDictionary.Values)
            {
                value.NoOfContacts = value.ContactIds.Count; 
            }

            return instituteDictionary.Values.ToList();
        }

        public void UpdateContactIdList(InstituteSearchDto institute, object[] tuple)
        {

            if (tuple[mContactIdIndex] != null)
            {
                institute.ContactIds.Add((int)tuple[mContactIdIndex]);
            }
            
        }

        private void SetPrimaryEmail(InstituteSearchDto institute, object[] tuple)
        {
            var email = tuple[mPrimaryEmailIndex];
            if (email != null)
            {
                institute.PrimaryEmail = email as string;
            }
        }

        private void SetPrimaryContactNumber(InstituteSearchDto institute, object[] tuple)
        {
            var primaryNumber = tuple[mPrimaryContactNumberIndex];
            if (primaryNumber != null)
            {
                institute.PrimaryContactNo = primaryNumber as string;
            }
        }
    }
}

