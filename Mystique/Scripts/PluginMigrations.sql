CREATE TABLE [dbo].[PluginMigrations]
(
    [PluginMigrationId] UNIQUEIDENTIFIER NOT NULL, 
	[PluginId] UNIQUEIDENTIFIER NOT NULL , 
    [Version] NVARCHAR(50) NOT NULL, 
    [Up] NTEXT NOT NULL, 
    [Down] NTEXT NULL
    PRIMARY KEY ([PluginMigrationId]) 
)

GO

ALTER TABLE [dbo].[PluginMigrations]
ADD CONSTRAINT FK_PluginMigrations_Plugins FOREIGN KEY(PluginId) REFERENCES [dbo].[Plugins](PluginId)

GO