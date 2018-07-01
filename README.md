# 3Shape Dental Manager - SQLServerStream BI Export
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

# Installation
## Pre-requisites
- The computer on which this service is to run must be able to reach (ping) both the SQL server instance as well as the Web server where records will be transmitted to. Make sure there's no firewall blocking connections and the web server and SQL server ports are open
- Ensure the listening Web server is running before you attempt to start this service. The web server is where payloads captured by this service will be forwarded to
- m

### First part(Installation)
1. You MUST first execute this query in SQL server to enable Service broker for the 3Shape Dental Manager database
            ```sql 
              ALTER DATABASE DentalManager SET ENABLE_BROKER 
            ```
  If you get errors running the query check your db user permissions
2. This service integrates with 3Shape Dental Manager to export order modification actions inside 3Shape Dental Manager to a third-party BI system. To make it work You must first run the Attached SQL scripts inside the **Transact SQL Queries** directory. These scripts should be executed in order to void any unexpected errors. The SQL scripts have been numbered to indicate order of execution

### Second part(Installation)

After running the above queries, you are ready to start running the service

1. After compiling and obtaining the MSI deployment package, copy it to the computer on which you need it to run from
2. Run the Installer, this will automatically start the service on completion. The Service name is "SQL Server Notify Stream Service"
3. Inside the service install directory, find the configuration file **SQLServerNotifyStream.exe.config**
4. Configure it to match your preferences. Configuration of this file is in the next section below

## Configuration
Configurations can be made inside the **App.config** during development. After deplyoment instead use the **SQLServerNotifyStream.exe.config** file found inside the service install directory.
A summary of parameters that can be configured using this file include:
- All the connection parameters and credentials to the Web Server and SQL Server
- Logging locations for both Failed Transmissions and Crash Logs
- Retransmission intervals for failed trasmissions

> NOTE: You currently cannot start the service if the web server is unreachable, a login to web server has to be performed before the service can continue operation.

A configuration sample has been provided with accompanying comments for valid values. Tweak them to match your application. 
Configuration Sample:
    ```xml
      
      <?xml version="1.0" encoding="utf-8" ?>
      <configuration>
        <!--           
          NOTE - If you change the SQL Server connection string below, make sure to also update the 'DBServerType'
                  configuration option further down in the config file, to correctly indicate whether it's a local or remote
                  db server. YOU CAN ONLY USE ONE! IGNORE THE ONE NOT IN USE USING
        -->
        <connectionStrings>
            <!-- 
                For local SQL server connection, use 'DBServerLocal' setting below,
                make sure 'connectionString' correctly points to your remote instance
            -->
            <add name="DBServerLocal"
                 connectionString="data source=.; initial catalog=DentalManager; integrated security=True"
                 providerName="System.Data.SqlClient"/>
          
            <!-- 
                For remote SQL server connections, use 'DBServerRemote' setting below,
                make sure 'connectionString' correctly points to your remote instance
            -->
            <add name="DBServerRemote"
                 connectionString="Server=172.16.204.100\THREESHAPEDENTAL;Database=DentalManager;User Id=sa;Password=3SDMdbmspw;"
                 providerName="System.Data.SqlClient"/>
        </connectionStrings>

        <appSettings>
          <!-- 
              The SQL server table name where our history records are being dumped
              NOTE! These table must match the History dumping table created by the attached SQL scripts
          -->
          <add key="DBTableName" value="tbl_processedHistory"/>
          <!-- 
              DBServerType can either be 'remote' or 'local'. This will make service pick between the 
              two types of DB connection strings in the  'connectionStrings' group element above
          -->
          <add key="DBServerType" value="remote"/>
          
          <!-- 
              Web Server connection options
              Change the 'value' property to match your credentials and address. These are just dummy placeholders
          -->
          <add key="WebServerAddress" value="http://localhost:3000"/>
          <add key="WebServerUsername" value="mimizaana"/>
          <add key="WebServerPassword" value="wewe"/>
          
          <!-- Logging Folders. Don't delete these, but you can change their value to a valid folder name -->
          <add key="LogFolder" value="FailedTransmits"/>
          <add key="CrashLogFolder" value="CrashLogs"/>

          <!-- 
            How often, in SECONDS the service will try to retransmit failed transmissions, 
            these failed transmits include locally logged failed, due to web server connectivity problems
            and those records whose insert|update events may have been missed by the service in the first place,
            maybe because the service was paused|stopped|crashed when they were inserted
            
            Default has been configured to 10 minutes i.e 60*10 == 600 seconds
          -->
          <add key="FailedRetransmitIntervalSeconds" value="600"/>
          
        </appSettings>
        
      </configuration>    
  

    ```