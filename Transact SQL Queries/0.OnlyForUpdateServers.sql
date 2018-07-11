drop trigger [dbo].[tr_insert_order_status_joined]
drop trigger [dbo].[tr_update_order_status_joined]

GO

ALTER TABLE [dbo].[tbl_processedHistory] ADD [UnitsCounted] int NULL

GO

/*
	Next you need to re-run the updated sql scripts from 2 & 3

	!! Do not run this script if the server is 'fresh' and hasn't been installed with the previosu release of this software

*/