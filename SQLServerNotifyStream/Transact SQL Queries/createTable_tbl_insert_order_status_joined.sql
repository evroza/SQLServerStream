USE [DentalManager]
GO

/****** Object:  Table [dbo].[tr_insert_order_status_joined]    Script Date: 6/30/2018 12:27:43 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tbl_processedHistory] (
  [HistoryID] int NOT NULL IDENTITY (1, 1),
  [ModelElementID] [varchar](150) NOT NULL,
  [ModelJobID] [varchar](150) NULL,
  [MaterialID] [nvarchar](150) NULL,
  [AlreadyExported] [bit] NOT NULL DEFAULT ('FALSE'), /* Acknowledgement messages will be stored here*/
  [OrderID] [varchar](150) NULL, /* Joined from dbo.Modeljob on dbo.ModelElement.ModelJobID = dbo.Modeljob.ModelJobID*/
  [EventDateTime] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, /* Time when this event was captured */
  [ProcessStatusID] [varchar](150) NULL,
  [ProcessLockID] [varchar](150) NULL,
  [ManufacturerID] [varchar](150) NULL,
  [ManufacturingProcessID] [varchar](150) NULL,
  [ModelTransformation] [varchar](2000) NULL,
  [ModelHeight] [real] NULL,
  [ModelFilename] [nvarchar](1000) NULL,
  [ModelVolume] [real] NOT NULL DEFAULT ((0)),
  [ModelBoundingBoxMin] [varchar](200) NOT NULL,
  [ModelBoundingBoxMax] [varchar](200) NOT NULL DEFAULT (''),
  [Location1] [varchar](40) NOT NULL DEFAULT (''),
  [Location2] [varchar](40) NOT NULL DEFAULT (''),
  [Location3] [varchar](40) NOT NULL DEFAULT (''),
  [Location4] [varchar](40) NOT NULL DEFAULT (''),
  [VirtualItem] [bit] NOT NULL DEFAULT ('FALSE'),
  [ColorID] [varchar](150) NULL,
  [ModelComment] [varchar](40) NOT NULL DEFAULT (''),
  [ModelActive] [bit] NOT NULL DEFAULT ('TRUE'),
  [CreateDate] [datetime] NULL,
  [DeliveryDate] [datetime] NULL,
  [ShippingDate] [datetime] NULL,
  [ReceiveDate] [datetime] NULL,
  [WasSent] [bit] NOT NULL DEFAULT ('FALSE'),
  [Items] [nvarchar](100) NOT NULL DEFAULT (''),
  [ManufName] [nvarchar](100) NOT NULL DEFAULT (''),
  [CAMBlankID] [nvarchar](100) NOT NULL DEFAULT (''),
  [CAMBlankBatchID] [nvarchar](100) NOT NULL DEFAULT (''),
  [CAMJobID] [nvarchar](100) NOT NULL DEFAULT (''),
  [CAMJobName] [nvarchar](100) NOT NULL DEFAULT (''),
  [CAMErrorDescription] [nvarchar](100) NOT NULL DEFAULT (''),
  [CheckSum] [timestamp] NOT NULL,
  [ERPItemNo] [nvarchar](100) NOT NULL DEFAULT (''),
  [CacheMaterialName] [nvarchar](100) NOT NULL DEFAULT (''),
  [AltProcessStatusID] [varchar](150) NOT NULL DEFAULT (''),
  [CacheColor] [nvarchar](100) NOT NULL DEFAULT (''),
  [ModelElementType] [varchar](150) NOT NULL DEFAULT ('meIndicationRegular'),
  [ValidationResult] [varchar](150) NOT NULL DEFAULT (''),

  CONSTRAINT PK_processedHistoryTBL PRIMARY KEY CLUSTERED (HistoryID)
)

GO

SET ANSI_PADDING OFF
GO