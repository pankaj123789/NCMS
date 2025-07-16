	delete from tblPanelNote
	DBCC CHECKIDENT('tblPanelNote', RESEED, 0);	

	delete from tblPanelMembership
	update tblTableData
	set NextKey = 1
	where TableName = 'PanelMembership'

	delete from tblPanel
	update tblTableData
	set NextKey = 1
	where TableName = 'Panel'
