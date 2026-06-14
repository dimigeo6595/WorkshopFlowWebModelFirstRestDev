BEGIN TRY
    BEGIN TRANSACTION;

    -- ============================================
    -- WorkshopFlow - Seed Workstations
    -- ============================================

    INSERT INTO [dbo].[Workstations] ([Code], [Name], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT [Code], [Name], GETUTCDATE(), GETUTCDATE(), 0
    FROM (VALUES
        ('C1',  'Cutting Station 1'),
        ('C2',  'Cutting Station 2'),
        ('F1',  'Forming Station 1'),
        ('F2',  'Forming Station 2'),
        ('A1',  'Assembly Station'),
        ('A3',  'Welding Station'),
        ('A6',  'Paint Station'),
        ('A9',  'Quality Control')
    ) AS NewWS([Code], [Name])
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[Workstations] w
        WHERE w.[Code] = NewWS.[Code]
    );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;


DBCC CHECKIDENT ('dbo.Workstations', RESEED, 8);
