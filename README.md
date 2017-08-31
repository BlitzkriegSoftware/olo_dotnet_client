# olo_dotnet_client
A C# .NET Client for Leveraging the OLO.com ordering API

# Problem statement

The OLO develop's guide at https://developer.olo.com/ gives some nice examples for leveraging their API, exposed nicely with SWAGGER. 

Using IP Address Restrictions and a APIKEY works ok, but is deprecated. 

Instead they would like API consumers to use a custom Authorization header, but this proved to be hard to implement in C#. After some experimentation the team came up with this little C# client that seems to produce the correct authorization header and inter-operate correctly with the OLO API.

# What's in the code?

## An OLO API Client

A client you can use that correctly interacts with the OLO API using either the APIKEY method or Authorization header, see the XML code comments and in-line code comments for details.

## Unit test samples

The unit test samples:
* Exercise both constructors
* Get the list of waiting batches
* Gets the batch detail as a zip
* Deletes a batch from the list

# How to compile it

We used Visual Studio 2017 v3, so load the solution up in that, and compile it.

Change the settings to your specific:
* API Key
* Client Id
* Client Secret 
* Download Folder

to the `app.config` in the unit test project to run the tests

# Contributors

* Stuart Williams <StuartW@Magenic.com>
* Jamin Bontrager <Jamin.Bontrager@famousdaves.com>
* Ramona Maxwell <RamonaM@magenic.com>

