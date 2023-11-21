using AutoMapper;
using MyBlogs.Models;
using MyBlogs.DTOModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MyBlogs.Services;

public class BlogSocialService
{
    private readonly IMongoCollection<BlogLikeDislike> _blogLikeDislike;
    private readonly IMongoCollection<BlogBookmark> _blogBookmark;

    public BlogSocialService(IOptions<BlogSocialDatabaseSettings> blogSocialDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            blogSocialDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            blogSocialDatabaseSettings.Value.DatabaseName);

        _blogLikeDislike = mongoDatabase.GetCollection<BlogLikeDislike>(
            blogSocialDatabaseSettings.Value.BlogLikeDislikeCollectionName); 
     
        _blogBookmark = mongoDatabase.GetCollection<BlogBookmark>(
            blogSocialDatabaseSettings.Value.BlogBookmarkCollectionName); 

    }

    public async Task<int> GetLikeDislikeCountAsync(BlogsGetDTO blog)
    {
        var blogData = await _blogLikeDislike.Find(x => x.Blog.Id == blog.Id).ToListAsync();
        int len = blogData.Count;
        return len;
    }

    public async Task<bool?> GetLikeDislikeAsync(BlogsGetDTO blog, UsersGetDTO user)
    {
        var filter = Builders<BlogLikeDislike>.Filter.Where(x => x.Blog.Id == blog.Id && x.User.Id == user.Id); 
        var result = await _blogLikeDislike.Find(filter).FirstOrDefaultAsync();
        return result != null;
    }
    public async Task CreateLikeDislikeAsync(BlogsGetDTO blog, UsersGetDTO user)
    {
        var blogSocialNew = new BlogLikeDislike
            {
                Blog = blog,
                User = user
            };
        await _blogLikeDislike.InsertOneAsync(blogSocialNew);
    }

    public async Task RemoveLikeDislikeAsync(BlogsGetDTO blog, UsersGetDTO user)
    { 
        await _blogLikeDislike.DeleteOneAsync(x => x.Blog == blog && x.User == user);
    }


    ///Bookmark Section
    public async Task<int> GetBookmarkCountAsync(BlogsGetDTO blog)
    {
        var blogData = await _blogBookmark.Find(x => x.Blog.Id == blog.Id).ToListAsync();
        int len = blogData.Count;
        return len;
    }

    public async Task<bool?> GetBookmarkAsync(BlogsGetDTO blog, UsersGetDTO user)
    {
        var filter = Builders<BlogBookmark>.Filter.Where(x => x.Blog.Id == blog.Id && x.User.Id == user.Id); 
        var result = await _blogBookmark.Find(filter).FirstOrDefaultAsync();
        return result != null;
    }
    public async Task CreateBookmarkAsync(BlogsGetDTO blog, UsersGetDTO user)
    {
        var blogSocialNew = new BlogBookmark
            {
                Blog = blog,
                User = user
            };
        await _blogBookmark.InsertOneAsync(blogSocialNew);
    }

    public async Task RemoveBookmarkAsync(BlogsGetDTO blog, UsersGetDTO user)
    { 
        await _blogBookmark.DeleteOneAsync(x => x.Blog == blog && x.User == user);
    }
}