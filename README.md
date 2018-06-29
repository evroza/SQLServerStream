# SQLServerStream
A Windows Service to listen for table changes on SQL Server and transmit to a listening Web server. Includes local logging


## Version 1.00 Release

Features:
- Listen for inserts to SQL Server table
- Login and securely transmit records to file server using tokens
- Failed Transmits are logged locally for later transmission at specified intervals
- DB Records that may have been skipped or failed to locally logged are also re-transmitted at given interval
- Successful transmit triggers DB update which sets flag to indicate record has been successfully transmitted
