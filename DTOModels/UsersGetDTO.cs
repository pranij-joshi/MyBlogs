using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyBlogs.DTOModels;

public class UsersGetDTO
{
    public string? Id { get; set; }
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Gender { get; set; } 
    public required string Phone { get; set; }
    public DateTime AddDateTime { get; set; }
}

