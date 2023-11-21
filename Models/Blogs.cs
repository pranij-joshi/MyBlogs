using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MyBlogs.DTOModels;

namespace MyBlogs.Models;

public class Blogs
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public UsersGetDTO User { get; set; }
    public required string Author { get; set; }
    public required string Title { get; set; }
    public required string ReadTime { get; set; } 
    public required string Intro { get; set; }
    public required string Content { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime PublishDateTime { get; set; }
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public bool? IsLike { get; set; } = false;
    public bool? IsLikeCount { get; set; } = false;
    public bool? IsBookmark { get; set; } = false;
}

