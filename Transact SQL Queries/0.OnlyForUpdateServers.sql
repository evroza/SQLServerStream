drop trigger [dbo].[tr_insert_order_status_joined]
drop trigger [dbo].[tr_update_order_status_joined]

GO

ALTER TABLE [dbo].[tbl_processedHistory] ADD [UnitsCounted] int NULL

GO

/*
	Next you need to re-run the updated sql scripts from 1-3

*/