using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Bl.DTO
{
    
    public class PdActivityPoints
    {
        
        public int MinPoints { get; set; }
        
        public int Points { get; set; }
        
        public bool Completed { get; set; }
        
        public IEnumerable<PdActivitySectionPoints> Sections { get; set; }
        
        public IEnumerable<PdPreRequisite> PreRequisites { get; set; }
        
        public IEnumerable<int> IncludedActivitiesIds { get; set; }
        
        public bool Calculated { get; set; }

       }

    
    public class PdActivitySectionPoints
    {
        
        public int SectionId { get; set; }

        
        public string SectionName { get; set; }


        
        public bool Completed { get; set; }
        
        public IEnumerable<PdPreRequisite> PreRequisites { get; set; }
        
        public IEnumerable<PdCategoryPoints> Categories { get; set; }

        

        public int Points { get; set; }
        
        public int MinPoints { get; set; }

    }


    
    public class PdCategoryPoints
    {
        
        public int CategoryId { get; set; }

        
        public string CategoryName { get; set; }

        
        public bool Completed { get; set; }
        
        public IEnumerable<PdPreRequisite> PreRequisites { get; set; }

        
        public int Points { get; set; }
        
        public int MinPoints { get; set; }

    }

    public class PdPreRequisite
    {
        public bool Completed { get; set; }

        public string Message { get; set; }
    }
}
