using AutoMapper;
using Dhbw.ThesisManager.Api.Data.Entities;

namespace Dhbw.ThesisManager.Api.Data.Mapping;
using DbEntities = Dhbw.ThesisManager.Api.Data.Entities;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<DbEntities.Student, Models.Student>()
            .ReverseMap();
            
        CreateMap<DbEntities.Address, Models.Address>()
            .ReverseMap();
            
        CreateMap<DbEntities.Thesis, Models.Thesis>()
            .ForMember(dest => dest.PreparationPeriod, opt => opt.MapFrom(src => new Models.PreparationPeriod 
            { 
                From = src.PreparationPeriodFrom.ToString("O"),
                To = src.PreparationPeriodTo.ToString("O")
            }))
            .ReverseMap()
            .ForMember(dest => dest.PreparationPeriodFrom, opt => opt.MapFrom(src => DateTime.Parse(src.PreparationPeriod.From)))
            .ForMember(dest => dest.PreparationPeriodTo, opt => opt.MapFrom(src => DateTime.Parse(src.PreparationPeriod.To)));
            
        CreateMap<DbEntities.PartnerCompany, Models.PartnerCompany>()
            .ReverseMap();
            
        CreateMap<DbEntities.OperationalLocation, Models.OperationalLocation>()
            .ReverseMap();
            
        CreateMap<DbEntities.InCompanySupervisor, Models.InCompanySupervisor>()
            .ReverseMap();
            
        CreateMap<DbEntities.Comment, Models.Comment>()
            .ReverseMap();
            
        CreateMap<DbEntities.Thesis, Models.ThesisSummary>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Topic))
            .ForMember(dest => dest.StudentFirstName, opt => opt.MapFrom(src => src.Student.FirstName))
            .ForMember(dest => dest.StudentLastName, opt => opt.MapFrom(src => src.Student.LastName));
    }
}
