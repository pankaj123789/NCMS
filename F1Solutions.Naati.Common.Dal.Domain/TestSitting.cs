using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestSitting : EntityBase
    {
        private IList<TestResult> mTestResults = new List<TestResult>();
        private IList<TestSittingNote> mTestSittingNotes = new List<TestSittingNote>();
        private IList<TestSittingDocument> mTestSittingDocuments = new List<TestSittingDocument>();
        private IList<TestSittingTestMaterial> mTestSittingTestMaterials = new List<TestSittingTestMaterial>();

        public virtual TestSession TestSession { get; set; }
        public virtual TestSpecification TestSpecification { get; set; }
        public virtual CredentialRequest CredentialRequest { get; set; }
        public virtual IList<TestSittingNote> TestSittingNotes => mTestSittingNotes;
        public virtual IList<TestResult> TestResults => mTestResults;
        public virtual IList<TestSittingDocument> TestSittingDocuments => mTestSittingDocuments;

        public virtual IList<TestSittingTestMaterial> TestSittingTestMaterials => mTestSittingTestMaterials;
        public virtual bool Supplementary { get; set; }

        public virtual TestStatus TestStatus { get; set; }
        public virtual bool Sat { get; set; }
        public virtual bool Rejected { get; set; }
        public virtual DateTime? RejectedDate { get; set; }
        public virtual DateTime AllocatedDate { get; set; }

        public override IAuditObject RootAuditObject => CredentialRequest;


        public virtual bool HasDefaultSpecification()
        {
            return this.TestSpecification.Id == this.TestSession.DefaultTestSpecification.Id;
        }

        public virtual void RemoveTestNote(TestSittingNote testSittingNote)
        {
            var result = (from pn in mTestSittingNotes
                          where pn.Id == testSittingNote.Id
                          select pn).SingleOrDefault();

            if (result != null)
            {
                mTestSittingNotes.Remove(result);
                testSittingNote.TestSitting = null;
            }
        }
    }
}
