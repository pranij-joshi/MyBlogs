using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Threading.Tasks;


namespace MyBlogs.Models;

public class Image
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [Key]
    public required string Id { get; set; }
    public string? ImageName { get; set; }
    public string? ImagePath { get; set; }
    public string? ImageType { get; set; }
}

