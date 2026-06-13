BEGIN TRY
    BEGIN TRANSACTION;

    -- ============================================
    -- WorkshopFlow - Seed UnitOfMeasures
    -- ============================================

    INSERT INTO [dbo].[UnitOfMeasures] ([Name], [Symbol], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT [Name], [Symbol], GETUTCDATE(), GETUTCDATE(), 0
    FROM (VALUES
        ('Pieces',          'pcs'),
        ('Kilograms',       'kg'),
        ('Grams',           'gr'),
        ('Meters',          'm'),
        ('Liters',          'lt'),
        ('Milliliters',     'ml'),
        ('Square Meters',   'm2'),
        ('Hours',           'hr')
    ) AS NewUoMs([Name], [Symbol])
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[UnitOfMeasures] u
        WHERE u.[Symbol] = NewUoMs.[Symbol]
    );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;

DBCC CHECKIDENT ('dbo.UnitOfMeasures', RESEED, 8);