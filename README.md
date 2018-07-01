# 3Shape Dental Manager - SQLServerStream BI
A Windows Service to listen for table changes on SQL Server and transmit to a listening Web server. Includes local logging

Any order operations inside 3Shape Dental Manager will be captured by this service and the modified records are transmitted to listening web server. 
### Purpose
This service is supposed to feed a separate Business Intelligence System by immediately transmitting any order modification operations inside 3Shape Dental Manager to the web server.

The Web server is using expiring tokens to authenticate connections, so this service will manage credentials and send datasets with valid tokens.


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

Configuration Sample:
    <code>
    
    &lt;?xml version="1.0" encoding="utf-8" ?&gt;
&lt;configuration&gt;
  &lt;!-- 
    IMPORTANT!! - Incorrectly configuring or formatting this config file will cause the service to fail to start
    ONLY touch these configs if you know what you are doing
    
    Read the comments next to the important configurations to learn about their valid values
    
    NOTE - If you change the SQL Server connection string below, make sure to also update the 'DBServerType'
            configuration option further down in the config file, to correctly indicate whether it's a local or remote
            server. YOU CAN ONLY USE ONE! IGNORE THE ONE YOU ARE NOT USING
  --&gt;
  &lt;connectionStrings&gt;
      &lt;!-- 
          For local SQL server connection, use 'DBServerLocal' setting below,
          make sure 'connectionString' correctly points to your remote instance
      --&gt;
      &lt;add name="DBServerLocal"
           connectionString="data source=.; initial catalog=DentalManager; integrated security=True"
           providerName="System.Data.SqlClient"/&gt;
    
      &lt;!-- 
          For remote SQL server connections, use 'DBServerRemote' setting below,
          make sure 'connectionString' correctly points to your remote instance
      --&gt;
      &lt;add name="DBServerRemote"
           connectionString="Server=172.16.204.100\THREESHAPEDENTAL;Database=DentalManager;User Id=sa;Password=3SDMdbmspw;"
           providerName="System.Data.SqlClient"/&gt;
  &lt;/connectionStrings&gt;

  &lt;appSettings&gt;
    &lt;!-- The SQL server table name where our history records are being dumped --&gt;
    &lt;add key="DBTableName" value="tbl_processedHistory"/&gt;
    &lt;!-- 
        DBServerType can either be 'remote' or 'local'. This will make service pick between the 
        two types of DB connection strings in the  'connectionStrings' group element above
    --&gt;
    &lt;add key="DBServerType" value="remote"/&gt;
    
    &lt;!-- Web Server connection options --&gt;
    &lt;add key="WebServerAddress" value="http://localhost:3000"/&gt;
    &lt;add key="WebServerUsername" value="mimizaana"/&gt;
    &lt;add key="WebServerPassword" value="wewe"/&gt;
    
    &lt;!-- Logging Folders. Don't delete these, but you can change their value to a valid folder name --&gt;
    &lt;add key="LogFolder" value="FailedTransmits"/&gt;
    &lt;add key="CrashLogFolder" value="CrashLogs"/&gt;

    &lt;!-- 
      How often, in SECONDS the service will try to retransmit failed transmissions, 
      these failed transmits include locally logged failed, due to web server connectivity problems
      and those records whose insert|update events may have been missed by the service in the first place,
      maybe because the service was paused|stopped|crashed when they were inserted
      
      Default has been configured to 10 minutes i.e 60*10 == 600 seconds
    --&gt;
    &lt;add key="FailedRetransmitIntervalSeconds" value="600"/&gt;
    
  &lt;/appSettings&gt;
  
&lt;/configuration&gt;
    
    </code>