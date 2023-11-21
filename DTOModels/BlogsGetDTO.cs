using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyBlogs.DTOModels;

public class BlogsGetDTO
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Author")]
    public required string Author { get; set; }
    public required string Title { get; set; }
    [BsonElement("Read Time")]
    public required string ReadTime { get; set; } 
    public required string Intro { get; set; }
    public required string Content { get; set; }
    public DateTime PublishDateTime { get; set; }
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public bool? IsLike { get; set; } = false;
    public bool? IsLikeCount { get; set; } = false;
    public bool? IsBookmark { get; set; } = false;


    // public BlogsGetDTO(Blogs blogItem) =>
    // (Id, Author, ReadTime, Intro, Content, Category, Tags, PublishDateTime) = 
    // (blogItem.Id, blogItem.Author, blogItem.ReadTime, blogItem.Intro, blogItem.Content, blogItem.Category, blogItem.Tags, blogItem.PublishDateTime);

    // PublishDateTime = blogItem.PublishDateTime;
}


