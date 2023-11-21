using AutoMapper;
using MyBlogs.Models;
using MyBlogs.DTOModels;

namespace MyBlogs.Profiles;

public class BlogCommentProfile : Profile
{
     public BlogCommentProfile()
     {

          // .ForMember(dest => dest.CommentDateTIme, opt => opt.MapFrom((src, dest, args, context) =>{
          //      return context.Items[Constants.Like];
          //   }));
          CreateMap<BlogCommentsPostDTO, BlogComments>()
               .ForMember(dest => dest.CommentDateTIme, opt => opt.MapFrom(src => DateTime.Now))
               .ForMember(dest => dest.User, opt => opt.MapFrom((src, dest, args, context) => 
                    {return context.Items["user"];}))
               .ForMember(dest => dest.Blog, opt => opt.MapFrom((src, dest, args, context) => 
                    {return context.Items["blog"];}));
          
          
          CreateMap<BlogComments, BlogCommentsGetDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.BlogId, opt => opt.MapFrom(src => src.Blog.Id))
               .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
               .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
               .ForMember(dest => dest.CommentDateTIme, opt => opt.MapFrom(src => src.CommentDateTIme)); 
     }
} 
