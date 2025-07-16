    update tblTestSession set Capacity = (select Capacity from tblVenue where VenueId = tblTestSession.VenueId)    
	where Completed = 1