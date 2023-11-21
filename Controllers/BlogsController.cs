using AutoMapper;
using MyBlogs.Models;
using MyBlogs.PaginationModels;
using MyBlogs.Services;
using MyBlogs.DTOModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Bson.IO;
using System.Text.Json;

namespace MyBlogs.Controllers;

[ApiController]
[Route("api/blog")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class BlogsController : ControllerBase
{
    private readonly BlogsService _blogsService;
    private readonly JwtService _jwtService;
    private readonly UsersService _userService;
    private readonly IMapper _mapper;

    public BlogsController(BlogsService blogsService, JwtService jwtService, UsersService usersService, IMapper mapper)
    {
        _blogsService = blogsService;
        _jwtService = jwtService;
        _userService = usersService;
        _mapper = mapper;
    }

    
    [HttpGet("getAll")]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] PaginationParams @params)
    {
        var blog = await _blogsService.GetAsync();
                                    
        if (blog.Any())
        {
            var paginationMetadata = new PaginationMetadata(blog.Count, @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            var paginatedBlogs = blog
                .Skip((@params.Page - 1) * @params.ItemsPerPage)
                .Take(@params.ItemsPerPage);
            return Ok(paginatedBlogs);
        }
        return NotFound();
    }

    [HttpGet("search/{search}")]
    [AllowAnonymous]
    public async Task<IActionResult> Search(string search, [FromQuery] PaginationParams @params)
    {
        var blog = await _blogsService.SearchAsync(search);
        if (blog.Any())
        {
             var paginationMetadata = new PaginationMetadata(blog.Count, @params.Page, @params.ItemsPerPage);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            var paginatedBlogs = blog
                .Skip((@params.Page - 1) * @params.ItemsPerPage)
                .Take(@params.ItemsPerPage);
                return Ok(paginatedBlogs);
        }
        return NotFound();
    }

    [HttpGet("get/{id:length(24)}")]
    public async Task<IActionResult> Get(string id, [FromQuery] PaginationParams @params)
    {
        var blog = await _blogsService.GetAsync(id);

        if (blog is null)
        {
            return NotFound();
        }
        return Ok(blog);
    }
    

    [HttpPost("create")]
    public async Task<IActionResult> Post(BlogsPostDTO newBlog)
    {   
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        // Check if the token is missing or empty
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("No token provided.");
        }

        var userId = _jwtService.DecodeJwtAccessToken(token);  
        var user = await _userService.GetAsync(userId);

        var createdBlog = await _blogsService.CreateAsync(newBlog, user);

        return CreatedAtAction(nameof(Get), new { id = createdBlog.Id }, createdBlog);
    }


    [HttpPut("update/{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Blogs blog)
    {
        var existingBlog = await _blogsService.GetAsync(id);
        
        if (existingBlog is null)
            return NotFound();
        
        blog.Id = existingBlog.Id;
        await _blogsService.UpdateAsync(blog);
        return NoContent();
    }

    [HttpDelete("delete/{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingBlog = await _blogsService.GetAsync(id);
        
        if (existingBlog is null)
            return NotFound();

        await _blogsService.RemoveAsync(id).ConfigureAwait(false);

        return Ok();   
    }
}

