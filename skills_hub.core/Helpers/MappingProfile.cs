
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using skills_hub.core.DTO;
using skills_hub.domain.Models.ManyToMany;
using skills_hub.domain.Models.User;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace skills_hub.core.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<IdentityUser, ApplicationUser>().ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UserLoginDTO, ApplicationUser>().ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UserCreateDTO, ApplicationUser>().ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UserCreateDTO, BaseUserInfo>().ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<ApplicationUser, Teacher>().ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<ApplicationUser, Student>().ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


        CreateMap<Student, LessonStudent>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Student, opt => opt.MapFrom(src => src));

        //CreateMap<PaymentCategory, DurationType>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DurationName)).ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.DurationValue));
        //CreateMap<DurationType, PaymentCategory>().ForMember(dest => dest.DurationName, opt => opt.MapFrom(src => src.Name)).ForMember(dest => dest.DurationValue, opt => opt.MapFrom(src => src.Value));
        /*
        CreateMap<LessonStudent, Student>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest, opt => opt.MapFrom(src => src.Student));
        */
    }

}
