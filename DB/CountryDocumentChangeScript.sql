CREATE TABLE Tmp_CountryDocument(
	[CountryDocumentId] [int] IDENTITY(1,1) NOT NULL,
	[CountryId] [int] NOT NULL,
	[ShipmentType] [varchar](20) NOT NULL,
	[DocumentName] [varchar](100) NOT NULL 
) ON [PRIMARY]

SET IDENTITY_INSERT Tmp_CountryDocument ON
GO
IF EXISTS(SELECT * FROM CountryDocument)
	 EXEC('INSERT INTO Tmp_CountryDocument (CountryDocumentId,CountryId,ShipmentType,DocumentName)
		  SELECT CountryDocumentId,CountryId,
		ShipmentType = 
			CASE 
				WHEN DocumentTypeId = 1 Then ''Air''
				WHEN DocumentTypeId = 2 Then ''Courier''
				WHEN DocumentTypeId = 3 Then ''Expryes''
				WHEN DocumentTypeId = 4 Then ''Sea''
			END
				,DocumentName 
		FROM CountryDocument WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT Tmp_CountryDocument OFF
GO
DROP TABLE CountryDocument
GO
EXECUTE sp_rename N'dbo.Tmp_CountryDocument', N'CountryDocument', 'OBJECT' 
GO
ALTER TABLE dbo.CountryDocument ADD CONSTRAINT PK_CountryDocument PRIMARY KEY CLUSTERED(
	CountryDocumentId
) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]