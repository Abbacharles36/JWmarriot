IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Merchants] (
    [MerchantId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [ContactEmail] nvarchar(max) NOT NULL,
    [ContactPhone] nvarchar(max) NOT NULL,
    [PortalToken] nvarchar(max) NOT NULL,
    [MaxInvites] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Merchants] PRIMARY KEY ([MerchantId])
);

CREATE TABLE [AuditLogs] (
    [LogId] int NOT NULL IDENTITY,
    [MerchantId] int NOT NULL,
    [Action] nvarchar(max) NOT NULL,
    [Details] nvarchar(max) NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [PerformedBy] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([LogId]),
    CONSTRAINT [FK_AuditLogs_Merchants_MerchantId] FOREIGN KEY ([MerchantId]) REFERENCES [Merchants] ([MerchantId]) ON DELETE CASCADE
);

CREATE TABLE [Invitations] (
    [InvitationId] int NOT NULL IDENTITY,
    [MerchantId] int NOT NULL,
    [GuestName] nvarchar(max) NOT NULL,
    [GuestEmail] nvarchar(max) NOT NULL,
    [GuestPhone] nvarchar(max) NOT NULL,
    [GuestIdNumber] nvarchar(max) NOT NULL,
    [TicketNumber] nvarchar(max) NOT NULL,
    [QrCodeBase64] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [CheckedInAt] datetime2 NULL,
    CONSTRAINT [PK_Invitations] PRIMARY KEY ([InvitationId]),
    CONSTRAINT [FK_Invitations_Merchants_MerchantId] FOREIGN KEY ([MerchantId]) REFERENCES [Merchants] ([MerchantId]) ON DELETE CASCADE
);

CREATE INDEX [IX_AuditLogs_MerchantId] ON [AuditLogs] ([MerchantId]);

CREATE INDEX [IX_Invitations_MerchantId] ON [Invitations] ([MerchantId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260615200909_InitialCreate', N'9.0.0');

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Merchants]') AND [c].[name] = N'PortalToken');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Merchants] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Merchants] ALTER COLUMN [PortalToken] nvarchar(450) NOT NULL;

CREATE UNIQUE INDEX [IX_Merchants_PortalToken] ON [Merchants] ([PortalToken]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260615205726_AddPortalTokenIndex', N'9.0.0');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260702091955_ServerDeploymentMigration', N'9.0.0');

COMMIT;
GO

