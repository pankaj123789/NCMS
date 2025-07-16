using System.Collections;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using NHibernate.Transform;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Dal
{
    public class PersonSearchTransformer : IResultTransformer
    {

        public readonly bool ActiveApplicationProperty;
        public readonly int CredentialStatusProperty;
        public readonly int CredentialCertificationProperty;
        public readonly int IsExaminer;
        public readonly int IsRolePlayer;

        private readonly int mPersonIdIndex;
        private readonly int mEntityIdIndex;
        private readonly int mNaatiNumberIndex;
        private readonly int mPrimaryEmailIndex;
        private readonly int mPrimaryContactNumberIndex;
        private readonly int mNameIndex;
        private readonly int mPractitionerNumberIndex;
        private readonly int mIsActiveApplicationIndex;

        private readonly int mCredentialStatusIndex;
        private readonly int mIsCredentialCertficationIndex;
        private readonly int mIsEportalActiveIndex;
        private readonly int mIsExaminerIndex;
        private readonly int mIsRolePlayerIndex;


        public object TransformTuple(object[] tuple, string[] aliases)
        {
            return tuple;
        }

        public IList TransformList(IList collection)
        {
            var personDictionary = new Dictionary<int, PersonSearchDto>();
            foreach (object[] tuple in collection)
            {
                PersonSearchDto person;
                if (!personDictionary.TryGetValue((int)tuple[mPersonIdIndex], out person))
                {
                    person = new PersonSearchDto
                    {
                        PersonId = (int) tuple[mPersonIdIndex],
                        NaatiNumber = (int) tuple[mNaatiNumberIndex],
                        PersonTypes = new HashSet<PersonType>(),
                        PractitionerNumber = tuple[mPractitionerNumberIndex] as string,
                        Name = tuple[mNameIndex] as string,
                        IsEportalActive = (bool) tuple[mIsEportalActiveIndex],
                        EntityId =  (int)tuple[mEntityIdIndex],
                    };

                    personDictionary[person.PersonId] = person;
                }

                UpdatePersonTypeList(person, tuple);
                SetPrimaryEmail(person, tuple);
                SetPrimaryContactNumber(person, tuple);
            }

            return personDictionary.Values.ToList();

        }

        public PersonSearchTransformer(IReadOnlyDictionary<string, int> projectionIndices)
        {
            mPersonIdIndex = projectionIndices[nameof(PersonSearchDto.PersonId)];
            mNaatiNumberIndex = projectionIndices[nameof(PersonSearchDto.NaatiNumber)];
            mPrimaryEmailIndex = projectionIndices[nameof(PersonSearchDto.PrimaryEmail)];
            mPrimaryContactNumberIndex = projectionIndices[nameof(PersonSearchDto.PrimaryContactNumber)];
            mNameIndex = projectionIndices[nameof(PersonSearchDto.Name)];
            mPractitionerNumberIndex = projectionIndices[nameof(PersonSearchDto.PractitionerNumber)];
            mIsActiveApplicationIndex = projectionIndices[nameof(ActiveApplicationProperty)];
            mCredentialStatusIndex = projectionIndices[nameof(CredentialStatusProperty)];
            mIsCredentialCertficationIndex = projectionIndices[nameof(CredentialCertificationProperty)];
            mIsEportalActiveIndex = projectionIndices[nameof(PersonSearchDto.IsEportalActive)];
            mIsExaminerIndex = projectionIndices[nameof(IsExaminer)];
            mIsRolePlayerIndex = projectionIndices[nameof(IsRolePlayer)];
            mEntityIdIndex = projectionIndices[nameof(PersonSearchDto.EntityId)];
        }

        public void UpdatePersonTypeList(PersonSearchDto person, object[] tuple)
        {
            if ((bool)tuple[mIsActiveApplicationIndex])
            {
                person.PersonTypes.Add(PersonType.Applicant);
            }

            if ((bool)tuple[mIsExaminerIndex])
            {
                person.PersonTypes.Add(PersonType.Examiner);
            }

            if ((bool)tuple[mIsRolePlayerIndex])
            {
                person.PersonTypes.Add(PersonType.RolePlayer);
            }

            if (((bool?)tuple[mIsCredentialCertficationIndex]).GetValueOrDefault())
            {
                var credentialStatusId = tuple[mCredentialStatusIndex] != null ? (int)tuple[mCredentialStatusIndex] :(int)CredentialStatusTypeName.Unknown;
                switch (credentialStatusId)
                {
                    case (int)CredentialStatusTypeName.Expired:
                    case (int)CredentialStatusTypeName.Terminated:
                        if (!person.PersonTypes.Contains(PersonType.Practitioner) && !person.PersonTypes.Contains(PersonType.FuturePractitioner))
                        {
                            person.PersonTypes.Add(PersonType.FormerPractitioner);
                        }
                        break;
                    case (int)CredentialStatusTypeName.Future:
                        if (!person.PersonTypes.Contains(PersonType.Practitioner))
                        {
                            person.PersonTypes.Remove(PersonType.FormerPractitioner);
                            person.PersonTypes.Add(PersonType.FuturePractitioner);
                        }
                        break;
                    case (int)CredentialStatusTypeName.Active:
                        person.PersonTypes.Remove(PersonType.FormerPractitioner);
                        person.PersonTypes.Remove(PersonType.FuturePractitioner);
                        person.PersonTypes.Add(PersonType.Practitioner);
                        break;
                }
            }
        }
        private void SetPrimaryEmail(PersonSearchDto person, object[] tuple)
        {
            var email = tuple[mPrimaryEmailIndex];
            if (email != null)
            {
                person.PrimaryEmail = email as string;
            }
        }

        private void SetPrimaryContactNumber(PersonSearchDto person, object[] tuple)
        {
            var primaryNumber = tuple[mPrimaryContactNumberIndex];
            if (primaryNumber != null)
            {
                person.PrimaryContactNumber = primaryNumber as string;
            }
        }
    }
}
