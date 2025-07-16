
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171211_ProductTables
{
    [NaatiMigration(201712111000)]
    public class ProductTables : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"
                            ALTER TABLE [dbo].[tblProductSpecification] DROP CONSTRAINT [FK_tblProductSpecification_tblGLCode1]
                            GO
                            drop table [tblGLCode]


                            CREATE TABLE [dbo].[tblGLCode](
	                            [GLCodeId] [int] IDENTITY(1,1) NOT NULL,
	                            [Code] [char](6) NOT NULL,
	                            [AccountName] [varchar](50) NOT NULL,
	                            [Description] [varchar](500) NOT NULL,
	                            [RowVersion] [timestamp] NULL,	
                             CONSTRAINT [PK_tblGLCode] PRIMARY KEY CLUSTERED 
                            (
	                            [GLCodeId] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
                            ) ON [PRIMARY]

                            GO

                            ALTER TABLE [dbo].[tblProductSpecification]  WITH NOCHECK ADD  CONSTRAINT [FK_tblProductSpecification_tblGLCode1] FOREIGN KEY([GLCodeId])
                            REFERENCES [dbo].[tblGLCode] ([GLCodeId])
                            GO

                            ALTER TABLE [dbo].[tblProductSpecification] NOCHECK CONSTRAINT [FK_tblProductSpecification_tblGLCode1]


                            ALTER TABLE tblJobExaminer DROP CONSTRAINT FK_tblJobExaminer_tblProductSpecification

                            ALTER TABLE tblTestAvailability DROP CONSTRAINT FK_tblTestAvailability_tblProductSpecification1

                            ALTER TABLE tblTestAvailability DROP CONSTRAINT FK_tblTestAvailability_tblProductSpecification

                            ALTER TABLE tblWorkshopEvent DROP CONSTRAINT FK_tblWorkshopEvent_tblProductSpecification
                            ALTER TABLE tblCredentialWorkflowFee DROP CONSTRAINT FK_CredentialWorkflowFee_ProductSpecification


                            drop table tblProductSpecification


                            CREATE TABLE [dbo].[tblProductSpecification](
	                            [ProductSpecificationId] [int] IDENTITY(1,1) NOT NULL,
	                            [ProductCategoryId] [int] NOT NULL,
	                            [Name] [varchar](50) NOT NULL,
	                            [Description] [varchar](500) NOT NULL,
	                            [Code] [varchar](10) NOT NULL,
	                            [CostPerUnit] [money] NOT NULL,
	                            [GSTApplies] [bit] NOT NULL,
	                            [GLCodeId] [int] NOT NULL,
	                            [BatchQuantity] [int] NOT NULL,
	                            [RowVersion] [timestamp] NULL,
	                            [Inactive] [bit] NOT NULL,
	                            [JobTypeId] [int] NOT NULL,
	                            [TrackingActivity] [varchar](255) NULL,
                             CONSTRAINT [PK_tblProductSpecification] PRIMARY KEY CLUSTERED 
                            (
	                            [ProductSpecificationId] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
                            ) ON [PRIMARY]

                            GO

                            ALTER TABLE [dbo].[tblProductSpecification] ADD  CONSTRAINT [DF_tblProductSpecification_Inactive]  DEFAULT (0) FOR [Inactive]
                            GO

                            ALTER TABLE [dbo].[tblProductSpecification] ADD  CONSTRAINT [DF_tblProductSpecification_JobTypeId]  DEFAULT ((1)) FOR [JobTypeId]
                            GO

                            ALTER TABLE [dbo].[tblProductSpecification]  WITH NOCHECK ADD  CONSTRAINT [FK_tblProductSpecification_tblGLCode1] FOREIGN KEY([GLCodeId])
                            REFERENCES [dbo].[tblGLCode] ([GLCodeId])
                            GO

                            ALTER TABLE [dbo].[tblProductSpecification] NOCHECK CONSTRAINT [FK_tblProductSpecification_tblGLCode1]
                            GO

                            ALTER TABLE [dbo].[tblProductSpecification]  WITH CHECK ADD  CONSTRAINT [FK_tblProductSpecification_tluJobType] FOREIGN KEY([JobTypeId])
                            REFERENCES [dbo].[tluJobType] ([JobTypeId])
                            GO

                            ALTER TABLE [dbo].[tblProductSpecification] CHECK CONSTRAINT [FK_tblProductSpecification_tluJobType]
                            GO

                            ALTER TABLE [dbo].[tblProductSpecification]  WITH CHECK ADD  CONSTRAINT [FK_tblProductSpecification_tluProductCategory] FOREIGN KEY([ProductCategoryId])
                            REFERENCES [dbo].[tluProductCategory] ([ProductCategoryId])
                            GO

                            ALTER TABLE [dbo].[tblProductSpecification] CHECK CONSTRAINT [FK_tblProductSpecification_tluProductCategory]
                            GO


                            ALTER TABLE [dbo].[tblJobExaminer]  WITH CHECK ADD  CONSTRAINT [FK_tblJobExaminer_tblProductSpecification] FOREIGN KEY([ProductSpecificationId])
                            REFERENCES [dbo].[tblProductSpecification] ([ProductSpecificationId])
                            GO

                            ALTER TABLE [dbo].[tblJobExaminer] CHECK CONSTRAINT [FK_tblJobExaminer_tblProductSpecification]
                            GO

                            ALTER TABLE [dbo].[tblTestAvailability]  WITH CHECK ADD  CONSTRAINT [FK_tblTestAvailability_tblProductSpecification1] FOREIGN KEY([NZScheduledProductSpecificationId])
                            REFERENCES [dbo].[tblProductSpecification] ([ProductSpecificationId])
                            GO

                            ALTER TABLE [dbo].[tblTestAvailability] CHECK CONSTRAINT [FK_tblTestAvailability_tblProductSpecification1]
                            GO

                            ALTER TABLE [dbo].[tblTestAvailability]  WITH CHECK ADD  CONSTRAINT [FK_tblTestAvailability_tblProductSpecification] FOREIGN KEY([ScheduledProductSpecificationId])
                            REFERENCES [dbo].[tblProductSpecification] ([ProductSpecificationId])
                            GO

                            ALTER TABLE [dbo].[tblTestAvailability] CHECK CONSTRAINT [FK_tblTestAvailability_tblProductSpecification]
                            GO

                            ALTER TABLE [dbo].[tblWorkshopEvent]  WITH CHECK ADD  CONSTRAINT [FK_tblWorkshopEvent_tblProductSpecification] FOREIGN KEY([ProductSpecificationId])
                            REFERENCES [dbo].[tblProductSpecification] ([ProductSpecificationId])
                            GO

                            ALTER TABLE [dbo].[tblWorkshopEvent] CHECK CONSTRAINT [FK_tblWorkshopEvent_tblProductSpecification]
                            GO


                            ALTER TABLE [dbo].[tblCredentialWorkflowFee]  WITH CHECK ADD  CONSTRAINT [FK_CredentialWorkflowFee_ProductSpecification] FOREIGN KEY([ProductSpecificationId])
                            REFERENCES [dbo].[tblProductSpecification] ([ProductSpecificationId])
                            GO

                            ALTER TABLE [dbo].[tblCredentialWorkflowFee] CHECK CONSTRAINT [FK_CredentialWorkflowFee_ProductSpecification]
                            GO

                            ALTER TABLE [dbo].[tblProductSpecification] DROP CONSTRAINT [FK_tblProductSpecification_tluProductCategory]
                            GO
                            drop table [tluProductCategory]


                            CREATE TABLE [dbo].[tluProductCategory](
	                            [ProductCategoryId] [int] IDENTITY(1,1) NOT NULL,
	                            [Name] [varchar](50) NOT NULL,
	                            [Code] [varchar](10) NOT NULL,
	                            [RowVersion] [timestamp] NULL,
	                            [ProductTypeId] [int] NULL,
                             CONSTRAINT [PK_tluProductCategory] PRIMARY KEY CLUSTERED 
                            (
	                            [ProductCategoryId] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
                            ) ON [PRIMARY]

                            GO

                            ALTER TABLE [dbo].[tluProductCategory]  WITH CHECK ADD  CONSTRAINT [FK_tluProductCategory_tluProductType] FOREIGN KEY([ProductTypeId])
                            REFERENCES [dbo].[tluProductType] ([ProductTypeId])
                            GO

                            ALTER TABLE [dbo].[tluProductCategory] CHECK CONSTRAINT [FK_tluProductCategory_tluProductType]
                            GO

                            ALTER TABLE [dbo].[tblProductSpecification]  WITH CHECK ADD  CONSTRAINT [FK_tblProductSpecification_tluProductCategory] FOREIGN KEY([ProductCategoryId])
                            REFERENCES [dbo].[tluProductCategory] ([ProductCategoryId])
                            GO

                            ALTER TABLE [dbo].[tblProductSpecification] CHECK CONSTRAINT [FK_tblProductSpecification_tluProductCategory]
                            GO


                            delete tblProductSpecification
                            delete tluProductCategory
                            delete tluProductType                 
                            delete tblGLCode                          
 
                            alter table tblProductSpecification
                            alter column Code nvarchar(20)
                        ");

        }
    }
}
