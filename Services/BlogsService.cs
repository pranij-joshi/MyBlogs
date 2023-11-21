using AutoMapper;
using MyBlogs.Models;
using MyBlogs.DTOModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MyBlogs.Services;

public class BlogsService
{
    private readonly IMongoCollection<Blogs> _blogsCollection;
    private readonly IMapper _mapper;

    public BlogsService(IOptions<BlogsDatabaseSettings> blogsDatabaseSettings, IMapper mapper)
    {
        var mongoClient = new MongoClient(
            blogsDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            blogsDatabaseSettings.Value.DatabaseName);

        _blogsCollection = mongoDatabase.GetCollection<Blogs>(
            blogsDatabaseSettings.Value.BlogsCollectionName);
        
        _mapper = mapper;
    }

    public async Task<List<BlogsGetDTO>> GetAsync()
    {
        var blog = await _blogsCollection.Find(_ => true).ToListAsync();
        var res = _mapper.Map<List<BlogsGetDTO>>(blog);
        return res;
    }
    
    public async Task<List<BlogsGetDTO>> SearchAsync(string search)
    {
        var filter = Builders<Blogs>.Filter.Where(x => 
            x.Title.ToUpper().Contains(search.ToUpper()) ||
            x.Author.ToUpper().Contains(search.ToUpper()) ||
            x.Category.ToUpper().Contains(search.ToUpper()) || 
            x.Tags.ToUpper().Contains(search.ToUpper()) ||
            x.Author.ToUpper().Contains(search.ToUpper())); 
        var blog = await _blogsCollection.Find(filter).ToListAsync();
        var res = _mapper.Map<List<BlogsGetDTO>>(blog);
        System.Console.WriteLine(res.Count);
        return res;
    }

    public async Task<BlogsGetDTO?> GetAsync(string id)
    {
        var blog = await _blogsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        var res = _mapper.Map<BlogsGetDTO>(blog);   
        return res;
    }
    public async Task<Blogs> CreateAsync(BlogsPostDTO newBlogDto, UsersGetDTO userData)
    {
        var newBlog = _mapper.Map<Blogs>(newBlogDto);
        newBlog.PublishDateTime = DateTime.Now;
        newBlog.IsBookmark = false;
        newBlog.IsLike = false;
        newBlog.IsLikeCount = null;
        newBlog.User = userData;
        // var mappedData = newBlog.Select((r, index) =>
        // {
        //     r.PublishDateTime = DateTimeOffset.UtcNow;
        // });
        await _blogsCollection.InsertOneAsync(newBlog);
        return newBlog;
    }

    public async Task UpdateAsync(Blogs updateBlog)
    {
        await _blogsCollection.ReplaceOneAsync(x => x.Id == updateBlog.Id, updateBlog);
    }

    public async Task RemoveAsync(string id) => await _blogsCollection.DeleteOneAsync(x => x.Id == id);
}