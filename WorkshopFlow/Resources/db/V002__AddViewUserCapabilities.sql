BEGIN TRY
    BEGIN TRANSACTION;

    -- ============================================
    -- Migration: Add VIEW_USER and VIEW_USERS capabilities
    -- Assign to ADMIN and PRODUCTION_ENGINEER
    -- ============================================

    INSERT INTO [dbo].[Capabilities] ([Name], [Description])
    SELECT [Name], [Description]
    FROM (VALUES
        ('VIEW_USER',  'View a single user'),
        ('VIEW_USERS', 'View user list and details')
    ) AS NewCaps([Name], [Description])
    WHERE NOT EXISTS (
        SELECT 1
        FROM [dbo].[Capabilities] c
        WHERE c.[Name] = NewCaps.[Name]
    );

    -- ============================================
    -- Assign to ADMIN
    -- ============================================
    INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
    SELECT r.[Id], c.[Id]
    FROM [dbo].[Roles] r
    CROSS JOIN [dbo].[Capabilities] c
    WHERE r.[Name] = 'ADMIN'
      AND c.[Name] IN ('VIEW_USER', 'VIEW_USERS')
      AND NOT EXISTS (
          SELECT 1
          FROM [dbo].[RolesCapabilities] rc
          WHERE rc.[RolesId] = r.[Id]
            AND rc.[CapabilitiesId] = c.[Id]
      );

    -- ============================================
    -- Assign to PRODUCTION_ENGINEER
    -- ============================================
    INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
    SELECT r.[Id], c.[Id]
    FROM [dbo].[Roles] r
    CROSS JOIN [dbo].[Capabilities] c
    WHERE r.[Name] = 'PRODUCTION_ENGINEER'
      AND c.[Name] IN ('VIEW_USER', 'VIEW_USERS')
      AND NOT EXISTS (
          SELECT 1
          FROM [dbo].[RolesCapabilities] rc
          WHERE rc.[RolesId] = r.[Id]
            AND rc.[CapabilitiesId] = c.[Id]
      );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;

DBCC CHECKIDENT ('dbo.Capabilities', RESEED, 24);
