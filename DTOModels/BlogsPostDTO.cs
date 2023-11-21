using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MyBlogs.Models;

namespace MyBlogs.DTOModels;

public class BlogsPostDTO
{
    public required string Author { get; set; }
    public required string Title { get; set; }
    public required string ReadTime { get; set; } 
    public required string Intro { get; set; }
    public required string Content { get; set; }
    public string? Category { get; set; }
    public List<string>? Tags { get; set; }

    // public BlogsDTO(Blogs blogItem) =>
    // (Id, Author, ReadTime, Intro, Content, Category, Tags) = 
    // (blogItem.Id, blogItem.Author, blogItem.ReadTime, blogItem.Intro, blogItem.Content, blogItem.Category, blogItem.Tags);

    // PublishDateTime = blogItem.PublishDateTime;
}


