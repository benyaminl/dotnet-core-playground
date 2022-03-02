#!/bin/bash
# Wait to be sure that SQL Server came up
sleep 60

# Run the setup script to create the DB and the schema in the DB
# Note: make sure that your password matches what is in the Dockerfile
c=$(/opt/mssql-tools/bin/sqlcmd -U sa -P 12345\!@Local -d master -h -1 -Q "set nocount on;SELECT count(name) FROM master.dbo.sysdatabases WHERE name = 'coba'" -W)
if [ $c -eq 0 ]
then
    echo "Creating Database..."
    /opt/mssql-tools/bin/sqlcmd -U sa -P 12345!@Local -d master -i create-database.sql
    echo "Database and Table created"
else 
    echo "Database Exists... no need to recreate DB"
fi 
