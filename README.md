# Kerppi - a lightweight small business ERP software

Kerppi, or "kevyt erppi" - Finnish slang for "light ERP", is a lightweight ERP software base for small businesses. It includes simple customer and stock keeping unit management and printing support. Originally there were more features regarding SKU management and especially the main workflow, but for this GPL version they are removed as it was mostly customer specific stuff, so this is pretty much just a basis for custom applications.

Kerppi uses the MVVM model and Dapper with SimpleCRUD extensions. It is not asynchronous, though, as it uses System.Data.SQLite for database with encryption so normally there's really no waiting time anyway. Although the encryption feature of the SQLite lib seems to be sort of a hack, it works. Other code features include your usual visual tree walker and time conversion classes along with a bunch of converters.

The software is written in C# using Visual Studio Express 2013 for Windows Desktop. It is GPL-3.0 licensed.

## Dependencies

* System.Data.SQLite (included)
* Dapper (included)
* Dapper.SimpleCRUD (included)
* .NET Framework 4.5
* Visual C++ Redistributable for Visual Studio 2012 Update 4
