using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal
{
    public class ExaminerTestMaterialDtoTransformer : IResultTransformer
    {
        private readonly int mNaatiNumberIndex;
        private readonly int mNameIndex;
        private readonly int mPanelMembershipRoleNameIndex;
        private readonly int mPanelMembershipFromIndex;
        private readonly int mPanelMembershipToIndex;
        private readonly int mPanelIdIndex;
        private readonly int mPanelNameIndex;

        public ExaminerTestMaterialDtoTransformer(IReadOnlyDictionary<string, int> projectionIndices)
        {
            mNaatiNumberIndex = projectionIndices[nameof(ExaminerTestMaterialDto.NaatiNumber)];
            mNameIndex = projectionIndices[nameof(ExaminerTestMaterialDto.Name)];
            mPanelMembershipRoleNameIndex = projectionIndices[nameof(PanelMembershipDto.RoleName)];
            mPanelMembershipFromIndex = projectionIndices[nameof(PanelMembershipDto.From)];
            mPanelMembershipToIndex = projectionIndices[nameof(PanelMembershipDto.To)];
            mPanelIdIndex = projectionIndices[nameof(PanelDto.PanelId)];
            mPanelNameIndex = projectionIndices["PanelName"];
        }
        public object TransformTuple(object[] tuple, string[] aliases)
        {
            return tuple;
        }

        public IList TransformList(IList collection)
        {
            var personDictionary = new Dictionary<int, ExaminerTestMaterialDto>();
            foreach (object[] tuple in collection)
            {
                ExaminerTestMaterialDto dto;
                if (!personDictionary.TryGetValue((int)tuple[mNaatiNumberIndex], out dto))
                {
                    dto = new ExaminerTestMaterialDto
                    {
                        NaatiNumber = (int)tuple[mNaatiNumberIndex],
                        PanelMemberships = new HashSet<PanelMembershipDto>(),
                        Name = tuple[mNameIndex] as string,
                    };

                    personDictionary[dto.NaatiNumber] = dto;
                }

                dto.PanelMemberships.Add(new PanelMembershipDto
                {
                    RoleName = tuple[mPanelMembershipRoleNameIndex] as string,
                    From = (DateTime)tuple[mPanelMembershipFromIndex],
                    To = (DateTime)tuple[mPanelMembershipToIndex],
                    Panel = new PanelDto
                    {
                        PanelId = (int)tuple[mPanelIdIndex],
                        Name = tuple[mPanelNameIndex] as string
                    }
                });
            }

            return personDictionary.Values.ToList();
        }
    }
}
