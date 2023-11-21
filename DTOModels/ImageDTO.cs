using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyBlogs.DTOModels;

public class ImageDTO
{
    public required string ImageName { get; set; }
    public required IFormFile ImageFile { get; set; }
    public required string ImageType { get; set; }
}



