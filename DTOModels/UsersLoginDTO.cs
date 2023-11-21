using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyBlogs.DTOModels;

public class UsersLoginDTO
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

