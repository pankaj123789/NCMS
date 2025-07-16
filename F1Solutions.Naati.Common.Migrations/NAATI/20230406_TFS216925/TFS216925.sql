  update tblTestComponentType set CandidateBriefRequired = 1, CandidateBriefAvailabilityDays = 3
  where 
	Name = 'CPDI Task Type A' and TestSpecificationId = 
	(Select TestSpecificationId from tblTestSpecification 
	where Description = 'Certified Provisional Deaf Interpreter' and Active = 1)