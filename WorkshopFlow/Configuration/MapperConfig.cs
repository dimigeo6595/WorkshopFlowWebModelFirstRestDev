using AutoMapper;
using WorkshopFlow.Models;
using WorkshopFlow.DTO;


namespace WorkshopFlow.Configuration
{
    public class MapperConfig : Profile
    {

        public MapperConfig()
        {
            CreateMap<User, UserReadOnlyDTO>()
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.Role.Name));

            CreateMap<UserInsertDTO, User>();
            CreateMap<UserUpdateDTO, User>();

        }


    }
}
