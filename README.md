# OPC-Foundation-Labs-Server-Client-.Net-4.7.2

This solution contains a standard .NET 4.7.2 projects regarding the client-server layer to interact with OPC Foundation together with the database and model projects to store data.
The solution is divided in the next modules or assemblies:

## Client-server layer
* DB.ModelLib : Database interaction context (EntityFramework)
*   Hangfire.OPC.Configuration : Hangfire default Job configuration
*   Hangfire.OPC.JobLib : Job definitions managed by the Hangfire process
*   OPCFoundation.ClientLib : Opc client procedures and clases to interact with the SDK / Stack
*   OPCFoundation.ServerLib : Opc server procedures and clases to interact with the SDK / Stack 
*   OPCFoundation.TaskLib : Tasks managed by the jobs

## Database and model
*   Model.GenericUAServer : GenericUAServer DB model to store the Job process and related data  
*   SQL.GenericUAServer : Database to store node data

## Requirements

* Visual Studio 2022
* .NET Framework 4.5.1
* https://github.com/kraftcoding/OPC-Foundation-Labs-.Net-4.8
