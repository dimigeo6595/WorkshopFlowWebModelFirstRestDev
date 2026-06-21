BEGIN TRY
    BEGIN TRANSACTION;

    -- ============================================
    -- WorkshopFlow - Seed Elbow/Duct hierarchy
    -- Δημιουργεί: 4 raw materials/consumables,
    -- 7 SemiFinished Cylinders, 7 SemiFinished Parts,
    -- 2 FinalProducts (Elbow, Duct), με πλήρες BOM + Routing
    -- ============================================

    DECLARE @UoM_pcs INT = (SELECT [Id] FROM [dbo].[UnitOfMeasures] WHERE [Symbol] = 'pcs');
    DECLARE @UoM_m   INT = (SELECT [Id] FROM [dbo].[UnitOfMeasures] WHERE [Symbol] = 'm');
    DECLARE @UoM_m2  INT = (SELECT [Id] FROM [dbo].[UnitOfMeasures] WHERE [Symbol] = 'm2');
    DECLARE @UoM_lt  INT = (SELECT [Id] FROM [dbo].[UnitOfMeasures] WHERE [Symbol] = 'lt');

    DECLARE @WS_Cutting1  INT = (SELECT [Id] FROM [dbo].[Workstations] WHERE [Code] = 'C1');
    DECLARE @WS_Cutting2  INT = (SELECT [Id] FROM [dbo].[Workstations] WHERE [Code] = 'C2');
    DECLARE @WS_Forming1  INT = (SELECT [Id] FROM [dbo].[Workstations] WHERE [Code] = 'F1');
    DECLARE @WS_Assembly  INT = (SELECT [Id] FROM [dbo].[Workstations] WHERE [Code] = 'A1');
    DECLARE @WS_Welding   INT = (SELECT [Id] FROM [dbo].[Workstations] WHERE [Code] = 'A3');
    DECLARE @WS_Paint     INT = (SELECT [Id] FROM [dbo].[Workstations] WHERE [Code] = 'A6');
    DECLARE @WS_QC        INT = (SELECT [Id] FROM [dbo].[Workstations] WHERE [Code] = 'A9');

    DECLARE @AdminUserId INT = (SELECT [Id] FROM [dbo].[Users] WHERE [Username] = 'admin');

    -- ============================================
    -- 1. Raw materials & consumables
    -- ============================================

    INSERT INTO [dbo].[Items]
        ([ItemCode], [Name], [Description], [ItemType], [StockQuantity], [WeightPerUoM], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'RM-WIRE-01', 'Steel Wire', 'Steel wire used in cylinders and assemblies', 'RawMaterial', 0, 0.15, @UoM_m, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Items] WHERE [ItemCode] = 'RM-WIRE-01');

    INSERT INTO [dbo].[Items]
        ([ItemCode], [Name], [Description], [ItemType], [StockQuantity], [WeightPerUoM], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'RM-SHEET-01', 'Sheet Metal', 'Galvanized sheet metal for cylinder bodies', 'RawMaterial', 0, 7.85, @UoM_m2, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Items] WHERE [ItemCode] = 'RM-SHEET-01');

    INSERT INTO [dbo].[Items]
        ([ItemCode], [Name], [Description], [ItemType], [StockQuantity], [WeightPerUoM], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'RM-FLANGE-01', 'Flange', 'Connection flange for ducts', 'RawMaterial', 0, 0.4, @UoM_pcs, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Items] WHERE [ItemCode] = 'RM-FLANGE-01');

    INSERT INTO [dbo].[Items]
        ([ItemCode], [Name], [Description], [ItemType], [StockQuantity], [WeightPerUoM], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'CON-PAINT-01', 'Paint', 'Protective paint coating', 'Consumable', 0, NULL, @UoM_lt, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Items] WHERE [ItemCode] = 'CON-PAINT-01');

    DECLARE @Wire INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'RM-WIRE-01');
    DECLARE @SheetMetal INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'RM-SHEET-01');
    DECLARE @Flange INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'RM-FLANGE-01');
    DECLARE @Paint INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'CON-PAINT-01');

    -- Initial stock (1000 units each) via InventoryTransactions + StockQuantity update
    INSERT INTO [dbo].[InventoryTransactions]
        ([TransactionType], [Quantity], [Notes], [ItemId], [CreatedByUserId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'Purchase', 1000, 'Initial seed stock', i.[Id], @AdminUserId, GETUTCDATE(), GETUTCDATE(), 0
    FROM [dbo].[Items] i
    WHERE i.[ItemCode] IN ('RM-WIRE-01', 'RM-SHEET-01', 'RM-FLANGE-01', 'CON-PAINT-01')
      AND NOT EXISTS (
          SELECT 1 FROM [dbo].[InventoryTransactions] t
          WHERE t.[ItemId] = i.[Id] AND t.[Notes] = 'Initial seed stock'
      );

    UPDATE [dbo].[Items]
    SET [StockQuantity] = [StockQuantity] + 1000, [ModifiedAt] = GETUTCDATE()
    WHERE [ItemCode] IN ('RM-WIRE-01', 'RM-SHEET-01', 'RM-FLANGE-01', 'CON-PAINT-01')
      AND [StockQuantity] = 0;

    -- ============================================
    -- 2. Cylinders (SemiFinished) — one per Part, each with own BOM + Routing
    --    BOM: Sheet Metal + Wire | Routing: Cutting -> Forming -> Welding
    -- ============================================

    INSERT INTO [dbo].[Items]
        ([ItemCode], [Name], [Description], [ItemType], [StockQuantity], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT v.[ItemCode], v.[Name], v.[Description], 'SemiFinished', 0, @UoM_pcs, GETUTCDATE(), GETUTCDATE(), 0
    FROM (VALUES
        ('SF-CYL-ELB-A', 'Cylinder for Elbow Part A', 'Cylinder body used inside Elbow Part A'),
        ('SF-CYL-ELB-B', 'Cylinder for Elbow Part B', 'Cylinder body used inside Elbow Part B'),
        ('SF-CYL-ELB-C', 'Cylinder for Elbow Part C', 'Cylinder body used inside Elbow Part C'),
        ('SF-CYL-ELB-D', 'Cylinder for Elbow Part D', 'Cylinder body used inside Elbow Part D'),
        ('SF-CYL-DUC-A', 'Cylinder for Duct Part A', 'Cylinder body used inside Duct Part A'),
        ('SF-CYL-DUC-B', 'Cylinder for Duct Part B', 'Cylinder body used inside Duct Part B'),
        ('SF-CYL-DUC-C', 'Cylinder for Duct Part C', 'Cylinder body used inside Duct Part C')
    ) AS v([ItemCode], [Name], [Description])
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Items] WHERE [ItemCode] = v.[ItemCode]);

    -- BOM lines for all 7 cylinders: Sheet Metal (0.8 m2) + Wire (1.2 m)
    INSERT INTO [dbo].[BomLines]
        ([Quantity], [Notes], [ProducedItemId], [ComponentItemId], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 0.8, 'Sheet metal for cylinder body', i.[Id], @SheetMetal, @UoM_m2, GETUTCDATE(), GETUTCDATE(), 0
    FROM [dbo].[Items] i
    WHERE i.[ItemCode] LIKE 'SF-CYL-%'
      AND NOT EXISTS (
          SELECT 1 FROM [dbo].[BomLines] b
          WHERE b.[ProducedItemId] = i.[Id] AND b.[ComponentItemId] = @SheetMetal
      );

    INSERT INTO [dbo].[BomLines]
        ([Quantity], [Notes], [ProducedItemId], [ComponentItemId], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 1.2, 'Wire for cylinder seam', i.[Id], @Wire, @UoM_m, GETUTCDATE(), GETUTCDATE(), 0
    FROM [dbo].[Items] i
    WHERE i.[ItemCode] LIKE 'SF-CYL-%'
      AND NOT EXISTS (
          SELECT 1 FROM [dbo].[BomLines] b
          WHERE b.[ProducedItemId] = i.[Id] AND b.[ComponentItemId] = @Wire
      );

    -- Routing for all 7 cylinders: Cutting -> Forming -> Welding
    INSERT INTO [dbo].[RoutingSteps]
        ([Sequence], [OperationName], [EstimatedMinutes], [Notes], [ProducedItemId], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 1, 'Cutting', 15, 'Cut sheet metal to size', i.[Id], @WS_Cutting1, GETUTCDATE(), GETUTCDATE(), 0
    FROM [dbo].[Items] i
    WHERE i.[ItemCode] LIKE 'SF-CYL-%'
      AND NOT EXISTS (SELECT 1 FROM [dbo].[RoutingSteps] r WHERE r.[ProducedItemId] = i.[Id] AND r.[Sequence] = 1);

    INSERT INTO [dbo].[RoutingSteps]
        ([Sequence], [OperationName], [EstimatedMinutes], [Notes], [ProducedItemId], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 2, 'Forming', 20, 'Roll into cylinder shape', i.[Id], @WS_Forming1, GETUTCDATE(), GETUTCDATE(), 0
    FROM [dbo].[Items] i
    WHERE i.[ItemCode] LIKE 'SF-CYL-%'
      AND NOT EXISTS (SELECT 1 FROM [dbo].[RoutingSteps] r WHERE r.[ProducedItemId] = i.[Id] AND r.[Sequence] = 2);

    INSERT INTO [dbo].[RoutingSteps]
        ([Sequence], [OperationName], [EstimatedMinutes], [Notes], [ProducedItemId], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 3, 'Welding', 25, 'Weld seam closed', i.[Id], @WS_Welding, GETUTCDATE(), GETUTCDATE(), 0
    FROM [dbo].[Items] i
    WHERE i.[ItemCode] LIKE 'SF-CYL-%'
      AND NOT EXISTS (SELECT 1 FROM [dbo].[RoutingSteps] r WHERE r.[ProducedItemId] = i.[Id] AND r.[Sequence] = 3);

    -- ============================================
    -- 3. Parts (SemiFinished) — own Cylinder + Wire
    --    Routing: Cutting -> Assembly -> QC
    -- ============================================

    INSERT INTO [dbo].[Items]
        ([ItemCode], [Name], [Description], [ItemType], [StockQuantity], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT v.[ItemCode], v.[Name], v.[Description], 'SemiFinished', 0, @UoM_pcs, GETUTCDATE(), GETUTCDATE(), 0
    FROM (VALUES
        ('SF-PART-ELB-A', 'Elbow Part A', 'Elbow Part A sub-assembly'),
        ('SF-PART-ELB-B', 'Elbow Part B', 'Elbow Part B sub-assembly'),
        ('SF-PART-ELB-C', 'Elbow Part C', 'Elbow Part C sub-assembly'),
        ('SF-PART-ELB-D', 'Elbow Part D', 'Elbow Part D sub-assembly'),
        ('SF-PART-DUC-A', 'Duct Part A', 'Duct Part A sub-assembly'),
        ('SF-PART-DUC-B', 'Duct Part B', 'Duct Part B sub-assembly'),
        ('SF-PART-DUC-C', 'Duct Part C', 'Duct Part C sub-assembly')
    ) AS v([ItemCode], [Name], [Description])
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Items] WHERE [ItemCode] = v.[ItemCode]);

    -- BOM: each Part uses its own matching Cylinder (1 pcs) + Wire (0.5 m)
    INSERT INTO [dbo].[BomLines]
        ([Quantity], [Notes], [ProducedItemId], [ComponentItemId], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 1, 'Cylinder body', part.[Id], cyl.[Id], @UoM_pcs, GETUTCDATE(), GETUTCDATE(), 0
    FROM [dbo].[Items] part
    JOIN [dbo].[Items] cyl
        ON cyl.[ItemCode] = REPLACE(part.[ItemCode], 'SF-PART-', 'SF-CYL-')
    WHERE part.[ItemCode] LIKE 'SF-PART-%'
      AND NOT EXISTS (
          SELECT 1 FROM [dbo].[BomLines] b
          WHERE b.[ProducedItemId] = part.[Id] AND b.[ComponentItemId] = cyl.[Id]
      );

    INSERT INTO [dbo].[BomLines]
        ([Quantity], [Notes], [ProducedItemId], [ComponentItemId], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 0.5, 'Wire for part assembly', i.[Id], @Wire, @UoM_m, GETUTCDATE(), GETUTCDATE(), 0
    FROM [dbo].[Items] i
    WHERE i.[ItemCode] LIKE 'SF-PART-%'
      AND NOT EXISTS (
          SELECT 1 FROM [dbo].[BomLines] b
          WHERE b.[ProducedItemId] = i.[Id] AND b.[ComponentItemId] = @Wire
      );

    -- Routing for all 7 parts: Cutting -> Assembly -> QC
    INSERT INTO [dbo].[RoutingSteps]
        ([Sequence], [OperationName], [EstimatedMinutes], [Notes], [ProducedItemId], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 1, 'Cutting', 10, 'Trim part to spec', i.[Id], @WS_Cutting2, GETUTCDATE(), GETUTCDATE(), 0
    FROM [dbo].[Items] i
    WHERE i.[ItemCode] LIKE 'SF-PART-%'
      AND NOT EXISTS (SELECT 1 FROM [dbo].[RoutingSteps] r WHERE r.[ProducedItemId] = i.[Id] AND r.[Sequence] = 1);

    INSERT INTO [dbo].[RoutingSteps]
        ([Sequence], [OperationName], [EstimatedMinutes], [Notes], [ProducedItemId], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 2, 'Assembly', 20, 'Assemble part components', i.[Id], @WS_Assembly, GETUTCDATE(), GETUTCDATE(), 0
    FROM [dbo].[Items] i
    WHERE i.[ItemCode] LIKE 'SF-PART-%'
      AND NOT EXISTS (SELECT 1 FROM [dbo].[RoutingSteps] r WHERE r.[ProducedItemId] = i.[Id] AND r.[Sequence] = 2);

    INSERT INTO [dbo].[RoutingSteps]
        ([Sequence], [OperationName], [EstimatedMinutes], [Notes], [ProducedItemId], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 3, 'Quality Control', 10, 'Inspect part', i.[Id], @WS_QC, GETUTCDATE(), GETUTCDATE(), 0
    FROM [dbo].[Items] i
    WHERE i.[ItemCode] LIKE 'SF-PART-%'
      AND NOT EXISTS (SELECT 1 FROM [dbo].[RoutingSteps] r WHERE r.[ProducedItemId] = i.[Id] AND r.[Sequence] = 3);

    -- ============================================
    -- 4. Elbow (FinalProduct): Part A, B, C, D + Wire
    --    Routing: Assembly -> Welding -> QC
    -- ============================================

    INSERT INTO [dbo].[Items]
        ([ItemCode], [Name], [Description], [ItemType], [StockQuantity], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'FP-ELBOW-01', 'Elbow Final Product', '90-degree duct elbow, fully assembled', 'FinalProduct', 0, @UoM_pcs, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Items] WHERE [ItemCode] = 'FP-ELBOW-01');

    DECLARE @Elbow INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'FP-ELBOW-01');

    INSERT INTO [dbo].[BomLines]
        ([Quantity], [Notes], [ProducedItemId], [ComponentItemId], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 1, CONCAT('Part ', RIGHT(part.[ItemCode], 1)), @Elbow, part.[Id], @UoM_pcs, GETUTCDATE(), GETUTCDATE(), 0
    FROM [dbo].[Items] part
    WHERE part.[ItemCode] IN ('SF-PART-ELB-A', 'SF-PART-ELB-B', 'SF-PART-ELB-C', 'SF-PART-ELB-D')
      AND NOT EXISTS (
          SELECT 1 FROM [dbo].[BomLines] b
          WHERE b.[ProducedItemId] = @Elbow AND b.[ComponentItemId] = part.[Id]
      );

    INSERT INTO [dbo].[BomLines]
        ([Quantity], [Notes], [ProducedItemId], [ComponentItemId], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 0.3, 'Final assembly wire', @Elbow, @Wire, @UoM_m, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[BomLines] b
        WHERE b.[ProducedItemId] = @Elbow AND b.[ComponentItemId] = @Wire
    );

    INSERT INTO [dbo].[RoutingSteps]
        ([Sequence], [OperationName], [EstimatedMinutes], [Notes], [ProducedItemId], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 1, 'Assembly', 30, 'Assemble all parts', @Elbow, @WS_Assembly, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[RoutingSteps] r WHERE r.[ProducedItemId] = @Elbow AND r.[Sequence] = 1);

    INSERT INTO [dbo].[RoutingSteps]
        ([Sequence], [OperationName], [EstimatedMinutes], [Notes], [ProducedItemId], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 2, 'Welding', 20, 'Weld joints', @Elbow, @WS_Welding, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[RoutingSteps] r WHERE r.[ProducedItemId] = @Elbow AND r.[Sequence] = 2);

    INSERT INTO [dbo].[RoutingSteps]
        ([Sequence], [OperationName], [EstimatedMinutes], [Notes], [ProducedItemId], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 3, 'Quality Control', 15, 'Final inspection', @Elbow, @WS_QC, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[RoutingSteps] r WHERE r.[ProducedItemId] = @Elbow AND r.[Sequence] = 3);

    -- ============================================
    -- 5. Duct (FinalProduct): Part A, B, C + Wire + Flange + Paint
    --    Routing: Assembly -> Welding -> Paint -> QC
    -- ============================================

    INSERT INTO [dbo].[Items]
        ([ItemCode], [Name], [Description], [ItemType], [StockQuantity], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'FP-DUCT-01', 'Duct Final Product', 'Straight duct section, fully assembled and painted', 'FinalProduct', 0, @UoM_pcs, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Items] WHERE [ItemCode] = 'FP-DUCT-01');

    DECLARE @Duct INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'FP-DUCT-01');

    INSERT INTO [dbo].[BomLines]
        ([Quantity], [Notes], [ProducedItemId], [ComponentItemId], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 1, CONCAT('Part ', RIGHT(part.[ItemCode], 1)), @Duct, part.[Id], @UoM_pcs, GETUTCDATE(), GETUTCDATE(), 0
    FROM [dbo].[Items] part
    WHERE part.[ItemCode] IN ('SF-PART-DUC-A', 'SF-PART-DUC-B', 'SF-PART-DUC-C')
      AND NOT EXISTS (
          SELECT 1 FROM [dbo].[BomLines] b
          WHERE b.[ProducedItemId] = @Duct AND b.[ComponentItemId] = part.[Id]
      );

    INSERT INTO [dbo].[BomLines]
        ([Quantity], [Notes], [ProducedItemId], [ComponentItemId], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 0.3, 'Final assembly wire', @Duct, @Wire, @UoM_m, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[BomLines] b
        WHERE b.[ProducedItemId] = @Duct AND b.[ComponentItemId] = @Wire
    );

    INSERT INTO [dbo].[BomLines]
        ([Quantity], [Notes], [ProducedItemId], [ComponentItemId], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 2, 'End flanges', @Duct, @Flange, @UoM_pcs, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[BomLines] b
        WHERE b.[ProducedItemId] = @Duct AND b.[ComponentItemId] = @Flange
    );

    INSERT INTO [dbo].[BomLines]
        ([Quantity], [Notes], [ProducedItemId], [ComponentItemId], [UnitOfMeasureId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 0.5, 'Protective coating', @Duct, @Paint, @UoM_lt, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[BomLines] b
        WHERE b.[ProducedItemId] = @Duct AND b.[ComponentItemId] = @Paint
    );

    INSERT INTO [dbo].[RoutingSteps]
        ([Sequence], [OperationName], [EstimatedMinutes], [Notes], [ProducedItemId], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 1, 'Assembly', 30, 'Assemble all parts', @Duct, @WS_Assembly, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[RoutingSteps] r WHERE r.[ProducedItemId] = @Duct AND r.[Sequence] = 1);

    INSERT INTO [dbo].[RoutingSteps]
        ([Sequence], [OperationName], [EstimatedMinutes], [Notes], [ProducedItemId], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 2, 'Welding', 20, 'Weld joints and flanges', @Duct, @WS_Welding, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[RoutingSteps] r WHERE r.[ProducedItemId] = @Duct AND r.[Sequence] = 2);

    INSERT INTO [dbo].[RoutingSteps]
        ([Sequence], [OperationName], [EstimatedMinutes], [Notes], [ProducedItemId], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 3, 'Painting', 15, 'Apply protective coating', @Duct, @WS_Paint, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[RoutingSteps] r WHERE r.[ProducedItemId] = @Duct AND r.[Sequence] = 3);

    INSERT INTO [dbo].[RoutingSteps]
        ([Sequence], [OperationName], [EstimatedMinutes], [Notes], [ProducedItemId], [WorkstationId], [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 4, 'Quality Control', 15, 'Final inspection', @Duct, @WS_QC, GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[RoutingSteps] r WHERE r.[ProducedItemId] = @Duct AND r.[Sequence] = 4);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;
