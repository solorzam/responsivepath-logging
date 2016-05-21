CREATE TABLE [dbo].[EntryIndexes]
(
    [EntryId] [int] NOT NULL,
    [Key] [varchar](50) NOT NULL,
    [Value] [varchar](200) NOT NULL, 
    CONSTRAINT [PK_EntryIndexes] PRIMARY KEY ([EntryId], [Value], [Key]), 
    CONSTRAINT [FK_EntryIndexes_Entries] FOREIGN KEY (EntryId) REFERENCES [Entries] ([EntryId])
)

GO

CREATE INDEX [EntryIndexes_KeyValue] ON [dbo].[EntryIndexes] ([Key] ASC, [Value] ASC)
