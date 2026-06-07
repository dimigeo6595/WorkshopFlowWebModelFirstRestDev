BEGIN TRY
    BEGIN TRANSACTION;

    -- ============================================
    -- WorkshopFlowDB - Seed Data
    -- Roles, Capabilities, Role-Capability mappings
    -- ============================================

    -- ============================================
    -- Insert Roles
    -- ============================================
    INSERT INTO [dbo].[Roles] ([Name])
    VALUES
        ('ADMIN'),
        ('PRODUCTION_ENGINEER'),
        ('OPERATOR'),
        ('WAREHOUSE_MANAGER');

    -- ============================================
    -- Insert Capabilities
    -- ============================================
    INSERT INTO [dbo].[Capabilities] ([Name], [Description])
    VALUES
        ('VIEW_USERS', 'View users'),
        ('INSERT_USER', 'Create users'),
        ('EDIT_USER', 'Modify users'),
        ('DELETE_USER', 'Deactivate users'),

        ('VIEW_ITEMS', 'View item list and details'),
        ('INSERT_ITEM', 'Create new items'),
        ('EDIT_ITEM', 'Modify existing items'),
        ('DELETE_ITEM', 'Deactivate items'),

        ('VIEW_BOM', 'View item bill of materials'),
        ('EDIT_BOM', 'Create or modify item bill of materials'),

        ('VIEW_ROUTING', 'View item routing'),
        ('EDIT_ROUTING', 'Create or modify item routing'),

        ('VIEW_MACHINES', 'View machines and workstations'),
        ('EDIT_MACHINES', 'Create or modify machines and workstations'),

        ('VIEW_WORK_ORDERS', 'View work orders'),
        ('INSERT_WORK_ORDER', 'Create work orders'),
        ('EDIT_WORK_ORDER', 'Modify work orders'),
        ('START_WORK_ORDER', 'Start work orders'),
        ('COMPLETE_WORK_ORDER', 'Complete work orders'),
        ('ASSIGN_WORK_ORDER', 'Assign users to work orders'),

        ('VIEW_INVENTORY', 'View inventory and transactions'),
        ('ADJUST_INVENTORY', 'Create manual inventory adjustments');

    -- ============================================
    -- ADMIN: all capabilities
    -- ============================================
    INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
    SELECT r.[Id], c.[Id]
    FROM [dbo].[Roles] r
    CROSS JOIN [dbo].[Capabilities] c
    WHERE r.[Name] = 'ADMIN';

    -- ============================================
    -- PRODUCTION_ENGINEER
    -- ============================================
    INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
    SELECT r.[Id], c.[Id]
    FROM [dbo].[Roles] r
    CROSS JOIN [dbo].[Capabilities] c
    WHERE r.[Name] = 'PRODUCTION_ENGINEER'
      AND c.[Name] IN (
            'VIEW_ITEMS', 'INSERT_ITEM', 'EDIT_ITEM',
            'VIEW_BOM', 'EDIT_BOM',
            'VIEW_ROUTING', 'EDIT_ROUTING',
            'VIEW_MACHINES',
            'VIEW_WORK_ORDERS', 'INSERT_WORK_ORDER', 'EDIT_WORK_ORDER',
            'START_WORK_ORDER', 'COMPLETE_WORK_ORDER', 'ASSIGN_WORK_ORDER',
            'VIEW_INVENTORY'
      );

    -- ============================================
    -- OPERATOR
    -- ============================================
    INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
    SELECT r.[Id], c.[Id]
    FROM [dbo].[Roles] r
    CROSS JOIN [dbo].[Capabilities] c
    WHERE r.[Name] = 'OPERATOR'
      AND c.[Name] IN (
            'VIEW_WORK_ORDERS',
            'START_WORK_ORDER',
            'COMPLETE_WORK_ORDER',
            'VIEW_ITEMS',
            'VIEW_ROUTING'
      );

    -- ============================================
    -- WAREHOUSE_MANAGER
    -- ============================================
    INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
    SELECT r.[Id], c.[Id]
    FROM [dbo].[Roles] r
    CROSS JOIN [dbo].[Capabilities] c
    WHERE r.[Name] = 'WAREHOUSE_MANAGER'
      AND c.[Name] IN (
            'VIEW_ITEMS',
            'VIEW_INVENTORY',
            'ADJUST_INVENTORY'
      );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;

DBCC CHECKIDENT ('dbo.Roles', RESEED, 4);
DBCC CHECKIDENT ('dbo.Capabilities', RESEED, 22);