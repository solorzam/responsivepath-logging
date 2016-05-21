CREATE TABLE [dbo].[Entries]
(
    [EntryId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Timestamp] DATETIME2 NOT NULL, 
    [Severity] NVARCHAR(50) NOT NULL, 
    [Message] NVARCHAR(200) NULL, 
    [Exception] NVARCHAR(MAX) NULL, 
    [Data] NVARCHAR(MAX) NULL
)

GO

CREATE INDEX [Entries_Severity] ON [dbo].[Entries] ([Severity] ASC, [Timestamp] ASC)

GO

CREATE INDEX [Entries_Time] ON [dbo].[Entries] ([Timestamp] ASC)
