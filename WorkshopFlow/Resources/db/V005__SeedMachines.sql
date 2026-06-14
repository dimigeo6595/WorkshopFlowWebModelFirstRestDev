BEGIN TRY
    BEGIN TRANSACTION;

    -- ============================================
    -- WorkshopFlow - Seed Machines
    -- ============================================

    -- C1 - Cutting Station 1: Hypertherm MAXPRO200 Plasma Cutter
    INSERT INTO [dbo].[Machines] ([Code], [Name], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'MCH-C1-01', 'Hypertherm MAXPRO200 Plasma Cutter',
        (SELECT [Id] FROM [dbo].[Workstations] WHERE [Code] = 'C1'),
        GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Machines] WHERE [Code] = 'MCH-C1-01');

    -- C2 - Cutting Station 2: Hypertherm XPR300 Plasma Cutter
    INSERT INTO [dbo].[Machines] ([Code], [Name], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'MCH-C2-01', 'Hypertherm XPR300 Plasma Cutter',
        (SELECT [Id] FROM [dbo].[Workstations] WHERE [Code] = 'C2'),
        GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Machines] WHERE [Code] = 'MCH-C2-01');

    -- F1 - Forming Station 1: Amada HFE 1303 Press Brake (Στράντζα)
    INSERT INTO [dbo].[Machines] ([Code], [Name], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'MCH-F1-01', 'Amada HFE 1303 Press Brake',
        (SELECT [Id] FROM [dbo].[Workstations] WHERE [Code] = 'F1'),
        GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Machines] WHERE [Code] = 'MCH-F1-01');

    -- F2 - Forming Station 2: Davi MCB 2020 4-Roll Plate Rolling Machine (Κύλινδρος)
    INSERT INTO [dbo].[Machines] ([Code], [Name], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'MCH-F2-01', 'Davi MCB 2020 4-Roll Plate Rolling Machine',
        (SELECT [Id] FROM [dbo].[Workstations] WHERE [Code] = 'F2'),
        GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Machines] WHERE [Code] = 'MCH-F2-01');

    -- A1 - Assembly Station: δύο welding machines
    INSERT INTO [dbo].[Machines] ([Code], [Name], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'MCH-A1-01', 'Lincoln Electric Power MIG 262MP Welder',
        (SELECT [Id] FROM [dbo].[Workstations] WHERE [Code] = 'A1'),
        GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Machines] WHERE [Code] = 'MCH-A1-01');

    INSERT INTO [dbo].[Machines] ([Code], [Name], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'MCH-A1-02', 'Miller Multimatic 220 TIG/MIG Welder',
        (SELECT [Id] FROM [dbo].[Workstations] WHERE [Code] = 'A1'),
        GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Machines] WHERE [Code] = 'MCH-A1-02');

    -- A3 - Welding Station: δύο welding machines
    INSERT INTO [dbo].[Machines] ([Code], [Name], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'MCH-A3-01', 'Lincoln Electric Power MIG 262MP Welder',
        (SELECT [Id] FROM [dbo].[Workstations] WHERE [Code] = 'A3'),
        GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Machines] WHERE [Code] = 'MCH-A3-01');

    INSERT INTO [dbo].[Machines] ([Code], [Name], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'MCH-A3-02', 'Miller Multimatic 220 TIG/MIG Welder',
        (SELECT [Id] FROM [dbo].[Workstations] WHERE [Code] = 'A3'),
        GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Machines] WHERE [Code] = 'MCH-A3-02');

    -- A6 - Paint Station: Graco Mark HD Airless Sprayer
    INSERT INTO [dbo].[Machines] ([Code], [Name], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'MCH-A6-01', 'Graco Mark HD Electric Airless Sprayer',
        (SELECT [Id] FROM [dbo].[Workstations] WHERE [Code] = 'A6'),
        GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Machines] WHERE [Code] = 'MCH-A6-01');

    -- A9 - Quality Control: no equipment

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;

DBCC CHECKIDENT ('dbo.Machines', RESEED, 8);
