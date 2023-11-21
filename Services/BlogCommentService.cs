using AutoMapper;
using MyBlogs.Models;
using MyBlogs.DTOModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MyBlogs.Services;

public class BlogCommentService
{
    private readonly IMongoCollection<BlogComments> _blogCommentCollection;
    private readonly IMapper _mapper;

    public BlogCommentService(IOptions<BlogCommentsDatabaseSettings> blogCommentDatabaseSettings, IMapper mapper)
    {
        var mongoClient = new MongoClient(
            blogCommentDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            blogCommentDatabaseSettings.Value.DatabaseName);

        _blogCommentCollection = mongoDatabase.GetCollection<BlogComments>(
            blogCommentDatabaseSettings.Value.BlogCommentCollectionName);
        
        _mapper = mapper;
    }

    public async Task<BlogComments> GetComment(string id)
    {
        var filter = Builders<BlogComments>.Filter.Where(x => x.Id == id); 
        var comment = await _blogCommentCollection.Find(filter).FirstOrDefaultAsync();
        return comment;
    }

    public async Task<List<BlogCommentsGetDTO?>> GetBlogComment(string blogId)
    {
        var comments = Builders<BlogComments>.Filter.Where(x => x.Blog.Id == blogId); 
        var result = await _blogCommentCollection.Find(comments).ToListAsync();
        var res = _mapper.Map<List<BlogCommentsGetDTO>>(result);

        return res;
    }
    public async Task<BlogComments> CreateComment(BlogsGetDTO blog, UsersGetDTO user, BlogCommentsPostDTO commentBody)
    {
        var newComment = _mapper.Map<BlogComments>(commentBody, opt => {
            opt.Items["blog"] = blog;
            opt.Items["user"] = user;
        });
        // newComment.Blog = blog;
        // newComment.User = user;
        // newComment.CommentDateTIme = DateTime.Now;
        System.Console.WriteLine(newComment.CommentDateTIme);
        System.Console.WriteLine(newComment.User);
        System.Console.WriteLine(newComment.Blog);
    
        await _blogCommentCollection.InsertOneAsync(newComment);
        // var res = _mapper.Map<BlogCommentsGetDTO>();
        return newComment;
    }

    public async Task UpdateComment(BlogComments updateComment)
    {
        await _blogCommentCollection.ReplaceOneAsync(x => x.Id == updateComment.Id, updateComment);
    }

    public async Task RemoveComment(string cid, string uid) 
    {   
        await _blogCommentCollection.DeleteOneAsync(x => x.Id == cid && x.User.Id == uid);
    }
}