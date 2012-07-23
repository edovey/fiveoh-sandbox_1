-- Creating table 'BDCombinedEntries'
CREATE TABLE [BDCombinedEntry] (
    [uuid] uniqueidentifier  NOT NULL,
    [nodeType] int  NULL,
    [layoutVariant] int  NULL,
    [parentId] uniqueidentifier  NULL,
    [parentType] int  NULL,
    [parentKeyName] nvarchar(100)  NULL,
    [displayOrder] int  NULL,
    [createdDate] datetime  NULL,
    [modifiedDate] datetime  NULL,
    [schemaVersion] int  NULL,
    [groupTitle] nvarchar(750)  NULL,
    [name] nvarchar(750)  NULL,
    [groupJoinType] int  NULL,
    [entryTitle01] nvarchar(750)  NULL,
    [entryDetail01] nvarchar(750)  NULL,
    [entryJoinType01] int  NULL,
    [entryTitle02] nvarchar(750)  NULL,
    [entryDetail02] nvarchar(750)  NULL,
    [entryJoinType02] int  NULL,
    [entryTitle03] nvarchar(750)  NULL,
    [entryDetail03] nvarchar(750)  NULL,
    [entryJoinType03] int  NULL,
    [entryTitle04] nvarchar(750)  NULL,
    [entryDetail04] nvarchar(750)  NULL,
    [entryJoinType04] int  NULL
);
GO

-- Creating primary key on [uuid] in table 'BDCombinedEntries'
ALTER TABLE [BDCombinedEntry]
ADD CONSTRAINT [PK_BDCombinedEntry]
    PRIMARY KEY ([uuid] );
GO