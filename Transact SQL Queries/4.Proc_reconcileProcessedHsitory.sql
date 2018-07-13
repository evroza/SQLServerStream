USE [DentalManager]
GO
/****** Object:  StoredProcedure [dbo].[Proc_reconcileProcessedHistory]    Script Date: 7/14/2018 3:26:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
	* Proc_reconcileProcessedHistory Procedure 
	*
	* Version 1.0.0
	* Author Evans Rutoh
	* Creation 13/8/2018
	* Updated 14/08/2018
	*
	* It reconciles the UnitsCounted in the tbl_processedHistory table vs the actual recorded
	* units inside the dbo.ToothElement table for a given ModelElementID.
**/

ALTER PROCEDURE [dbo].[Proc_reconcileProcessedHistory] 
(
	@HistoryID VARCHAR(20)
)
   AS
   BEGIN
    SET NOCOUNT ON;

	DECLARE @UnitsCounted varchar(20),
	@HistoryIDLocal int,
	@ModelElementID varchar(50)
     
	SELECT @HistoryIDLocal = tbl_processedHistory.HistoryID
	, @ModelElementID = tbl_processedHistory.ModelElementID
	, @UnitsCounted = COUNT(ToothElement.ModelElementID)
	FROM dbo.tbl_processedHistory 
	LEFT JOIN dbo.ToothElement
	ON ToothElement.ModelElementID = tbl_processedHistory.ModelElementID
	WHERE tbl_processedHistory.HistoryID = @HistoryID
	GROUP BY tbl_processedHistory.ModelElementID, tbl_processedHistory.HistoryID;

	/**
		* Next, we need to update the processedHistory table with the 
		* correct UnitsCounted (number of teeth) for that order
	**/

	UPDATE dbo.tbl_processedHistory SET UnitsCounted = @UnitsCounted WHERE HistoryID = @HistoryID


   END
