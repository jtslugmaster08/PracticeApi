using AutoMapper;
using PracticeModel.Dto;
using PracticeModel.Entities;

namespace PracticeApi.Helpers
{
    public class MappingProfile:Profile
    {
        public MappingProfile() {
            CreateMap<SigninDto, BaseUser>();
        }
    }
}
