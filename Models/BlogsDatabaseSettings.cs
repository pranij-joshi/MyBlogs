namespace MyBlogs.Models;

public class BlogsDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string BlogsCollectionName { get; set; } = null!;
}