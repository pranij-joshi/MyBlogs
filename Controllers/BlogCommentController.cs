using AutoMapper;
using MyBlogs.Models;
using MyBlogs.Services;
using MyBlogs.DTOModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Bson.IO;

namespace MyBlogs.Controllers;

[ApiController]
[Route("api/comments/")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class BlogCommentController : ControllerBase
{
    private readonly BlogsService _blogsService;
    private readonly BlogCommentService _blogCommentService;
    private readonly UsersService _usersService;
    private readonly JwtService _jwtService;
    private readonly IMapper _mapper;

    public BlogCommentController(BlogsService blogsService, UsersService usersService, BlogCommentService blogCommentService, JwtService jwtService, IMapper mapper)
    {
        _blogsService = blogsService;
        _usersService = usersService;
        _blogCommentService = blogCommentService;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    
    // [HttpGet]
    // public async Task<IActionResult> Get()
    // {
    //     var blog = await _blogsService.GetAsync();
    //     if (blog.Any())
    //         return Ok(blog);
    //     return NotFound();
    // }

    [HttpGet("get/{id:length(24)}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(string id)
    {
        var comments = await _blogCommentService.GetBlogComment(id);

        if (comments is null)
        {
            return NotFound();
        }

        return Ok(comments);
    }
    
    [HttpPost("postComments/{id:length(24)}")]
    public async Task<IActionResult> PostComment(string id, [FromBody] BlogCommentsPostDTO commentBody)
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        // Check if the token is missing or empty
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("No token provided.");
        }

        var userId = _jwtService.DecodeJwtAccessToken(token);   
        var user = await _usersService.GetAsync(userId);
        var blog = await _blogsService.GetAsync(id);

        if (user == null || blog == null)
        {
            return NotFound("User or Blog not found.");
        }

        try{
            var result = await _blogCommentService.CreateComment(blog, user, commentBody);
            if (result == null)
            {
                return StatusCode(401, new { message = "Error posting your comment"});
            }
            else
            {
                
                return Ok( new{ Message =  "Blog Comment posted successfully"});
            }
        }
        catch (Exception ex)
        {
            return StatusCode(401, new { message = "Error posting your comment", error = ex});
        }

    }


    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] string updatedComment)
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        // Check if the token is missing or empty
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("No token provided.");
        }

        var userId = _jwtService.DecodeJwtAccessToken(token);
        var existingBlog = await _blogCommentService.GetComment(id);

        if (existingBlog is null)
            return NotFound();

        if (existingBlog.User.Id == userId)
        {
            existingBlog.Comments = updatedComment;
            await _blogCommentService.UpdateComment(existingBlog);
            return Ok("Successfully updated");
        }
        else
        {
            return Unauthorized();
        }        
    }

    [HttpDelete("delete/{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        // Check if the token is missing or empty
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("No token provided.");
        }

        var userId = _jwtService.DecodeJwtAccessToken(token);

        // var existingBlog = await _blogCommentService.GetComment(id);
        
        // if (existingBlog is null)
        //     return NotFound();

        await _blogCommentService.RemoveComment(id, userId);

        return Ok("Successfully Deleted");   
    }
}

