using AutoMapper;
using MyBlogs.Models;
using MyBlogs.DTOModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace MyBlogs.Profiles;

public class UsersProfile : Profile
{
   public UsersProfile()
   {
      byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
      CreateMap<UsersPostDTO, Users>()
         .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
         .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
         .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
         .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
         .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
         .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
         .ForMember(dest => dest.Salt, opt => opt.MapFrom(src => salt))
         // .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
         .ForMember(dest => dest.Password, opt => opt.MapFrom(src => EncryptPassword(src.Password, salt)))
         .ForMember(dest => dest.ProfilePic, opt => opt.MapFrom(src => ImageSaving(src.Image, "ProfilePics")))
         .ForMember(dest => dest.AddDateTime, opt => opt.MapFrom(src => DateTime.Now));
      CreateMap<Users, UsersGetDTO>();
      CreateMap<Users, UsersLoginDTO>();
   }

   public string EncryptPassword(string password, byte[] salt)
   {
      // string hashed = password;
      var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
      password: password!,
      salt: salt,
      prf: KeyDerivationPrf.HMACSHA256,
      iterationCount: 100000,
      numBytesRequested: 256 / 8));
      return hashed;
   }

   public string ImageSaving(IFormFile image, string folder)
    {
        var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;

        var uploadsFolder = Path.Combine("D:\\AppboxTech Internship\\playground\\MyBlogs\\Uploads", folder);

        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            image.CopyToAsync(fileStream);
        }
        return filePath;
    }
} 

