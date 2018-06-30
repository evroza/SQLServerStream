# 3Shape Dental Manager - SQLServerStream BI
A Windows Service to listen for table changes on SQL Server and transmit to a listening Web server. Includes local logging

Any order operations inside 3Shape Dental Manager will be captured by this service and the modified records are transmitted to listening web server. 
### Purpose
This service is supposed to feed a separate Business Intelligence System by immediately transmitting any order modification operations inside 3Shape Dental Manager to the web server.

The Web server is using expiring tokens to authenticate connections, so this service will manage credentials and send datasets with valid credentials.


## Version 1.10 Production Release

Features:
- Listen for inserts to SQL Server table
- Login and securely transmit records to file server using tokens
- Failed Transmits are logged locally for later transmission at specified intervals
- DB Records that may have been skipped or failed to locally logged are also re-transmitted at given interval
- Successful transmit triggers DB update which sets flag to indicate record has been successfully transmitted


## Configuration
Configurations can be made inside the App.config file found inside the service install directory.
A summary of parameters that can be configured using this file include:
- All the connection parameters and credentials to the Web Server and SQL Server
- Logging locations for both Failed Transmissions and Crash Logs
- Retransmission intervals for failed trasmissions

#NOTE: You currently cannot start the service if the web server is unreachable, a login to web server has to be performed before the service can continue