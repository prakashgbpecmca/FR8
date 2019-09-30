/****** Object:  Table [dbo].[SpecialDelivery]    Script Date: 11/1/2015 1:40:52 PM ******/
DROP TABLE [dbo].[SpecialDelivery]
GO
/****** Object:  Table [dbo].[ShipmentTerm]    Script Date: 11/1/2015 1:40:52 PM ******/
DROP TABLE [dbo].[ShipmentTerm]
GO
/****** Object:  Table [dbo].[ShipmentTerm]    Script Date: 11/1/2015 1:40:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ShipmentTerm](
	[Code] [varchar](5) NOT NULL,
	[Detail] [varchar](50) NOT NULL,
 CONSTRAINT [PK_ShipmentTerm] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SpecialDelivery]    Script Date: 11/1/2015 1:40:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SpecialDelivery](
	[SpecialDeliveryId] [int] IDENTITY(1,1) NOT NULL,
	[Detail] [varchar](50) NOT NULL,
 CONSTRAINT [PK_SpecialDelivery] PRIMARY KEY CLUSTERED 
(
	[SpecialDeliveryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[ShipmentTerm] ([Code], [Detail]) VALUES (N'CFR', N'Cost and Freight')
GO
INSERT [dbo].[ShipmentTerm] ([Code], [Detail]) VALUES (N'CIF', N'Cost, Insurance and Freight')
GO
INSERT [dbo].[ShipmentTerm] ([Code], [Detail]) VALUES (N'CIP', N'Carriage and Insurance paid')
GO
INSERT [dbo].[ShipmentTerm] ([Code], [Detail]) VALUES (N'CPT', N'Carriage paid to')
GO
INSERT [dbo].[ShipmentTerm] ([Code], [Detail]) VALUES (N'DAP', N'Delivered at place')
GO
INSERT [dbo].[ShipmentTerm] ([Code], [Detail]) VALUES (N'DAT', N'Delivered at terminal')
GO
INSERT [dbo].[ShipmentTerm] ([Code], [Detail]) VALUES (N'DDP', N'Delivered duty paid')
GO
INSERT [dbo].[ShipmentTerm] ([Code], [Detail]) VALUES (N'EXW', N'Ex Works')
GO
INSERT [dbo].[ShipmentTerm] ([Code], [Detail]) VALUES (N'FAS', N'Free alongside ship')
GO
INSERT [dbo].[ShipmentTerm] ([Code], [Detail]) VALUES (N'FCA', N'Free Carrier')
GO
INSERT [dbo].[ShipmentTerm] ([Code], [Detail]) VALUES (N'FOB', N'Free On Board')
GO
SET IDENTITY_INSERT [dbo].[SpecialDelivery] ON 

GO
INSERT [dbo].[SpecialDelivery] ([SpecialDeliveryId], [Detail]) VALUES (1, N'Next Day')
GO
INSERT [dbo].[SpecialDelivery] ([SpecialDeliveryId], [Detail]) VALUES (2, N'NDS')
GO
INSERT [dbo].[SpecialDelivery] ([SpecialDeliveryId], [Detail]) VALUES (3, N'Public Holiday')
GO
INSERT [dbo].[SpecialDelivery] ([SpecialDeliveryId], [Detail]) VALUES (4, N'Weekend')
GO
SET IDENTITY_INSERT [dbo].[SpecialDelivery] OFF
GO
