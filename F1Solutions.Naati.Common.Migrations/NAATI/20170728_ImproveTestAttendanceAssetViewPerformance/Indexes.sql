CREATE NONCLUSTERED INDEX [_dta_index_tblEvent_7_53575229__K1_3] ON [dbo].[tblEvent]
(
	[EventId] ASC
)
INCLUDE ( 	[OfficeId]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [_dta_index_tblExaminerMarking_7_1034486764__K2_K1_K3_5_10] ON [dbo].[tblExaminerMarking]
(
	[TestResultID] ASC,
	[ExaminerMarkingID] ASC,
	[JobExaminerID] ASC
)
INCLUDE ( 	[CountMarks],
	[SubmittedDate]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [_dta_index_tblJob_7_197575742__K14] ON [dbo].[tblJob]
(
	[SettingMaterialId] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [_dta_index_tblJob_7_197575742__K1_K14] ON [dbo].[tblJob]
(
	[JobId] ASC,
	[SettingMaterialId] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

CREATE STATISTICS [_dta_stat_229575856_2_3_1] ON [dbo].[tblJobExaminer]([JobId], [PanelMembershipId], [JobExaminerID])

CREATE NONCLUSTERED INDEX [_dta_index_tblJobExaminer_7_229575856__K1_K2_K3] ON [dbo].[tblJobExaminer]
(
	[JobExaminerID] ASC,
	[JobId] ASC,
	[PanelMembershipId] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [_dta_index_tblPanelMembership_7_469576711__K1_K2] ON [dbo].[tblPanelMembership]
(
	[PanelMembershipId] ASC,
	[PersonId] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [_dta_index_tblPerson_7_597577167__K1_2] ON [dbo].[tblPerson]
(
	[PersonId] ASC
)
INCLUDE ( 	[EntityId]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX [_dta_index_tblPersonName_7_629577281__K2_K1_3_5] ON [dbo].[tblPersonName]
(
	[PersonId] ASC,
	[PersonNameId] ASC
)
INCLUDE ( 	[GivenName],
	[Surname]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

CREATE STATISTICS [_dta_stat_789577851_3_8_1] ON [dbo].[tblTestAttendance]([TestEventId], [Sat], [TestAttendanceId])

CREATE STATISTICS [_dta_stat_789577851_2_8_1] ON [dbo].[tblTestAttendance]([TestInvitationId], [Sat], [TestAttendanceId])

CREATE STATISTICS [_dta_stat_789577851_1_2_3_8] ON [dbo].[tblTestAttendance]([TestAttendanceId], [TestInvitationId], [TestEventId], [Sat])

CREATE NONCLUSTERED INDEX [_dta_index_tblTestEvent_7_853578079__K1_K2] ON [dbo].[tblTestEvent]
(
	[TestEventId] ASC,
	[EventId] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [_dta_index_tblTestInvitation_7_869578136__K1_K2] ON [dbo].[tblTestInvitation]
(
	[TestInvitationId] ASC,
	[ApplicationId] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [_dta_index_tblTestResult_7_901578250__K2_K3_K1_13] ON [dbo].[tblTestResult]
(
	[TestAttendanceId] ASC,
	[CurrentJobId] ASC,
	[TestResultId] ASC
)
INCLUDE ( 	[SatDate]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
