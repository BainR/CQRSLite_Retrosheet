# CQRS with Retrosheet

This program loads Retrosheet event data into various repositories using the CQRS/ES pattern.

## Why I Did This
- [To Learn CQRS](Education.md)
- [But not to load Retrosheet data](BetterOptions.md)

## Prerequisites

DotNet Core 2, Sql Server, Redis (optional)

## Getting Started

This application can run on Windows 10, some versions of Linux including Ubuntu, and perhaps on a Mac.  The following steps will help you get started.

* Install DotNet Core2, RabbitMQ, Redis (optional), and Sql Server.
* Create a database in Sql Server or select and existing one.  Then run the CreateTablesForMSSQL.sql script found in CQRSLite_Retrosheet.Domain/ReadModel/Repositories/CreateTableSql.
* Edit nlog.config in CQRSLite_Retrosheet.Web.  At a minimum, you will need to update the connection string to point to your own database and server, or edit the file more radically to log to something other than Sql Server.
* Edit appsettings.json in CQRSLite_Retrosheet.Web.  Any one of the 3 repository types should be uncommented.  The connection strings will need to be edited to match your system.
* Edit appsetting.json in CQRSLite_Retrosheet.LoadGames.  My preferred option is to download zip files from the Retrosheet website to the local file system and then configure appsetting.json to use the local copy of the zip file.  You can further restrict the data by extracting a subset of the files from the zip file into a local folder and using the folder as the data source.  The final alternative is to use the url of the zip file directly from the Retrosheet website.
* Visual Studio creates a launchsettings.json file in the Properties folder that is incompatible with the rest of my code.  If you get this file, you will need to edit the port number to 57285 and optionally set launchBrowser to false.

### Running the Application

This application can be run like any other DotNet Core application.  I have run it in debug mode on Windows 10 using Visual Studio and on Linux using Visual Studio Code.  The release version of the code can be published and run using the dotnet runtime environment.

First, run LoadGames together with the Web component to enter the data into MSSQL or Redis.  Then the Web component can be run by itself to preview the data.  The QueryController.cs file in CQRSLite_Retrosheet.Web/Controllers has several example urls relating to the 1991 World Series.

### RabbitMQ Branch

This repository contains a RabbitMQ branch.  That branch replaces the default Router from CQRSLite with one leveraging more advanced messaging from RabbitMQ.  Although adding complexity to both the program and the program setup, the branch solves the challenge of waiting for multiple requests to trigger a command handler.  If you got this far, please take a look at the RabbitMQ branch too.

### Future Enhancements

This project met my objective of guiding me on a journey to gain hands on experience with the CQRS principles.  However, it falls short of being a practical application.  To help this code gain a wider audience, it would be almost trivial to enable it to save data to MySQL, SQLite, PostgreSQL, and to comma or tab separated text files.  A slightly more difficult challenge would be to include all fields from the Retrosheet utility in the ReadModel from this utility.  A much more difficult challenge would be to increase the performance to the point anyone would want to use this to load more than 12 million rows of data.  Bulk load operations are much more efficient.

## Author

Richard Bain

## Acknowledgements
- [Exception Not Found - Part 1](https://www.exceptionnotfound.net/real-world-cqrs-es-with-asp-net-and-redis-part-1-overview/)
- [Exception Not Found - Part 2](https://www.exceptionnotfound.net/real-world-cqrs-es-with-asp-net-and-redis-part-2-the-write-model/)
- [Exception Not Found - Part 3](https://www.exceptionnotfound.net/real-world-cqrs-es-with-asp-net-and-redis-part-3-the-read-model/)
- [Exception Not Found - Part 4](https://www.exceptionnotfound.net/real-world-cqrs-es-with-asp-net-and-redis-part-4-creating-the-apis/)
- [Exception Not Found - Part 5](https://www.exceptionnotfound.net/real-world-cqrs-es-with-asp-net-and-redis-part-5-running-the-apis/)
- [Retrosheet homepage](http://www.retrosheet.org/)
- [Retrosheet Event Files](http://www.retrosheet.org/game.htm)
- [Retrosheet Event File Format](http://www.retrosheet.org/eventfile.htm)

##### Use of Retrosheet Data

[The information used here was obtained free of charge from and is copyrighted by Retrosheet.  Interested parties may contact Retrosheet at "www.retrosheet.org".](http://www.retrosheet.org/notice.txt)

##### Word of Thanks

I have tried hard to find naturally occurring examples in the Retrosheet data to test my Validation code.  David Smith and the contributors to the Retrosheet website have worked even harder to make sure these examples don't exist, at least not for very long.
