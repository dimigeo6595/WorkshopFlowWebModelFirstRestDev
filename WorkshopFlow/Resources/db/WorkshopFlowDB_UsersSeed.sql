BEGIN TRY
    BEGIN TRANSACTION;

    -- ADMIN
    INSERT INTO [dbo].[Users] 
        ([Username], [Email], [Password], [Firstname], [Lastname], [RoleId], 
         [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'admin', 'admin@workshopflow.gr',
        '$2b$11$bMlu1tBRqAgIpi3MppQn2O2BzmXRF.MhJVJhudcvjmb8dy5/UZ6Z.',
        'Admin', 'User',
        (SELECT [Id] FROM [dbo].[Roles] WHERE [Name] = 'ADMIN'),
        GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[Users] WHERE [Username] = 'admin'
    );

    -- PRODUCTION_ENGINEER
    INSERT INTO [dbo].[Users] 
        ([Username], [Email], [Password], [Firstname], [Lastname], [RoleId], 
         [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'engineer1', 'engineer1@workshopflow.gr',
        '$2b$11$TepvLLy6iXUOidh/TXlqn.JqwZsJzB5R51OM54.pVPk0caBAiacCW',
        'Nikos', 'Papadopoulos',
        (SELECT [Id] FROM [dbo].[Roles] WHERE [Name] = 'PRODUCTION_ENGINEER'),
        GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[Users] WHERE [Username] = 'engineer1'
    );

    -- OPERATOR
    INSERT INTO [dbo].[Users] 
        ([Username], [Email], [Password], [Firstname], [Lastname], [RoleId], 
         [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'operator1', 'operator1@workshopflow.gr',
        '$2b$11$79IY386LeqMtqnWjfHhm5.fVlMsmEN1.dUzrNLLbXrGC1WU4qt6CO',
        'Giorgos', 'Ioannou',
        (SELECT [Id] FROM [dbo].[Roles] WHERE [Name] = 'OPERATOR'),
        GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[Users] WHERE [Username] = 'operator1'
    );

    -- WAREHOUSE_MANAGER
    INSERT INTO [dbo].[Users] 
        ([Username], [Email], [Password], [Firstname], [Lastname], [RoleId], 
         [InsertedAt], [ModifiedAt], [IsDeleted])
    SELECT 'warehouse1', 'warehouse1@workshopflow.gr',
        '$2b$11$slY.pU/t.rtP5MwH8yJaKOvuTTLyaORr7KAsrRS4efD6dGX7FAjRS',
        'Maria', 'Georgiou',
        (SELECT [Id] FROM [dbo].[Roles] WHERE [Name] = 'WAREHOUSE_MANAGER'),
        GETUTCDATE(), GETUTCDATE(), 0
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[Users] WHERE [Username] = 'warehouse1'
    );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;