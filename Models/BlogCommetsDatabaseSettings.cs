namespace MyBlogs.Models;

public class BlogCommentsDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string BlogCommentCollectionName { get; set; } = null!;
}