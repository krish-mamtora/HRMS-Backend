using AutoMapper;
using HRMS_Backend.Entities.FixEntityUserProfile;
using HRMS_Backend.Entities.Games_Scheduling;
using HRMS_Backend.Entities.GamesScheduling;
using HRMS_Backend.Entities.JobListing;
using HRMS_Backend.Entities.TravelandExpense;
using HRMS_Backend.Model.DtoUserProfile;
using HRMS_Backend.Model.GameScheduling;
using HRMS_Backend.Model.JobListing;
using HRMS_Backend.Model.TravelandExpense;

namespace HRMS_Backend.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<TravelPlan, TravelResponseDto>();
            CreateMap<Jobs, JobResponseDto>();
            CreateMap<Referals, JobRefferalResponseDto>();
            CreateMap<TravelAssignment, TravelAssignmentDisplayDto>();
            CreateMap<UserProfileCreateUpdateDto, UserProfile>();
            CreateMap<UserProfile, UserProfileDisplayDto>();
            CreateMap<UserProfileDisplayDto, UserProfile>();
            CreateMap<TravelExpense, ExpenseDisplayDto>();
            CreateMap<ExpenseCreateUpdateDto, TravelExpense>();
            CreateMap<Expenses, ExpenseDisplayDto>();
            CreateMap<ExpenseProof, ExpenseProofDisplayDto>();
            CreateMap<TravelDocuments, TravelDocumentsDisplayDto>();
            CreateMap<Games, GamesDisplayDto>();
            CreateMap<GameConfiguration, GameConfigDisplayDto>();
            CreateMap<GameSlots, GameSlotsDisplayDto>();
            CreateMap<EmployeeCycleStats, EmployeeCycleStatsDisplayDto>();
        }
    }
}
