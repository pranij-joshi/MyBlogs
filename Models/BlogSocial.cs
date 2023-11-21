using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MyBlogs.DTOModels;

namespace MyBlogs.Models;

public class BlogLikeDislike
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public required BlogsGetDTO Blog { get; set; }
    public required UsersGetDTO User { get; set; } 
}

public class BlogBookmark
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public required BlogsGetDTO Blog { get; set; }
    public required UsersGetDTO User { get; set; }
}

