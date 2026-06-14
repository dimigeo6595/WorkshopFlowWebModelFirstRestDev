using AutoMapper;
using WorkshopFlow.DTO;
using WorkshopFlow.Models;

namespace WorkshopFlow.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            // User mappings
            CreateMap<User, UserReadOnlyDTO>()
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.Role.Name));
            CreateMap<UserInsertDTO, User>();
            CreateMap<UserUpdateDTO, User>();

            // Item mappings
            CreateMap<Item, ItemReadOnlyDTO>()
                .ForMember(dest => dest.ItemType,
                    opt => opt.MapFrom(src => src.ItemType.ToString()))
                .ForMember(dest => dest.IsManufactured,
                    opt => opt.MapFrom(src => src.IsManufactured))
                .ForMember(dest => dest.UnitOfMeasureSymbol,
                    opt => opt.MapFrom(src => src.UnitOfMeasure.Symbol))
                .ForMember(dest => dest.WeightUoMSymbol,
                    opt => opt.MapFrom(src => src.WeightUoM != null
                        ? src.WeightUoM.Symbol
                        : null));

            CreateMap<ItemInsertDTO, Item>()
                .ForMember(dest => dest.ItemType,
                    opt => opt.MapFrom(src => src.ItemType!.Value))
                .ForMember(dest => dest.UnitOfMeasureId,
                    opt => opt.MapFrom(src => src.UnitOfMeasureId!.Value));

            CreateMap<ItemUpdateDTO, Item>()
                .ForMember(dest => dest.ItemType,
                    opt => opt.MapFrom(src => src.ItemType!.Value))
                .ForMember(dest => dest.UnitOfMeasureId,
                    opt => opt.MapFrom(src => src.UnitOfMeasureId!.Value));

            // BomLine mappings
            CreateMap<BomLine, BomLineReadOnlyDTO>()
                .ForMember(dest => dest.ComponentItemCode,
                    opt => opt.MapFrom(src => src.ComponentItem.ItemCode))
                .ForMember(dest => dest.ComponentItemName,
                    opt => opt.MapFrom(src => src.ComponentItem.Name))
                .ForMember(dest => dest.UnitOfMeasureSymbol,
                    opt => opt.MapFrom(src => src.UnitOfMeasure.Symbol));

            CreateMap<BomLineInsertDTO, BomLine>()
                .ForMember(dest => dest.ComponentItemId,
                    opt => opt.MapFrom(src => src.ComponentItemId!.Value))
                .ForMember(dest => dest.UnitOfMeasureId,
                    opt => opt.MapFrom(src => src.UnitOfMeasureId!.Value));

            CreateMap<BomLineUpdateDTO, BomLine>();

            // Workstation mappings
            CreateMap<Workstation, WorkstationReadOnlyDTO>();

            CreateMap<WorkstationInsertDTO, Workstation>();

            CreateMap<WorkstationUpdateDTO, Workstation>();

            // Machine mappings
            CreateMap<Machine, MachineReadOnlyDTO>()
                .ForMember(dest => dest.WorkstationCode,
                    opt => opt.MapFrom(src => src.Workstation.Code))
                .ForMember(dest => dest.WorkstationName,
                    opt => opt.MapFrom(src => src.Workstation.Name));

            CreateMap<MachineInsertDTO, Machine>()
                .ForMember(dest => dest.WorkstationId,
                    opt => opt.MapFrom(src => src.WorkstationId!.Value));

            CreateMap<MachineUpdateDTO, Machine>()
                .ForMember(dest => dest.WorkstationId,
                    opt => opt.MapFrom(src => src.WorkstationId!.Value));

            // RoutingStep mappings
            CreateMap<RoutingStep, RoutingStepReadOnlyDTO>()
                .ForMember(dest => dest.WorkstationCode,
                    opt => opt.MapFrom(src => src.Workstation.Code))
                .ForMember(dest => dest.WorkstationName,
                    opt => opt.MapFrom(src => src.Workstation.Name))
                .ForMember(dest => dest.MachineCode,
                    opt => opt.MapFrom(src => src.Machine != null ? src.Machine.Code : null))
                .ForMember(dest => dest.MachineName,
                    opt => opt.MapFrom(src => src.Machine != null ? src.Machine.Name : null));

            CreateMap<RoutingStepInsertDTO, RoutingStep>()
                .ForMember(dest => dest.WorkstationId,
                    opt => opt.MapFrom(src => src.WorkstationId!.Value))
                .ForMember(dest => dest.Sequence,
                    opt => opt.MapFrom(src => src.Sequence!.Value))
                .ForMember(dest => dest.EstimatedMinutes,
                    opt => opt.MapFrom(src => src.EstimatedMinutes!.Value));

            CreateMap<RoutingStepUpdateDTO, RoutingStep>()
                .ForMember(dest => dest.WorkstationId,
                    opt => opt.MapFrom(src => src.WorkstationId!.Value))
                .ForMember(dest => dest.Sequence,
                    opt => opt.MapFrom(src => src.Sequence!.Value))
                .ForMember(dest => dest.EstimatedMinutes,
                    opt => opt.MapFrom(src => src.EstimatedMinutes!.Value));

            // WorkOrder mappings
            CreateMap<WorkOrder, WorkOrderReadOnlyDTO>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ProducedItemCode,
                    opt => opt.MapFrom(src => src.ProducedItem.ItemCode))
                .ForMember(dest => dest.ProducedItemName,
                    opt => opt.MapFrom(src => src.ProducedItem.Name))
                .ForMember(dest => dest.UnitOfMeasureSymbol,
                    opt => opt.MapFrom(src => src.ProducedItem.UnitOfMeasure.Symbol))
                .ForMember(dest => dest.CreatedByUsername,
                    opt => opt.MapFrom(src => src.CreatedBy.Username))
                // Υπολογισμός προόδου operations
                .ForMember(dest => dest.TotalOperations,
                    opt => opt.MapFrom(src => src.Operations.Count(o => !o.IsDeleted)))
                .ForMember(dest => dest.CompletedOperations,
                    opt => opt.MapFrom(src => src.Operations
                        .Count(o => !o.IsDeleted && o.Status == WorkOrderOperationStatus.Completed)));

            // WorkOrderInsertDTO → WorkOrder — τα υπόλοιπα fields γεμίζουν στο service
            CreateMap<WorkOrderInsertDTO, WorkOrder>()
                .ForMember(dest => dest.ProducedItemId,
                    opt => opt.MapFrom(src => src.ProducedItemId!.Value))
                .ForMember(dest => dest.Quantity,
                    opt => opt.MapFrom(src => src.Quantity!.Value));

            // WorkOrderUpdateDTO → WorkOrder — μόνο τα editable fields
            CreateMap<WorkOrderUpdateDTO, WorkOrder>();

            // WorkOrderOperation mappings
            CreateMap<WorkOrderOperation, WorkOrderOperationReadOnlyDTO>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.OperationName,
                    opt => opt.MapFrom(src => src.RoutingStep.OperationName))
                .ForMember(dest => dest.WorkstationCode,
                    opt => opt.MapFrom(src => src.RoutingStep.Workstation.Code))
                .ForMember(dest => dest.WorkstationName,
                    opt => opt.MapFrom(src => src.RoutingStep.Workstation.Name))
                .ForMember(dest => dest.MachineCode,
                    opt => opt.MapFrom(src => src.RoutingStep.Machine != null
                        ? src.RoutingStep.Machine.Code : null))
                .ForMember(dest => dest.MachineName,
                    opt => opt.MapFrom(src => src.RoutingStep.Machine != null
                        ? src.RoutingStep.Machine.Name : null))
                .ForMember(dest => dest.AssignedToUsername,
                    opt => opt.MapFrom(src => src.AssignedTo != null
                        ? src.AssignedTo.Username : null));

            // InventoryTransaction mappings
            CreateMap<InventoryTransaction, InventoryTransactionReadOnlyDTO>()
                .ForMember(dest => dest.TransactionType,
                    opt => opt.MapFrom(src => src.TransactionType.ToString()))
                .ForMember(dest => dest.ItemCode,
                    opt => opt.MapFrom(src => src.Item.ItemCode))
                .ForMember(dest => dest.ItemName,
                    opt => opt.MapFrom(src => src.Item.Name))
                .ForMember(dest => dest.WorkOrderCode,
                    opt => opt.MapFrom(src => src.WorkOrder != null
                        ? src.WorkOrder.WorkOrderCode : null))
                .ForMember(dest => dest.CreatedByUsername,
                    opt => opt.MapFrom(src => src.CreatedBy.Username));

            CreateMap<InventoryTransactionInsertDTO, InventoryTransaction>()
                .ForMember(dest => dest.ItemId,
                    opt => opt.MapFrom(src => src.ItemId!.Value))
                .ForMember(dest => dest.TransactionType,
                    opt => opt.MapFrom(src => src.TransactionType!.Value))
                .ForMember(dest => dest.Quantity,
                    opt => opt.MapFrom(src => src.Quantity!.Value));





        }
    }
}
