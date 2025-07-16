using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Panel : EntityBase
    {

        public Panel()
        {
            mPanelMemberships = new List<PanelMembership>();
            mPanelNotes = new List<PanelNote>();
        }

        public virtual Language Language { get; set; }
        public virtual string Name { get; set; }
        public virtual string Note { get; set; }
        public virtual PanelType PanelType { get; set; }
        public virtual DateTime CommissionedDate { get; set; }
        public virtual bool VisibleInEportal { get; set; }

        private IList<PanelMembership> mPanelMemberships;
        private IList<PanelNote> mPanelNotes;
       
        public virtual IEnumerable<PanelMembership> PanelMemberships
        {
            get
            {
                return mPanelMemberships;
            }
        }

        public virtual IEnumerable<PanelNote> PanelNotes
        {
            get
            {
                return mPanelNotes;
            }
        }

        public virtual void AddPanelMembership(PanelMembership panelMembership)
        {
            panelMembership.Panel = this;
            mPanelMemberships.Add(panelMembership);
        }

        public virtual void RemovePanelMembership(PanelMembership panelMembership)
        {
            var result = (from pm in mPanelMemberships
                          where pm.Id == panelMembership.Id
                          select pm).SingleOrDefault();

            if (result != null)
            {
                mPanelMemberships.Remove(result);
                panelMembership.Panel = null;
            }
        }

        public virtual void AddPanelNote(PanelNote panelNote)
        {
            panelNote.Panel = this;
            mPanelNotes.Add(panelNote);
        }

        public virtual void RemovePanelNote(PanelNote panelNote)
        {
            var result = (from pn in mPanelNotes
                          where pn.Id == panelNote.Id
                          select pn).SingleOrDefault();

            if (result != null)
            {
                mPanelNotes.Remove(result);
                panelNote.Panel = null;
            }
        }
    }
}
