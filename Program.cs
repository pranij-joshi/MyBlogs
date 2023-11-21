using MyBlogs.Models;
using MyBlogs.DTOModels;
using MyBlogs.Services;
using MyBlogs.Validations;
using MyBlogs.Profiles;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Amazon.Util.Internal.PlatformServices;
using Microsoft.OpenApi.Models;
using FluentValidation;
// using MyBlogs.BlogProfile;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var jwtKey = builder.Configuration.GetSection("JwtKey").Value;
// Add services to the container.
builder.Services.Configure<UsersDatabaseSettings>(
    builder.Configuration.GetSection("UsersDatabase"));

builder.Services.Configure<BlogsDatabaseSettings>(
    builder.Configuration.GetSection("MyBlogsDatabase"));

builder.Services.Configure<BlogSocialDatabaseSettings>(
    builder.Configuration.GetSection("BlogSocialDatabase"));

builder.Services.Configure<BlogCommentsDatabaseSettings>(
    builder.Configuration.GetSection("BlogCommentDatabase"));

builder.Services.AddSingleton<BlogsService>();
builder.Services.AddSingleton<UsersService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<BlogSocialService>();
builder.Services.AddSingleton<BlogCommentService>();
// builder.Services.AddSingleton<IWebHostEnvironment>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<IValidator<UsersPostDTO>, UserRegisterValidaton>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    option => 
    {
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement{
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[]{}
            }
        });
    }
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
    {
        try{
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {   
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateLifetime=true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            };
        }
        catch (Exception ex)
        {
        
            System.Console.WriteLine($"Error configuring JWT authentication: {ex.Message}");
            // Optionally, you can rethrow the exception to propagate it up the call stack if needed.
            throw;

        }
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.ConfigureExceptionMiddleware();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.Run();
