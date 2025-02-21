using AutoMapper;
using Address = Dhbw.ThesisManager.Domain.Data.Entities.Address;
using Comment = Dhbw.ThesisManager.Domain.Data.Entities.Comment;
using InCompanySupervisor = Dhbw.ThesisManager.Domain.Data.Entities.InCompanySupervisor;
using OperationalLocation = Dhbw.ThesisManager.Domain.Data.Entities.OperationalLocation;
using PartnerCompany = Dhbw.ThesisManager.Domain.Data.Entities.PartnerCompany;
using Student = Dhbw.ThesisManager.Domain.Data.Entities.Student;
using Thesis = Dhbw.ThesisManager.Domain.Data.Entities.Thesis;

namespace Dhbw.ThesisManager.Api.Mapping;
using DbEntities = Domain.Data.Entities;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Student, Models.Student>()
            .ReverseMap();
            
        CreateMap<Address, Models.Address>()
            .ReverseMap();
            
        CreateMap<Thesis, Models.Thesis>()
            .ForMember(dest => dest.PreparationPeriod, opt => opt.MapFrom(src => new Models.PreparationPeriod 
            { 
                From = src.PreparationPeriodFrom.ToString("O"),
                To = src.PreparationPeriodTo.ToString("O")
            }))
            .ReverseMap()
            .ForMember(dest => dest.PreparationPeriodFrom, opt => opt.MapFrom(src => DateTime.Parse(src.PreparationPeriod.From)))
            .ForMember(dest => dest.PreparationPeriodTo, opt => opt.MapFrom(src => DateTime.Parse(src.PreparationPeriod.To)));
            
        CreateMap<PartnerCompany, Models.PartnerCompany>()
            .ReverseMap();
            
        CreateMap<OperationalLocation, Models.OperationalLocation>()
            .ReverseMap();
            
        CreateMap<InCompanySupervisor, Models.InCompanySupervisor>()
            .ReverseMap();
            
        CreateMap<Comment, Models.Comment>()
            .ReverseMap();
            
        CreateMap<Thesis, Models.ThesisSummary>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Topic))
            .ForMember(dest => dest.StudentFirstName, opt => opt.MapFrom(src => src.Student.FirstName))
            .ForMember(dest => dest.StudentLastName, opt => opt.MapFrom(src => src.Student.LastName));
    }
}
