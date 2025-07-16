using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class MaterialRequest : EntityBase
    {
        private IList<MaterialRequestNote> mMaterialRequestNotes;
        private IList<MaterialRequestPublicNote> mMaterialRequestPublicNotes;
        private IList<MaterialRequestPanelMembership> mMaterialRequestRoundPanelMemberships;
        public virtual Panel Panel { get; set; }
        public virtual TestMaterial OutputMaterial { get; set; }
        public virtual MaterialRequestStatusType MaterialRequestStatusType { get; set; }
        public virtual  TestMaterial SourceMaterial { get; set; }
        public virtual  DateTime CreatedDate { get; set; }
        public virtual  User CreatedByUser { get; set; }
        public virtual DateTime StatusChangeDate { get; set; }
        public virtual User StatusChangeUser { get; set; }
        public virtual User OwnedByUser { get; set; }
        public virtual ProductSpecification ProductSpecification { get; set; }
        public virtual double MaxBillableHours { get; set; }
        public virtual IEnumerable<MaterialRequestNote> MaterialRequestNotes => mMaterialRequestNotes;
        public virtual IEnumerable<MaterialRequestPublicNote> MaterialRequestPublicNotes => mMaterialRequestPublicNotes;

        public virtual IEnumerable<MaterialRequestPanelMembership> MaterialRequestRoundPanelMemberships => mMaterialRequestRoundPanelMemberships;

        public MaterialRequest()
        {
            mMaterialRequestNotes = new List<MaterialRequestNote>();
            mMaterialRequestPublicNotes = new List<MaterialRequestPublicNote>();
            mMaterialRequestRoundPanelMemberships = new List<MaterialRequestPanelMembership>();
        }
        public virtual void RemoveMaterialRequestNote(MaterialRequestNote materialRequestNote)
        {
            var result = (from pn in mMaterialRequestNotes
                          where pn.Id == materialRequestNote.Id
                select pn).SingleOrDefault();

            if (result != null)
            {
                mMaterialRequestNotes.Remove(result);
                materialRequestNote.MaterialRequest = null;
            }
        }
        public virtual void RemoveMaterialRequestPublicNote(MaterialRequestPublicNote materialRequestPublicNote)
        {
            var result = (from pn in mMaterialRequestPublicNotes
                          where pn.Id == materialRequestPublicNote.Id
                select pn).SingleOrDefault();

            if (result != null)
            {
                mMaterialRequestPublicNotes.Remove(result);
                materialRequestPublicNote.MaterialRequest = null;
            }
        }
    }
}
