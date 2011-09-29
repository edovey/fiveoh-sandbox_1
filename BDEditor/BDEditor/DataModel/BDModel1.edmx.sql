
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 09/29/2011 12:30:55
-- Generated from EDMX file: C:\Users\Liz Dovey\Documents\Git-SS\fiveoh-sandbox\BDEditor\BDEditor\DataModel\BDModel1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [BDData];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[BDCategories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BDCategories];
GO
IF OBJECT_ID(N'[dbo].[BDDiseases]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BDDiseases];
GO
IF OBJECT_ID(N'[dbo].[BDLinkedNotes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BDLinkedNotes];
GO
IF OBJECT_ID(N'[dbo].[BDPathogens]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BDPathogens];
GO
IF OBJECT_ID(N'[dbo].[BDPresentations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BDPresentations];
GO
IF OBJECT_ID(N'[dbo].[BDQueueEntries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BDQueueEntries];
GO
IF OBJECT_ID(N'[dbo].[BDSections]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BDSections];
GO
IF OBJECT_ID(N'[dbo].[BDSubcategories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BDSubcategories];
GO
IF OBJECT_ID(N'[dbo].[BDTherapies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BDTherapies];
GO
IF OBJECT_ID(N'[dbo].[BDTherapyGroups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BDTherapyGroups];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'BDCategories'
CREATE TABLE [dbo].[BDCategories] (
    [uuid] uniqueidentifier  NOT NULL,
    [sectionId] uniqueidentifier  NULL,
    [name] nvarchar(4000)  NULL,
    [createdBy] uniqueidentifier  NULL,
    [createdDate] datetime  NULL,
    [modifiedBy] uniqueidentifier  NULL,
    [modifiedDate] datetime  NULL,
    [schemaVersion] smallint  NULL,
    [deprecated] bit  NOT NULL,
    [inUseBy] uniqueidentifier  NULL,
    [displayOrder] smallint  NULL
);
GO

-- Creating table 'BDDiseases'
CREATE TABLE [dbo].[BDDiseases] (
    [uuid] uniqueidentifier  NOT NULL,
    [createdBy] uniqueidentifier  NULL,
    [createdDate] datetime  NULL,
    [deprecated] bit  NOT NULL,
    [inUseBy] nvarchar(4000)  NULL,
    [modifiedBy] uniqueidentifier  NULL,
    [modifiedDate] datetime  NULL,
    [name] nvarchar(4000)  NULL,
    [overview] nvarchar(4000)  NULL,
    [schemaVersion] smallint  NULL,
    [subcategoryId] uniqueidentifier  NULL,
    [categoryId] uniqueidentifier  NULL,
    [displayOrder] smallint  NULL
);
GO

-- Creating table 'BDLinkedNotes'
CREATE TABLE [dbo].[BDLinkedNotes] (
    [uuid] uniqueidentifier  NOT NULL,
    [createdBy] uniqueidentifier  NULL,
    [createdDate] datetime  NULL,
    [modifiedBy] uniqueidentifier  NULL,
    [modifiedDate] datetime  NULL,
    [schemaVersion] smallint  NULL,
    [deprecated] bit  NOT NULL,
    [inUseBy] uniqueidentifier  NULL,
    [documentText] nvarchar(4000)  NULL,
    [storageKey] nvarchar(4000)  NULL,
    [contextPropertyName] nvarchar(4000)  NULL,
    [parentId] uniqueidentifier  NULL,
    [displayOrder] smallint  NULL
);
GO

-- Creating table 'BDPathogens'
CREATE TABLE [dbo].[BDPathogens] (
    [uuid] uniqueidentifier  NOT NULL,
    [createdBy] uniqueidentifier  NULL,
    [createdDate] datetime  NULL,
    [deprecated] bit  NOT NULL,
    [inUseBy] nvarchar(4000)  NULL,
    [modifiedBy] uniqueidentifier  NULL,
    [modifiedDate] datetime  NULL,
    [name] nvarchar(4000)  NULL,
    [presentationId] uniqueidentifier  NULL,
    [schemaVersion] smallint  NULL,
    [displayOrder] smallint  NULL
);
GO

-- Creating table 'BDPresentations'
CREATE TABLE [dbo].[BDPresentations] (
    [uuid] uniqueidentifier  NOT NULL,
    [diseaseId] uniqueidentifier  NULL,
    [createdBy] uniqueidentifier  NULL,
    [createdDate] datetime  NULL,
    [deprecated] bit  NOT NULL,
    [inUseBy] nvarchar(4000)  NULL,
    [displayOrder] smallint  NULL,
    [modifiedBy] uniqueidentifier  NULL,
    [modifiedDate] datetime  NULL,
    [name] nvarchar(4000)  NULL,
    [overview] nvarchar(4000)  NULL,
    [schemaVersion] smallint  NULL
);
GO

-- Creating table 'BDQueueEntries'
CREATE TABLE [dbo].[BDQueueEntries] (
    [uuid] uniqueidentifier  NOT NULL,
    [timestamp] datetime  NULL,
    [objectUuid] uniqueidentifier  NULL,
    [objectEntityName] nvarchar(4000)  NULL,
    [action] smallint  NULL
);
GO

-- Creating table 'BDSections'
CREATE TABLE [dbo].[BDSections] (
    [uuid] uniqueidentifier  NOT NULL,
    [name] nvarchar(4000)  NULL,
    [createdBy] uniqueidentifier  NULL,
    [createdDate] datetime  NULL,
    [modifiedBy] uniqueidentifier  NULL,
    [modifiedDate] datetime  NULL,
    [schemaVersion] smallint  NULL,
    [deprecated] bit  NOT NULL,
    [inUseBy] uniqueidentifier  NULL,
    [displayOrder] smallint  NULL
);
GO

-- Creating table 'BDSubcategories'
CREATE TABLE [dbo].[BDSubcategories] (
    [uuid] uniqueidentifier  NOT NULL,
    [categoryId] uniqueidentifier  NULL,
    [name] nvarchar(4000)  NULL,
    [createdBy] uniqueidentifier  NULL,
    [createdDate] datetime  NULL,
    [modifiedBy] uniqueidentifier  NULL,
    [modifiedDate] datetime  NULL,
    [schemaVersion] smallint  NULL,
    [deprecated] bit  NOT NULL,
    [inUseBy] uniqueidentifier  NULL,
    [displayOrder] smallint  NULL
);
GO

-- Creating table 'BDTherapies'
CREATE TABLE [dbo].[BDTherapies] (
    [uuid] uniqueidentifier  NOT NULL,
    [createdBy] uniqueidentifier  NULL,
    [createdDate] datetime  NULL,
    [deprecated] bit  NOT NULL,
    [displayOrder] smallint  NULL,
    [dosage] nvarchar(4000)  NULL,
    [duration] nvarchar(4000)  NULL,
    [inUseBy] nvarchar(4000)  NULL,
    [modifiedBy] uniqueidentifier  NULL,
    [modifiedDate] datetime  NULL,
    [name] nvarchar(4000)  NULL,
    [schemaVersion] smallint  NULL,
    [therapyGroupId] uniqueidentifier  NULL,
    [therapyGroupJoinType] smallint  NULL
);
GO

-- Creating table 'BDTherapyGroups'
CREATE TABLE [dbo].[BDTherapyGroups] (
    [uuid] uniqueidentifier  NOT NULL,
    [createdBy] uniqueidentifier  NULL,
    [createdDate] datetime  NULL,
    [deprecated] bit  NOT NULL,
    [displayOrder] smallint  NULL,
    [inUseBy] nvarchar(4000)  NULL,
    [modifiedBy] uniqueidentifier  NULL,
    [modifiedDate] datetime  NULL,
    [pathogenId] uniqueidentifier  NULL,
    [schemalVersion] smallint  NULL,
    [therapyNote] nvarchar(4000)  NULL,
    [name] nvarchar(4000)  NULL,
    [schemaVersion] smallint  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [uuid] in table 'BDCategories'
ALTER TABLE [dbo].[BDCategories]
ADD CONSTRAINT [PK_BDCategories]
    PRIMARY KEY CLUSTERED ([uuid] ASC);
GO

-- Creating primary key on [uuid] in table 'BDDiseases'
ALTER TABLE [dbo].[BDDiseases]
ADD CONSTRAINT [PK_BDDiseases]
    PRIMARY KEY CLUSTERED ([uuid] ASC);
GO

-- Creating primary key on [uuid] in table 'BDLinkedNotes'
ALTER TABLE [dbo].[BDLinkedNotes]
ADD CONSTRAINT [PK_BDLinkedNotes]
    PRIMARY KEY CLUSTERED ([uuid] ASC);
GO

-- Creating primary key on [uuid] in table 'BDPathogens'
ALTER TABLE [dbo].[BDPathogens]
ADD CONSTRAINT [PK_BDPathogens]
    PRIMARY KEY CLUSTERED ([uuid] ASC);
GO

-- Creating primary key on [uuid] in table 'BDPresentations'
ALTER TABLE [dbo].[BDPresentations]
ADD CONSTRAINT [PK_BDPresentations]
    PRIMARY KEY CLUSTERED ([uuid] ASC);
GO

-- Creating primary key on [uuid] in table 'BDQueueEntries'
ALTER TABLE [dbo].[BDQueueEntries]
ADD CONSTRAINT [PK_BDQueueEntries]
    PRIMARY KEY CLUSTERED ([uuid] ASC);
GO

-- Creating primary key on [uuid] in table 'BDSections'
ALTER TABLE [dbo].[BDSections]
ADD CONSTRAINT [PK_BDSections]
    PRIMARY KEY CLUSTERED ([uuid] ASC);
GO

-- Creating primary key on [uuid] in table 'BDSubcategories'
ALTER TABLE [dbo].[BDSubcategories]
ADD CONSTRAINT [PK_BDSubcategories]
    PRIMARY KEY CLUSTERED ([uuid] ASC);
GO

-- Creating primary key on [uuid] in table 'BDTherapies'
ALTER TABLE [dbo].[BDTherapies]
ADD CONSTRAINT [PK_BDTherapies]
    PRIMARY KEY CLUSTERED ([uuid] ASC);
GO

-- Creating primary key on [uuid] in table 'BDTherapyGroups'
ALTER TABLE [dbo].[BDTherapyGroups]
ADD CONSTRAINT [PK_BDTherapyGroups]
    PRIMARY KEY CLUSTERED ([uuid] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------