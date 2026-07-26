-- WorkshopFlow - Seed Work Orders Q3 2026
-- Τρέξε αυτό απευθείας στη βάση WorkshopFlow

DECLARE @AdminUserId INT = (SELECT [Id] FROM [dbo].[Users] WHERE [Username] = 'admin')

DECLARE @Elbow    INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'FP-ELBOW-01')
DECLARE @Duct     INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'FP-DUCT-01')
DECLARE @CylElbA  INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'SF-CYL-ELB-A')
DECLARE @CylElbB  INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'SF-CYL-ELB-B')
DECLARE @CylElbC  INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'SF-CYL-ELB-C')
DECLARE @CylElbD  INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'SF-CYL-ELB-D')
DECLARE @CylDucA  INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'SF-CYL-DUC-A')
DECLARE @CylDucB  INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'SF-CYL-DUC-B')
DECLARE @CylDucC  INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'SF-CYL-DUC-C')
DECLARE @PartElbA INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'SF-PART-ELB-A')
DECLARE @PartElbB INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'SF-PART-ELB-B')
DECLARE @PartElbC INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'SF-PART-ELB-C')
DECLARE @PartElbD INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'SF-PART-ELB-D')
DECLARE @PartDucA INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'SF-PART-DUC-A')
DECLARE @PartDucB INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'SF-PART-DUC-B')
DECLARE @PartDucC INT = (SELECT [Id] FROM [dbo].[Items] WHERE [ItemCode] = 'SF-PART-DUC-C')

-- ============================================
-- ΙΟΥΝΙΟΣ 2026 — Completed + Cancelled
-- ============================================

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202606-200',5,'Completed','2026-06-01','2026-06-04','Elbow batch June #1',@Elbow,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202606-200')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202606-201',8,'Completed','2026-06-02','2026-06-06','Cylinders ELB-A batch',@CylElbA,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202606-201')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202606-202',6,'Completed','2026-06-05','2026-06-09','Parts ELB-A batch',@PartElbA,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202606-202')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202606-203',4,'Completed','2026-06-08','2026-06-12','Duct batch June #1',@Duct,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202606-203')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202606-204',10,'Completed','2026-06-10','2026-06-15','Cylinders DUC-A batch',@CylDucA,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202606-204')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202606-205',3,'Cancelled','2026-06-12','2026-06-16','Cancelled - supply issue',@Elbow,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202606-205')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202606-206',7,'Completed','2026-06-15','2026-06-19','Parts ELB-B batch',@PartElbB,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202606-206')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202606-207',5,'Completed','2026-06-17','2026-06-21','Cylinders ELB-B batch',@CylElbB,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202606-207')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202606-208',4,'Completed','2026-06-20','2026-06-24','Parts DUC-A batch',@PartDucA,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202606-208')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202606-209',6,'Completed','2026-06-23','2026-06-27','Duct batch June #2',@Duct,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202606-209')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202606-210',2,'Cancelled','2026-06-25','2026-06-29','Cancelled - redesign',@Duct,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202606-210')

-- ============================================
-- ΑΡΧΕΣ ΙΟΥΛΙΟΥ — Completed
-- ============================================

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202607-200',8,'Completed','2026-07-01','2026-07-05','Elbow batch July #1',@Elbow,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202607-200')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202607-201',12,'Completed','2026-07-03','2026-07-08','Cylinders ELB-C batch',@CylElbC,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202607-201')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202607-202',5,'Completed','2026-07-07','2026-07-11','Parts ELB-C batch',@PartElbC,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202607-202')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202607-203',6,'Completed','2026-07-10','2026-07-15','Cylinders DUC-B batch',@CylDucB,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202607-203')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202607-204',4,'Completed','2026-07-14','2026-07-18','Parts DUC-B batch',@PartDucB,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202607-204')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202607-205',3,'Completed','2026-07-17','2026-07-22','Duct batch July #1',@Duct,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202607-205')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202607-206',7,'Completed','2026-07-20','2026-07-24','Cylinders ELB-D batch',@CylElbD,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202607-206')

-- ============================================
-- ΤΩΡΑ (γύρω από 26/7) — InProgress + Released
-- ============================================

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202607-207',4,'InProgress','2026-07-23','2026-07-29','Parts ELB-D - in production',@PartElbD,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202607-207')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202607-208',6,'InProgress','2026-07-24','2026-07-30','Elbow batch July #2',@Elbow,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202607-208')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202607-209',5,'InProgress','2026-07-25','2026-07-31','Cylinders DUC-C batch',@CylDucC,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202607-209')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202607-210',3,'Released','2026-07-26','2026-08-01','Parts DUC-C - ready to start',@PartDucC,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202607-210')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202607-211',8,'Released','2026-07-27','2026-08-03','Duct batch July #2',@Duct,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202607-211')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202607-212',4,'Released','2026-07-28','2026-08-05','Cylinders ELB-A batch #2',@CylElbA,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202607-212')

-- ============================================
-- ΑΥΓΟΥΣΤΟΣ 2026 — Draft
-- ============================================

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202608-200',5,'Draft','2026-08-01','2026-08-06','Parts ELB-A batch #2',@PartElbA,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202608-200')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202608-201',6,'Draft','2026-08-04','2026-08-09','Elbow batch Aug #1',@Elbow,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202608-201')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202608-202',4,'Draft','2026-08-05','2026-08-11','Cylinders ELB-B batch #2',@CylElbB,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202608-202')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202608-203',7,'Draft','2026-08-08','2026-08-14','Duct batch Aug #1',@Duct,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202608-203')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202608-204',3,'Draft','2026-08-11','2026-08-16','Parts ELB-B batch #2',@PartElbB,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202608-204')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202608-205',9,'Draft','2026-08-13','2026-08-19','Cylinders DUC-A batch #2',@CylDucA,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202608-205')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202608-206',5,'Draft','2026-08-17','2026-08-22','Parts DUC-A batch #2',@PartDucA,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202608-206')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202608-207',4,'Draft','2026-08-19','2026-08-25','Elbow batch Aug #2',@Elbow,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202608-207')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202608-208',6,'Draft','2026-08-22','2026-08-28','Cylinders ELB-C batch #2',@CylElbC,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202608-208')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202608-209',8,'Draft','2026-08-25','2026-08-31','Duct batch Aug #2',@Duct,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202608-209')

-- ============================================
-- ΣΕΠΤΕΜΒΡΙΟΣ 2026 — Draft
-- ============================================

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202609-200',10,'Draft','2026-09-01','2026-09-06','Cylinders ELB-D batch #2',@CylElbD,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202609-200')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202609-201',5,'Draft','2026-09-03','2026-09-09','Parts ELB-D batch #2',@PartElbD,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202609-201')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202609-202',6,'Draft','2026-09-07','2026-09-13','Elbow batch Sep #1',@Elbow,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202609-202')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202609-203',4,'Draft','2026-09-09','2026-09-15','Cylinders DUC-B batch #2',@CylDucB,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202609-203')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202609-204',7,'Draft','2026-09-12','2026-09-18','Parts DUC-B batch #2',@PartDucB,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202609-204')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202609-205',5,'Draft','2026-09-15','2026-09-21','Duct batch Sep #1',@Duct,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202609-205')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202609-206',3,'Draft','2026-09-17','2026-09-23','Cylinders DUC-C batch #2',@CylDucC,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202609-206')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202609-207',8,'Draft','2026-09-20','2026-09-26','Parts DUC-C batch #2',@PartDucC,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202609-207')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202609-208',6,'Draft','2026-09-23','2026-09-28','Elbow batch Sep #2',@Elbow,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202609-208')

INSERT INTO [dbo].[WorkOrders] ([WorkOrderCode],[Quantity],[Status],[PlannedStartDate],[PlannedEndDate],[Notes],[ProducedItemId],[CreatedByUserId],[InsertedAt],[ModifiedAt],[IsDeleted])
SELECT 'WO-202609-209',10,'Draft','2026-09-25','2026-09-30','Duct batch Sep #2 - Q3 final',@Duct,@AdminUserId,GETUTCDATE(),GETUTCDATE(),0
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[WorkOrders] WHERE [WorkOrderCode]='WO-202609-209')

PRINT 'Done: ~50 work orders inserted for Q3 2026.'