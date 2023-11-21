namespace MyBlogs.Models;

public class BlogSocialDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string BlogLikeDislikeCollectionName { get; set; } = null!;
    public string BlogBookmarkCollectionName { get; set; } = null!;
}