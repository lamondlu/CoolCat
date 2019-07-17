CREATE TABLE [dbo].[Plugins]
(
	[PluginId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [UniqueKey] NVARCHAR(50) NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [DisplayName] NVARCHAR(MAX) NULL, 
    [Version] NVARCHAR(50) NOT NULL, 
    [DllPath] NVARCHAR(MAX) NULL, 
    [ViewPath] NVARCHAR(MAX) NULL
)
