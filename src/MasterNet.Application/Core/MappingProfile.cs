using AutoMapper;
using MasterNet.Application.Courses.CourseGet;
using MasterNet.Application.Instructors.InstructorsGet;
using MasterNet.Application.Photos.PhotosGet;
using MasterNet.Application.Prices.PricesGet;
using MasterNet.Application.Qualifications.QualificationsGet;
using MasterNet.Domain.Entities;

namespace MasterNet.Application.Core;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Course, CourseResponse>().ReverseMap();
        CreateMap<Instructor, InstructorResponse>().ReverseMap();
        CreateMap<Price, PriceResponse>().ReverseMap();
        CreateMap<Photo, PhotoResponse>().ReverseMap();
        CreateMap<Qualification, QualificationResponse>()
                .ForMember(dest => dest.CourseTitle, src => src.MapFrom(doc => doc.Course!.Title));
    }
}