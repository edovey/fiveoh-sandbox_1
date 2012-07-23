-- Creating table 'BDConfiguredEntry'
CREATE TABLE [BDConfiguredEntry] (
    [uuid] uniqueidentifier  NOT NULL,
    [name] nvarchar(750)  NULL,
    [parentId] uniqueidentifier  NULL,
    [parentKeyName] nvarchar(100)  NULL,
    [nodeType] int  NULL,
    [layoutVariant] int  NULL,
    [displayOrder] int  NULL,
    [schemaVersion] int  NULL,
    [createdDate] datetime  NULL,
    [modifiedDate] datetime  NULL,
    [field01] nvarchar(750)  NULL,
    [field02] nvarchar(750)  NULL,
    [field03] nvarchar(750)  NULL,
    [field04] nvarchar(750)  NULL,
    [field05] nvarchar(750)  NULL,
    [field06] nvarchar(750)  NULL,
    [field07] nvarchar(750)  NULL,
    [field08] nvarchar(750)  NULL,
    [field09] nvarchar(750)  NULL,
    [field10] nvarchar(750)  NULL,
    [field11] nvarchar(750)  NULL,
    [field12] nvarchar(750)  NULL,
    [field13] nvarchar(750)  NULL,
    [field14] nvarchar(750)  NULL,
    [field15] nvarchar(750)  NULL,
    [parentType] int NULL
);
GO

-- Creating primary key on [uuid] in table 'BDConfiguredEntry'
ALTER TABLE [BDConfiguredEntry]
ADD CONSTRAINT [PK_BDConfiguredEntry]
    PRIMARY KEY ([uuid] );
GO

