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

        }
    }
}
