SET IDENTITY_INSERT ShipmentStatus ON
GO
INSERT INTO 
	ShipmentStatus 
	(ShipmentStatusId, StatusName)
     SELECT 1, 'NewBooking'
	 UNION
	 SELECT 2, 'CustomerConfirm'
	 UNION
	 SELECT 3, 'CustomerAmended'
	 UNION
	 SELECT 4, 'CustomerReject'
	 UNION
	 SELECT 5, 'CommericalInvoiceUploaded'
	 UNION
	 SELECT 6, 'AgentConfirm'
	 UNION
	 SELECT 7, 'AgentReject'
	 UNION
	 SELECT 8, 'UpdateFlightSea'
	 UNION
	 SELECT 9, 'AWBUploaded'
	 UNION
	 SELECT 10, 'DestinatingAgentAnticipated'
	 UNION
	 SELECT 11, 'Close'
GO
SET IDENTITY_INSERT ShipmentStatus OFF

