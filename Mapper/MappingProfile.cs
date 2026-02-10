using AutoMapper;
using HRMS_Backend.Entities.JobListing;
using HRMS_Backend.Entities.TravelandExpense;
using HRMS_Backend.Model.JobListing;
using HRMS_Backend.Model.TravelandExpense;

namespace HRMS_Backend.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<Jobs, JobResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            CreateMap<JobCreateUpdateDto , Jobs>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<TravelPlan, TravelResponseDto>();

        }
    }
}
