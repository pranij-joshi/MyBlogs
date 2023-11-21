using MyBlogs.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace MyBlogs.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/social")]
public class BlogSocialController : ControllerBase
{
    private readonly UsersService _usersService;
    private readonly BlogsService _blogService;
    private readonly JwtService _jwtService;
    private readonly BlogSocialService _blogSocialService;

    public BlogSocialController(UsersService usersService, BlogsService blogService, JwtService jwtService, BlogSocialService blogSocialService)
    {
        _usersService = usersService;
        _blogService = blogService;
        _jwtService = jwtService;
        _blogSocialService = blogSocialService;
    }

    [HttpPost("likeDislike/{id}")]
    public async Task<IActionResult> LikeDislike(string id)
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        // Check if the token is missing or empty
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("No token provided.");
        }

        var userId = _jwtService.DecodeJwtAccessToken(token);   
        var user = await _usersService.GetAsync(userId);
        var blog = await _blogService.GetAsync(id);

        if (user == null || blog == null)
        {
            return NotFound("User or Blog not found.");
        }

        try{
            var result = await _blogSocialService.GetLikeDislikeAsync(blog, user);
            if (result == false)
            {
                await _blogSocialService.CreateLikeDislikeAsync(blog, user);
                return Ok("Blog Successfully liked");
            }
            else
            {
                await _blogSocialService.RemoveLikeDislikeAsync(blog, user);
                return Ok("Blog Successfully disliked");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(401, new { message = "Error liking or dislikng blog", error = ex});
        }

    }

    [HttpPost("bookmark/{id}")]
    public async Task<IActionResult> Bookmarks(string id)
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        // Check if the token is missing or empty
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("No token provided.");
        }

        var userId = _jwtService.DecodeJwtAccessToken(token);   
        var user = await _usersService.GetAsync(userId);
        var blog = await _blogService.GetAsync(id);

        if (user == null || blog == null)
        {
            return NotFound("User or Blog not found.");
        }

        try{
            var result = await _blogSocialService.GetBookmarkAsync(blog, user);
            if (result == false)
            {
                await _blogSocialService.CreateBookmarkAsync(blog, user);
                return Ok("Blog Successfully Bookmarked");
            }
            else
            {
                await _blogSocialService.RemoveBookmarkAsync(blog, user);
                return Ok("Blog Bookmark removed");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(401, new { message = "Error liking or dislikng blog", error = ex});
        }

    }
}

