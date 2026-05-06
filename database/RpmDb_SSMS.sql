-- =============================================
-- Недвижимость.РФ - Скрипт создания БД для SSMS
-- Версия: 1.0
-- Совместимость: MS SQL Server 2019+
-- =============================================

USE [master]
GO

IF DB_ID('RpmDb') IS NOT NULL
BEGIN
    ALTER DATABASE [RpmDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [RpmDb];
END
GO

CREATE DATABASE [RpmDb]
GO

USE [RpmDb]
GO

-- =============================================
-- Справочники (Lookup Tables)
-- =============================================

CREATE TABLE [dbo].[Roles](
    [RoleID] [int] IDENTITY(1,1) NOT NULL,
    [RoleName] [nvarchar](50) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([RoleID] ASC),
    CONSTRAINT [UQ_Roles_RoleName] UNIQUE NONCLUSTERED ([RoleName] ASC)
)
GO

CREATE TABLE [dbo].[PropertyStatuses](
    [StatusID] [int] IDENTITY(1,1) NOT NULL,
    [StatusName] [nvarchar](50) NOT NULL,
    CONSTRAINT [PK_PropertyStatuses] PRIMARY KEY CLUSTERED ([StatusID] ASC),
    CONSTRAINT [UQ_PropertyStatuses_StatusName] UNIQUE NONCLUSTERED ([StatusName] ASC)
)
GO

CREATE TABLE [dbo].[PaymentStatuses](
    [StatusID] [int] IDENTITY(1,1) NOT NULL,
    [StatusName] [nvarchar](50) NOT NULL,
    CONSTRAINT [PK_PaymentStatuses] PRIMARY KEY CLUSTERED ([StatusID] ASC),
    CONSTRAINT [UQ_PaymentStatuses_StatusName] UNIQUE NONCLUSTERED ([StatusName] ASC)
)
GO

CREATE TABLE [dbo].[RequestStatuses](
    [StatusID] [int] IDENTITY(1,1) NOT NULL,
    [StatusName] [nvarchar](50) NOT NULL,
    CONSTRAINT [PK_RequestStatuses] PRIMARY KEY CLUSTERED ([StatusID] ASC),
    CONSTRAINT [UQ_RequestStatuses_StatusName] UNIQUE NONCLUSTERED ([StatusName] ASC)
)
GO

-- =============================================
-- Основные таблицы
-- =============================================

CREATE TABLE [dbo].[Users](
    [UserID] [int] IDENTITY(1,1) NOT NULL,
    [Email] [nvarchar](100) NOT NULL,
    [PasswordHash] [nvarchar](255) NOT NULL,
    [Phone] [nvarchar](20) NULL,
    [RegistrationDate] [datetime2](7) NULL DEFAULT (GETDATE()),
    [IsActive] [bit] NULL DEFAULT (1),
    [LastLoginDate] [datetime2](7) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserID] ASC),
    CONSTRAINT [UQ_Users_Email] UNIQUE NONCLUSTERED ([Email] ASC)
)
GO

CREATE TABLE [dbo].[UserRoles](
    [UserID] [int] NOT NULL,
    [RoleID] [int] NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([UserID] ASC, [RoleID] ASC),
    CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserID]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Roles]([RoleID])
)
GO

CREATE TABLE [dbo].[AdminProfiles](
    [AdminID] [int] NOT NULL,
    [FullName] [nvarchar](100) NOT NULL,
    CONSTRAINT [PK_AdminProfiles] PRIMARY KEY CLUSTERED ([AdminID] ASC),
    CONSTRAINT [FK_AdminProfiles_Users] FOREIGN KEY ([AdminID]) REFERENCES [dbo].[Users]([UserID]) ON DELETE CASCADE
)
GO

CREATE TABLE [dbo].[LandlordProfiles](
    [LandlordID] [int] NOT NULL,
    [CompanyName] [nvarchar](100) NULL,
    [ContactPerson] [nvarchar](100) NOT NULL,
    [INN] [nvarchar](20) NULL,
    CONSTRAINT [PK_LandlordProfiles] PRIMARY KEY CLUSTERED ([LandlordID] ASC),
    CONSTRAINT [FK_LandlordProfiles_Users] FOREIGN KEY ([LandlordID]) REFERENCES [dbo].[Users]([UserID]) ON DELETE CASCADE
)
GO

CREATE TABLE [dbo].[TenantProfiles](
    [TenantID] [int] NOT NULL,
    [FirstName] [nvarchar](50) NOT NULL,
    [LastName] [nvarchar](50) NOT NULL,
    [PassportData] [nvarchar](100) NULL,
    CONSTRAINT [PK_TenantProfiles] PRIMARY KEY CLUSTERED ([TenantID] ASC),
    CONSTRAINT [FK_TenantProfiles_Users] FOREIGN KEY ([TenantID]) REFERENCES [dbo].[Users]([UserID]) ON DELETE CASCADE
)
GO

CREATE TABLE [dbo].[Properties](
    [PropertyID] [int] IDENTITY(1,1) NOT NULL,
    [LandlordID] [int] NOT NULL,
    [Title] [nvarchar](200) NOT NULL,
    [Description] [nvarchar](max) NULL,
    [Address] [nvarchar](255) NOT NULL,
    [City] [nvarchar](100) NOT NULL,
    [District] [nvarchar](100) NULL,
    [PropertyType] [nvarchar](50) NOT NULL,
    [Area] [decimal](10, 2) NOT NULL,
    [Rooms] [int] NULL,
    [Floor] [int] NULL,
    [TotalFloors] [int] NULL,
    [Price] [decimal](19, 2) NOT NULL,
    [IsActive] [bit] NULL DEFAULT (0),
    [CreatedDate] [datetime2](7) NULL DEFAULT (GETDATE()),
    [StatusID] [int] NOT NULL DEFAULT (1),
    CONSTRAINT [PK_Properties] PRIMARY KEY CLUSTERED ([PropertyID] ASC),
    CONSTRAINT [FK_Properties_Users] FOREIGN KEY ([LandlordID]) REFERENCES [dbo].[Users]([UserID]),
    CONSTRAINT [FK_Properties_Statuses] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[PropertyStatuses]([StatusID])
)
GO

CREATE TABLE [dbo].[PropertyMedia](
    [MediaID] [int] IDENTITY(1,1) NOT NULL,
    [PropertyID] [int] NOT NULL,
    [MediaType] [nvarchar](10) NOT NULL,
    [FilePath] [nvarchar](500) NOT NULL,
    [IsMain] [bit] NULL DEFAULT (0),
    [UploadDate] [datetime2](7) NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_PropertyMedia] PRIMARY KEY CLUSTERED ([MediaID] ASC),
    CONSTRAINT [FK_PropertyMedia_Properties] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Properties]([PropertyID]) ON DELETE CASCADE
)
GO

CREATE TABLE [dbo].[Favorites](
    [UserID] [int] NOT NULL,
    [PropertyID] [int] NOT NULL,
    [AddedDate] [datetime2](7) NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Favorites] PRIMARY KEY CLUSTERED ([UserID] ASC, [PropertyID] ASC),
    CONSTRAINT [FK_Favorites_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserID]),
    CONSTRAINT [FK_Favorites_Properties] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Properties]([PropertyID])
)
GO

CREATE TABLE [dbo].[LeaseAgreements](
    [AgreementID] [int] IDENTITY(1,1) NOT NULL,
    [TenantID] [int] NOT NULL,
    [PropertyID] [int] NOT NULL,
    [StartDate] [date] NOT NULL,
    [EndDate] [date] NOT NULL,
    [MonthlyRent] [decimal](19, 2) NOT NULL,
    [Deposit] [decimal](19, 2) NULL,
    [DocumentPath] [nvarchar](500) NULL,
    [IsActive] [bit] NULL DEFAULT (1),
    [CreatedDate] [datetime2](7) NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_LeaseAgreements] PRIMARY KEY CLUSTERED ([AgreementID] ASC),
    CONSTRAINT [FK_LeaseAgreements_Users] FOREIGN KEY ([TenantID]) REFERENCES [dbo].[Users]([UserID]),
    CONSTRAINT [FK_LeaseAgreements_Properties] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Properties]([PropertyID])
)
GO

CREATE TABLE [dbo].[Payments](
    [PaymentID] [int] IDENTITY(1,1) NOT NULL,
    [AgreementID] [int] NOT NULL,
    [Amount] [decimal](19, 2) NOT NULL,
    [DueDate] [date] NOT NULL,
    [PaymentDate] [date] NULL,
    [StatusID] [int] NOT NULL DEFAULT (1),
    [TransactionID] [nvarchar](100) NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED ([PaymentID] ASC),
    CONSTRAINT [FK_Payments_LeaseAgreements] FOREIGN KEY ([AgreementID]) REFERENCES [dbo].[LeaseAgreements]([AgreementID]),
    CONSTRAINT [FK_Payments_PaymentStatuses] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[PaymentStatuses]([StatusID])
)
GO

CREATE TABLE [dbo].[ViewingRequests](
    [RequestID] [int] IDENTITY(1,1) NOT NULL,
    [TenantID] [int] NOT NULL,
    [PropertyID] [int] NOT NULL,
    [RequestDate] [datetime2](7) NULL DEFAULT (GETDATE()),
    [PreferredDate] [datetime2](7) NULL,
    [Message] [nvarchar](500) NULL,
    [StatusID] [int] NOT NULL DEFAULT (1),
    CONSTRAINT [PK_ViewingRequests] PRIMARY KEY CLUSTERED ([RequestID] ASC),
    CONSTRAINT [FK_ViewingRequests_Users] FOREIGN KEY ([TenantID]) REFERENCES [dbo].[Users]([UserID]),
    CONSTRAINT [FK_ViewingRequests_Properties] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Properties]([PropertyID]),
    CONSTRAINT [FK_ViewingRequests_RequestStatuses] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[RequestStatuses]([StatusID])
)
GO

CREATE TABLE [dbo].[Notifications](
    [NotificationID] [int] IDENTITY(1,1) NOT NULL,
    [UserID] [int] NOT NULL,
    [Message] [nvarchar](500) NOT NULL,
    [NotificationType] [nvarchar](50) NOT NULL,
    [IsRead] [bit] NULL DEFAULT (0),
    [CreatedDate] [datetime2](7) NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED ([NotificationID] ASC),
    CONSTRAINT [FK_Notifications_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserID]) ON DELETE CASCADE
)
GO

CREATE TABLE [dbo].[ModerationHistory](
    [ModerationID] [int] IDENTITY(1,1) NOT NULL,
    [PropertyID] [int] NOT NULL,
    [AdminID] [int] NOT NULL,
    [Action] [nvarchar](20) NOT NULL,
    [Comment] [nvarchar](500) NULL,
    [ActionDate] [datetime2](7) NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_ModerationHistory] PRIMARY KEY CLUSTERED ([ModerationID] ASC),
    CONSTRAINT [FK_ModerationHistory_Properties] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Properties]([PropertyID]),
    CONSTRAINT [FK_ModerationHistory_Users] FOREIGN KEY ([AdminID]) REFERENCES [dbo].[Users]([UserID])
)
GO

CREATE TABLE [dbo].[TransferActs](
    [ActID] [int] IDENTITY(1,1) NOT NULL,
    [AgreementID] [int] NOT NULL,
    [ActType] [nvarchar](20) NOT NULL,
    [ActDate] [date] NOT NULL,
    [DocumentPath] [nvarchar](500) NULL,
    [Notes] [nvarchar](max) NULL,
    CONSTRAINT [PK_TransferActs] PRIMARY KEY CLUSTERED ([ActID] ASC),
    CONSTRAINT [FK_TransferActs_LeaseAgreements] FOREIGN KEY ([AgreementID]) REFERENCES [dbo].[LeaseAgreements]([AgreementID]) ON DELETE CASCADE
)
GO

-- =============================================
-- Заполнение справочников
-- =============================================

INSERT INTO [dbo].[Roles] ([RoleName]) VALUES (N'admin'), (N'landlord'), (N'tenant')
GO

INSERT INTO [dbo].[PropertyStatuses] ([StatusName]) VALUES
    (N'На модерации'), (N'Активно'), (N'Отклонено'), (N'Архив')
GO

INSERT INTO [dbo].[PaymentStatuses] ([StatusName]) VALUES
    (N'Ожидает оплаты'), (N'Оплачено'), (N'Просрочено')
GO

INSERT INTO [dbo].[RequestStatuses] ([StatusName]) VALUES
    (N'Новая'), (N'Подтверждена'), (N'Отклонена'), (N'Завершена')
GO

-- =============================================
-- Тестовые данные (демо-аккаунты)
-- Пароли захэшированы через bcrypt
-- admin123, realtor123, client123
-- =============================================

-- Примечание: хэши паролей генерируются при первом запуске backend API
-- Для ручного создания используйте API /api/auth/register

PRINT N'База данных RpmDb успешно создана!'
PRINT N'Справочники заполнены.'
PRINT N'Запустите backend API для автоматического создания демо-пользователей.'
GO
