
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 01/18/2013 23:59:21
-- Generated from EDMX file: G:\SkyDrive\project Code\ShowRoomSys.Service\ShowRommSys.Client.Web\Models\ShowRoomSysDBModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ShowRoomSys];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[ShowItemsList]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ShowItemsList];
GO
IF OBJECT_ID(N'[dbo].[Items]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Items];
GO
IF OBJECT_ID(N'[dbo].[ItemsPackage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ItemsPackage];
GO
IF OBJECT_ID(N'[dbo].[Resources]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Resources];
GO
IF OBJECT_ID(N'[dbo].[ShowRoom]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ShowRoom];
GO
IF OBJECT_ID(N'[dbo].[Customer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Customer];
GO
IF OBJECT_ID(N'[dbo].[CompanyNatures]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CompanyNatures];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[Rights]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Rights];
GO
IF OBJECT_ID(N'[dbo].[UsersRights]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UsersRights];
GO
IF OBJECT_ID(N'[dbo].[MenuItem]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MenuItem];
GO
IF OBJECT_ID(N'[dbo].[Device]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Device];
GO
IF OBJECT_ID(N'[dbo].[ItemsResources]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ItemsResources];
GO
IF OBJECT_ID(N'[dbo].[ListResources]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ListResources];
GO
IF OBJECT_ID(N'[dbo].[PackageItems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PackageItems];
GO
IF OBJECT_ID(N'[dbo].[Natures]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Natures];
GO
IF OBJECT_ID(N'[dbo].[NaturesItemsSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NaturesItemsSet];
GO
IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roles];
GO
IF OBJECT_ID(N'[dbo].[RolesRights]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RolesRights];
GO
IF OBJECT_ID(N'[dbo].[UserLogSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserLogSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'ShowItemsList'
CREATE TABLE [dbo].[ShowItemsList] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [CreateMethod] nvarchar(max)  NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [CreateTime] datetime  NOT NULL,
    [Approve] bit  NOT NULL
);
GO

-- Creating table 'Items'
CREATE TABLE [dbo].[Items] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Remark] nvarchar(max)  NOT NULL,
    [CreateTime] datetime  NOT NULL,
    [ListId] int  NOT NULL
);
GO

-- Creating table 'ItemsPackage'
CREATE TABLE [dbo].[ItemsPackage] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Remark] nvarchar(max)  NOT NULL,
    [CreateTime] datetime  NOT NULL
);
GO

-- Creating table 'Resources'
CREATE TABLE [dbo].[Resources] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] int  NOT NULL,
    [Uri] nvarchar(max)  NOT NULL,
    [Category] nvarchar(max)  NULL,
    [Approve] bit  NOT NULL,
    [Remark] nvarchar(max)  NOT NULL,
    [CreateTime] datetime  NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ShowRoom'
CREATE TABLE [dbo].[ShowRoom] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [City] nvarchar(max)  NOT NULL,
    [Address] nvarchar(max)  NOT NULL,
    [Principal] nvarchar(max)  NOT NULL,
    [PrincipalTel] nvarchar(max)  NOT NULL,
    [PrincipalEmail] nvarchar(max)  NOT NULL,
    [Manager] nvarchar(max)  NOT NULL,
    [ManagerTel] nvarchar(max)  NOT NULL,
    [ManageEmail] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [Picture] nvarchar(max)  NOT NULL,
    [InterfaceUri] nvarchar(max)  NOT NULL,
    [States] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Customer'
CREATE TABLE [dbo].[Customer] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Address] nvarchar(max)  NOT NULL,
    [Contact] nvarchar(max)  NOT NULL,
    [Tel] nvarchar(max)  NOT NULL,
    [Trade] nvarchar(max)  NOT NULL,
    [NatureId] int  NOT NULL,
    [Grade] int  NOT NULL,
    [Business] nvarchar(max)  NOT NULL,
    [OrderTime] datetime  NULL
);
GO

-- Creating table 'CompanyNatures'
CREATE TABLE [dbo].[CompanyNatures] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Nature] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [Number] nvarchar(max)  NOT NULL,
    [Tel] nvarchar(max)  NULL,
    [Email] nvarchar(max)  NULL,
    [Remark] nvarchar(max)  NULL,
    [RoleId] int  NOT NULL,
    [LastLoginTime] datetime  NULL
);
GO

-- Creating table 'Rights'
CREATE TABLE [dbo].[Rights] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Code] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'UsersRights'
CREATE TABLE [dbo].[UsersRights] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int  NOT NULL,
    [RightId] int  NOT NULL
);
GO

-- Creating table 'MenuItem'
CREATE TABLE [dbo].[MenuItem] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [PId] int  NOT NULL,
    [PName] nvarchar(max)  NOT NULL,
    [Level] int  NOT NULL,
    [URL] nvarchar(max)  NOT NULL,
    [Index] int  NOT NULL,
    [Visible] bit  NOT NULL,
    [IconStyle] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Device'
CREATE TABLE [dbo].[Device] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [MAC] nvarchar(max)  NOT NULL,
    [Remark] nvarchar(max)  NOT NULL,
    [Status] nvarchar(max)  NULL
);
GO

-- Creating table 'ItemsResources'
CREATE TABLE [dbo].[ItemsResources] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ItemId] int  NOT NULL,
    [ResourceId] int  NOT NULL,
    [DeviceId] int  NOT NULL
);
GO

-- Creating table 'ListResources'
CREATE TABLE [dbo].[ListResources] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ListID] int  NOT NULL,
    [ResourceID] int  NOT NULL
);
GO

-- Creating table 'PackageItems'
CREATE TABLE [dbo].[PackageItems] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PackageId] int  NOT NULL,
    [ItemId] int  NOT NULL,
    [ListId] int  NOT NULL
);
GO

-- Creating table 'Natures'
CREATE TABLE [dbo].[Natures] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Date] datetime  NOT NULL,
    [Time] int  NOT NULL,
    [Custmor] nvarchar(max)  NULL,
    [Contact] nvarchar(max)  NULL,
    [Tel] nvarchar(max)  NULL,
    [Remark] nvarchar(max)  NULL,
    [Status] nvarchar(max)  NOT NULL,
    [Feedback] nvarchar(max)  NULL,
    [CustmorCount] int  NOT NULL,
    [CustmorLevel] nvarchar(max)  NOT NULL,
    [Escort] nvarchar(max)  NOT NULL,
    [Follow] nvarchar(max)  NULL
);
GO

-- Creating table 'NaturesItemsSet'
CREATE TABLE [dbo].[NaturesItemsSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ItemId] int  NOT NULL,
    [ListId] int  NOT NULL,
    [NaId] int  NOT NULL,
    [FollowLevel] nvarchar(max)  NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'RolesRights'
CREATE TABLE [dbo].[RolesRights] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RightId] int  NOT NULL,
    [RoleId] int  NOT NULL
);
GO

-- Creating table 'UserLogSet'
CREATE TABLE [dbo].[UserLogSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int  NOT NULL,
    [LoginTime] datetime  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'ShowItemsList'
ALTER TABLE [dbo].[ShowItemsList]
ADD CONSTRAINT [PK_ShowItemsList]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Items'
ALTER TABLE [dbo].[Items]
ADD CONSTRAINT [PK_Items]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ItemsPackage'
ALTER TABLE [dbo].[ItemsPackage]
ADD CONSTRAINT [PK_ItemsPackage]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Resources'
ALTER TABLE [dbo].[Resources]
ADD CONSTRAINT [PK_Resources]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ShowRoom'
ALTER TABLE [dbo].[ShowRoom]
ADD CONSTRAINT [PK_ShowRoom]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Customer'
ALTER TABLE [dbo].[Customer]
ADD CONSTRAINT [PK_Customer]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CompanyNatures'
ALTER TABLE [dbo].[CompanyNatures]
ADD CONSTRAINT [PK_CompanyNatures]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Rights'
ALTER TABLE [dbo].[Rights]
ADD CONSTRAINT [PK_Rights]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UsersRights'
ALTER TABLE [dbo].[UsersRights]
ADD CONSTRAINT [PK_UsersRights]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MenuItem'
ALTER TABLE [dbo].[MenuItem]
ADD CONSTRAINT [PK_MenuItem]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Device'
ALTER TABLE [dbo].[Device]
ADD CONSTRAINT [PK_Device]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ItemsResources'
ALTER TABLE [dbo].[ItemsResources]
ADD CONSTRAINT [PK_ItemsResources]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ListResources'
ALTER TABLE [dbo].[ListResources]
ADD CONSTRAINT [PK_ListResources]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PackageItems'
ALTER TABLE [dbo].[PackageItems]
ADD CONSTRAINT [PK_PackageItems]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Natures'
ALTER TABLE [dbo].[Natures]
ADD CONSTRAINT [PK_Natures]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'NaturesItemsSet'
ALTER TABLE [dbo].[NaturesItemsSet]
ADD CONSTRAINT [PK_NaturesItemsSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RolesRights'
ALTER TABLE [dbo].[RolesRights]
ADD CONSTRAINT [PK_RolesRights]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserLogSet'
ALTER TABLE [dbo].[UserLogSet]
ADD CONSTRAINT [PK_UserLogSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------