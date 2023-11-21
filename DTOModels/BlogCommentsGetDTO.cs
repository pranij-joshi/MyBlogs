using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyBlogs.DTOModels;

public class BlogCommentsGetDTO
{
    public required string Id { get; set; }
    public required string Comments { get; set; }
    public required string BlogId { get; set; }
    public required string UserId { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime CommentDateTIme { get; set; }

}