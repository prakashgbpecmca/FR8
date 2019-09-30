CREATE TABLE Tmp_Courier(
	[CourierId] [int] IDENTITY(1,1) NOT NULL,
	[CourierName] [varchar](100) NOT NULL,
	[Website] [varchar](100) NULL,
	[LatestBookingTime] [time](7) NOT NULL,
	[ShipmentType] [varchar](20) NULL
) ON [PRIMARY]

SET IDENTITY_INSERT Tmp_Courier ON
GO
IF EXISTS(SELECT * FROM Courier)
	 EXEC('INSERT INTO Tmp_Courier (CourierId,CourierName,Website,LatestBookingTime,ShipmentType)
		  SELECT CourierId,CourierName,Website,LatestBookingTime,CourierType 
		FROM Courier WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT Tmp_Courier OFF
GO
DROP TABLE Courier
GO
EXECUTE sp_rename N'dbo.Tmp_Courier', N'Courier', 'OBJECT' 
GO
ALTER TABLE Courier ADD CONSTRAINT PK_Courier PRIMARY KEY CLUSTERED(
	CourierId
) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]