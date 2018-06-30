USE [Dentalmanager]
GO
 
 /* Triggers for update: */
 
Create TRIGGER [dbo].[tr_insert_order_status_joined] ON [dbo].[ModelElement]
FOR UPDATE 
AS
BEGIN

DECLARE @ModelElementID varchar(150), @ModelJobID varchar(150), @MaterialID nvarchar(150), @OrderID varchar(150),
      @ProcessStatusID varchar(150), @ProcessLockID varchar(150), @ManufacturerID varchar(150), @ManufacturingProcessID varchar(150),
      @ModelTransformation varchar(2000), @ModelHeight real, @ModelFilename nvarchar(1000), @ModelVolume real, @ModelBoundingBoxMin varchar(200),
      @ModelBoundingBoxMax varchar(200), @Location1 varchar(40), @Location2 varchar(40), @Location3 varchar(40), @Location4 varchar(40), @VirtualItem bit,
      @ColorID varchar(150), @ModelComment varchar(40), @ModelActive bit, @CreateDate datetime, @DeliveryDate datetime, @ShippingDate datetime,
      @ReceiveDate datetime, @WasSent bit, @Items nvarchar(100), @ManufName nvarchar(100), @CAMBlankID nvarchar(100), @CAMBlankBatchID nvarchar(100),
      @CAMJobID nvarchar(100), @CAMJobName nvarchar(100), @CAMErrorDescription nvarchar(100), @ERPItemNo nvarchar(100), @CacheMaterialName nvarchar(100),
      @AltProcessStatusID varchar(150), @CacheColor nvarchar(100), @ModelElementType varchar(150), @ValidationResult varchar(150)


  SET @ModelElementID = (SELECT ModelElementID FROM inserted)
  SET @ModelJobID = (SELECT ModelJobID FROM inserted)
  SET @MaterialID = (SELECT MaterialID FROM inserted)
  SET @OrderID = (SELECT OrderID FROM dbo.Modeljob WHERE ModelJobID = @ModelJobID)
  SET @ProcessStatusID = (SELECT ProcessStatusID FROM inserted)
  SET @ProcessLockID = (SELECT ProcessLockID FROM inserted)
  SET @ManufacturerID = (SELECT ManufacturerID FROM inserted)
  SET @ManufacturingProcessID = (SELECT ManufacturingProcessID FROM inserted)
  SET @ModelTransformation = (SELECT ModelTransformation FROM inserted)
  SET @ModelHeight = (SELECT ModelHeight FROM inserted)
  SET @ModelFilename = (SELECT ModelFilename FROM inserted)
  SET @ModelVolume = (SELECT ModelVolume FROM inserted)
  SET @ModelBoundingBoxMin = (SELECT ModelBoundingBoxMin FROM inserted)
  SET @ModelBoundingBoxMax = (SELECT ModelBoundingBoxMax FROM inserted)
  SET @Location1 = (SELECT Location1 FROM inserted)
  SET @Location2 = (SELECT Location2 FROM inserted)
  SET @Location3 = (SELECT Location3 FROM inserted)
  SET @Location4 = (SELECT Location4 FROM inserted)
  SET @VirtualItem = (SELECT VirtualItem FROM inserted)
  SET @ColorID = (SELECT ColorID FROM inserted)
  SET @ModelComment = (SELECT ModelComment FROM inserted)
  SET @ModelActive = (SELECT ModelActive FROM inserted)
  SET @CreateDate = (SELECT CreateDate FROM inserted)
  SET @DeliveryDate = (SELECT DeliveryDate FROM inserted)
  SET @ShippingDate = (SELECT ShippingDate FROM inserted)
  SET @ReceiveDate = (SELECT ReceiveDate FROM inserted)
  SET @WasSent = (SELECT WasSent FROM inserted)
  SET @Items = (SELECT Items FROM inserted)
  SET @ManufName = (SELECT ManufName FROM inserted)
  SET @CAMBlankID = (SELECT CAMBlankID FROM inserted)
  SET @CAMBlankBatchID = (SELECT CAMBlankBatchID FROM inserted)
  SET @CAMJobID = (SELECT CAMJobID FROM inserted)
  SET @CAMJobName = (SELECT CAMJobName FROM inserted)
  SET @CAMErrorDescription = (SELECT CAMErrorDescription FROM inserted)
  SET @ERPItemNo = (SELECT ERPItemNo FROM inserted)
  SET @CacheMaterialName = (SELECT CacheMaterialName FROM inserted)
  SET @AltProcessStatusID = (SELECT AltProcessStatusID FROM inserted)
  SET @CacheColor = (SELECT CacheColor FROM inserted)
  SET @ModelElementType = (SELECT ModelElementType FROM inserted)
  SET @ValidationResult = (SELECT ValidationResult FROM inserted)



INSERT INTO [dbo].[tbl_processedHistory]
           ([ModelElementID]
           ,[ModelJobID]
           ,[MaterialID]
           ,[OrderID] /* Obtained from joining [dbo].[ModelElement] with dbo.Modeljob*/
           ,[ProcessStatusID]
           ,[ProcessLockID]
           ,[ManufacturerID]
           ,[ManufacturingProcessID]
           ,[ModelTransformation]
           ,[ModelHeight]
           ,[ModelFilename]
           ,[ModelVolume]
           ,[ModelBoundingBoxMin]
           ,[ModelBoundingBoxMax]
           ,[Location1]
           ,[Location2]
           ,[Location3]
           ,[Location4]
           ,[VirtualItem]
           ,[ColorID]
           ,[ModelComment]
           ,[ModelActive]
           ,[CreateDate]
           ,[DeliveryDate]
           ,[ShippingDate]
           ,[ReceiveDate]
           ,[WasSent]
           ,[Items]
           ,[ManufName]
           ,[CAMBlankID]
           ,[CAMBlankBatchID]
           ,[CAMJobID]
           ,[CAMJobName]
           ,[CAMErrorDescription]
           ,[ERPItemNo]
           ,[CacheMaterialName]
           ,[AltProcessStatusID]
           ,[CacheColor]
           ,[ModelElementType]
           ,[ValidationResult])
     VALUES
           (@ModelElementID, @ModelJobID, @MaterialID, @OrderID,
      @ProcessStatusID, @ProcessLockID, @ManufacturerID, @ManufacturingProcessID,
      @ModelTransformation, @ModelHeight, @ModelFilename, @ModelVolume, @ModelBoundingBoxMin,
      @ModelBoundingBoxMax, @Location1, @Location2, @Location3, @Location4, @VirtualItem,
      @ColorID, @ModelComment, @ModelActive, @CreateDate, @DeliveryDate, @ShippingDate,
      @ReceiveDate, @WasSent, @Items, @ManufName, @CAMBlankID, @CAMBlankBatchID,
      @CAMJobID, @CAMJobName, @CAMErrorDescription, @ERPItemNo, @CacheMaterialName,
      @AltProcessStatusID, @CacheColor, @ModelElementType, @ValidationResult)

END

