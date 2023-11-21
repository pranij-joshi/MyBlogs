using AutoMapper;
using MyBlogs.Models;
using MyBlogs.Services;
using MyBlogs.DTOModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentValidation;


namespace MyBlogs.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/auth")]
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;
    private readonly IMapper _mapper;
    private readonly JwtService _jwtService;
    private readonly IValidator<UsersPostDTO> _userValidator;

    public UsersController(UsersService usersService, IMapper mapper, JwtService jwtService, IValidator<UsersPostDTO> userValidator)
    {
        _usersService = usersService;
        _mapper = mapper;
        _jwtService = jwtService;
        _userValidator = userValidator;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UsersLoginDTO loginDto)
    {
        // var userList = await _usersService.LoginService(loginDto);
        var user = await _usersService.LoginService(loginDto);
        // var user = userList.FirstOrDefault(c => c.Username.Trim().ToUpper() == loginDto.Username.Trim().ToUpper());
        try{

            var access_token = _jwtService.GenerateJwtAccessToken(user.Id, user.Username, user.Email);
            var refresh_token = _jwtService.GenerateJwtRefreshToken(user.Id, user.Username, user.Email);
            return Ok(new { messsage = "Login Successful", accessToken = access_token, refreshToken = refresh_token});
        }
        catch (Exception e)
        {
            return StatusCode(401, e.Message);
        }
        // return StatusCode(500, "Error");
    }
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try{

            var blog = await _usersService.GetAsync();
            if (blog.Any())
                return Ok(blog);
            return NotFound();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An exception occurred: {ex}");
               
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{id:length(24)}")]
    public async Task<IActionResult> Get(string id)
    {
        var blog = await _usersService.GetAsync(id);

        if (blog is null)
        {
            return NotFound();
        }

        return Ok(blog);
    }
    

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Post([FromForm]UsersPostDTO newUser)
    {   
        var validationResult = _userValidator.Validate(newUser);

        if (!validationResult.IsValid)
        {
            // Handle validation errors
            return BadRequest(validationResult.Errors);
        }
        var createdUser = await _usersService.CreateAsync(newUser);

        return CreatedAtAction(nameof(Get), new { id = createdUser.Id }, createdUser);
    }


    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Users user)
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        // Check if the token is missing or empty
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("No token provided.");
        }

        var userId = _jwtService.DecodeJwtAccessToken(token);
        var existingUser = await _usersService.GetAsync(id);
        
        if (existingUser is null)
            return NotFound();
        if (userId == id)
        {
            user.Id = existingUser.Id;
            await _usersService.UpdateAsync(user);
        }
        else{
            return Unauthorized();
        }

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        // Check if the token is missing or empty
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("No token provided.");
        }

        var userId = _jwtService.DecodeJwtAccessToken(token);

        var existingUser = await _usersService.GetAsync(id);
        if (userId != id)
        {
            
            return Unauthorized();
        }
        
        if (existingUser is null)
            return NotFound();

        await _usersService.RemoveAsync(id);

        return Ok(new { Message = "Successfully Deleted"});   
    }

}

