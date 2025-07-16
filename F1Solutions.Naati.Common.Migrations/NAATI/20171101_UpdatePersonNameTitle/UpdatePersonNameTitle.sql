  declare @TitleId int

  --UPDATE
  update [dbo].[tluTitle]
  set Title = 'Associate Prof.'
  where Title = 'Associate Professor'

  update [dbo].[tluTitle]
  set Title = 'Prof.'
  where Title = 'Professor'
 	
  update [dbo].tblPersonName
	set TitleId = null
	where TitleId in (select titleId from tluTitle where Title in ('FLGLT', 'General', 'Mgr', 'Reverend', 'Inspector' ))
   delete from tluTitle where Title in ('FLGLT', 'General', 'Mgr', 'Reverend', 'Inspector' )

   select * from [dbo].tblPersonName
   select * from tluTitle