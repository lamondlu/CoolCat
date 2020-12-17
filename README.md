![](./doc/images/logo_small.png)

# CoolCat
A sample about how to create a dynamic plugins mechanism with ASP.NET Core Mvc based on the AssemblyLoadContext.

This whole project is built under .NET Core 3.1 and .NET 5. 

## What i will do and what i will not do
I want to build a runtime plugin mechanism based on .NET Core 3.1 and .NET 5. Each plugin will be isolated by a custom AssemlyLoadContext. So the framework allow you to reference same library with different version.

![](./doc/images/load_way.png)

## Getting Started
 - Clone the source code
 - Run `docker-compose up`
 - Install the pre-set modules
![](./doc/images/20200726215825.png)
 - Start to use the system 

## How to create and publish a plugin
 - Run `dotnet new -i CoolCatModule`, it will install the CoolCatModule on your machine
 - Run `dotnet new CoolCatModule -n {your plugin name}`
 - Build the plugin with VisualStudio 2019 or `dotnet publish`
 - Package the release files into a zip package

