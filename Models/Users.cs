using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Threading.Tasks;


namespace MyBlogs.Models;

public class Users
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [Key]
    public required string Id { get; set; }
    [Required(ErrorMessage = "Username is required.")]
    public required string Username { get; set; }
    [Required(ErrorMessage = "Firstname is required.")]
    public required string FirstName { get; set; }
    [Required(ErrorMessage = "Lastname is required.")]
    public required string LastName { get; set; }
    [Required(ErrorMessage = "Email is required.")]
    public required string Email { get; set; }
    public string? Gender { get; set; }
    public string? ProfilePic { get; set; }
    public required string Phone { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime AddDateTime { get; set; }
    public required byte[] Salt { get; set; }
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password is required.")]
    public required string Password { get; set; }
}

