using AutoMapper;
using MyBlogs.Models;
using MyBlogs.DTOModels;

namespace MyBlogs.Profiles;

public class BlogProfile : Profile
{
     public BlogProfile()
     {
        CreateMap<BlogsPostDTO, Blogs>();
        CreateMap<Blogs, BlogsGetDTO>();
     }
} 