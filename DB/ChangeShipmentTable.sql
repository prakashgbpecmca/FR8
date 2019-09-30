CREATE TABLE dbo.Tmp_Shipment(
	[ShipmentId] [int] IDENTITY(1,1) NOT NULL,
	[ShipmentStatusId] [int] NOT NULL,
	[CargoWiseSo] [varchar](20) NULL,
	[BarCodeSo] [varchar](50) NULL,
	[CustomerId] [int] NULL,
	[PurchaseOrderNumber] [varchar](20) NULL,
	[ShipperId] [int] NOT NULL,
	[ShipperAddressId] [int] NOT NULL,
	[ReceiverId] [int] NOT NULL,
	[ReceiverAddressId] [int] NOT NULL,
	[ShipmentTermCode] [varchar](5) NOT NULL,
	[PackagingTypeId] [int] NOT NULL,
	[SpecialDeliveryId] [int] NULL,
	[ContentDescription] [varchar](2000) NULL,
	[Guidelines] [varchar](2000) NULL,
	[ShipmentDuitable] [varchar](20) NULL,
	[ShippingReference] [varchar](100) NULL,
	[ShippingDate] [date] NOT NULL,
	[ShippingTime] [time](7) NOT NULL,
	[PaymentParty] [varchar](20) NOT NULL,
	[DeclaredValue] [money] NULL,
	[DeclaredCurrency] [varchar](10) NULL,
	[DeliveredBy] [int] NOT NULL,
	[ShipmentPickupAddressId] [int] NOT NULL,
	[ShipmentPickupContactName] [varchar](100) NULL,
	[ShipmentPickupContactPhoneNumber] [varchar](50) NULL,
	[TransportToWarehouseId] [int] NULL,
	[WarehouseId] [int] NULL,
	[TradeLaneId] [int] NOT NULL,
	[LocationType] [varchar](50) NULL,
	[LocationOfShipment] [varchar](100) NULL,
	[SpecialInstruction] [varchar](1000) NULL,
	[PickupDate] [date] NULL,
	[ShipmentReadyBy] [time](7) NULL,
	[TimezoneId] [int] NOT NULL,
	[TermAndConditionId] [int] NOT NULL,
	[CustomerConfirmCode] [uniqueidentifier] NULL,
	[CommercialInvoice] [varchar](200) NULL,
	[PackingList] [varchar](200) NULL,
	[CustomDocument] [varchar](200) NULL,
	[ControlNumber] [varchar](100) NULL,
	[AirWayBill] [varchar](100) NULL,
	[AirWayBillDocument] [varchar](200) NULL,
	[AnticipatedPickupDate] [date] NULL,
	[AnticipatedPickupTime] [time](7) NULL,
	[AWB] [varchar](200) NULL,
	[OtherDocs] [varchar](200) NULL,
	[OriginatingAgentId] [int] NULL,
	[OriginatingAddressId] [int] NULL,
	[OriginatingDeliveryDate] [date] NULL,
	[OriginatingDeliveryTime] [time](7) NULL,
	[WarehouseDropOffDate] [date] NULL,
	[WarehouseDropOffTime] [time](7) NULL,
	[FlightVessel] [varchar](50) NULL,
	[ETDDate] [date] NULL,
	[ETDTime] [time](7) NULL,
	[ETADate] [date] NULL,
	[ETATime] [time](7) NULL,
	[MABBL] [varchar](50) NULL,
	[DestinatingAgentId] [int] NULL,
	[DestinatingDeliveryDate] [date] NULL,
	[DestinatingDeliveryTime] [time](7) NULL,
	[DestinatingDeliveryCustomIssue] [varchar](2000) NULL,
	[FinalDeliveryDate] [date] NULL,
	[FinalDeliveryTime] [time](7) NULL,
	[Signature] [varchar](100) NULL,
	[FinalImage] [varchar](200) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[UpdatedOn] [datetime] NULL,
	[UpdatedBy] [int] NULL
)  ON [PRIMARY]
GO

SET IDENTITY_INSERT dbo.Tmp_Shipment ON
GO
IF EXISTS(SELECT * FROM dbo.Shipment)
	 EXEC('INSERT INTO dbo.Tmp_Shipment (ShipmentId,ShipmentStatusId,CargoWiseSo,BarCodeSo, CustomerId,PurchaseOrderNumber
           ,ShipperId,ShipperAddressId,ReceiverId,ReceiverAddressId,ShipmentTermCode,PackagingTypeId,SpecialDeliveryId
           ,ContentDescription,Guidelines,ShipmentDuitable,ShippingReference,ShippingDate,ShippingTime,PaymentParty
		   ,DeclaredValue,DeclaredCurrency,DeliveredBy,ShipmentPickupAddressId,ShipmentPickupContactName
		   ,ShipmentPickupContactPhoneNumber,TransportToWarehouseId,WarehouseId,TradeLaneId,LocationType
		   ,LocationOfShipment,SpecialInstruction,PickupDate,ShipmentReadyBy,TimezoneId,TermAndConditionId
           ,CustomerConfirmCode,CommercialInvoice,PackingList,CustomDocument,ControlNumber,AirWayBill
		   ,AirWayBillDocument,AnticipatedPickupDate,AnticipatedPickupTime,AWB,OtherDocs,OriginatingAgentId
		   ,OriginatingAddressId,OriginatingDeliveryDate,OriginatingDeliveryTime,WarehouseDropOffDate,WarehouseDropOffTime
           ,FlightVessel,ETDDate,ETDTime,ETADate,ETATime,MABBL,DestinatingAgentId,DestinatingDeliveryDate,DestinatingDeliveryTime
           ,DestinatingDeliveryCustomIssue,FinalDeliveryDate,FinalDeliveryTime,Signature,FinalImage
		   ,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy)
		SELECT ShipmentId,ShipmentStatusId,CargoWiseSo,BarCodeSo, CustomerId,PurchaseOrderNumber
           ,ShipperId,ShipperAddressId,ReceiverId,ReceiverAddressId,ShipmentTermCode,PackagingTypeId,SpecialDeliveryId
           ,ContentDescription,Guidelines,ShipmentDuitable,ShippingReference,ShippingDate,''12:00:00'',PaymentParty
		   ,DeclaredValue,DeclaredCurrency,DeliveredBy,ShipmentPickupAddressId,ShipmentPickupContactName
		   ,ShipmentPickupContactPhoneNumber,TransportToWarehouseId,WarehouseId,TradeLaneId,LocationType
		   ,LocationOfShipment,SpecialInstruction,PickupDate,ShipmentReadyBy,TimezoneId,TermAndConditionId
           ,CustomerConfirmCode,CommercialInvoice,PackingList,CustomDocument,ControlNumber,AirWayBill
		   ,AirWayBillDocument,AnticipatedPickupDate,AnticipatedPickupTime,AWB,OtherDocs,OriginatingAgentId
		   ,OriginatingAddressId,OriginatingDeliveryDate,OriginatingDeliveryTime,WarehouseDropOffDate,WarehouseDropOffTime
           ,FlightVessel,ETDDate,ETDTime,ETADate,ETATime,MABBL,DestinatingAgentId,DestinatingDeliveryDate,DestinatingDeliveryTime
           ,DestinatingDeliveryCustomIssue,FinalDeliveryDate,FinalDeliveryTime,Signature,FinalImage
		   ,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy 
		FROM dbo.Shipment WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Shipment OFF
GO
DROP TABLE dbo.Shipment
GO
EXECUTE sp_rename N'dbo.Tmp_Shipment', N'Shipment', 'OBJECT' 
GO
ALTER TABLE dbo.Shipment ADD CONSTRAINT PK_Shipment PRIMARY KEY CLUSTERED(
	ShipmentId
) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]