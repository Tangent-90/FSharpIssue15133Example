using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

var builder = new HostApplicationBuilder();
builder.Services.AddSqlite<BloggingContext>("Data Source=./1.db");
builder.Logging.AddConsole();

var app = builder.Build();

var db = app.Services.GetRequiredService<BloggingContext>();
db.Database.EnsureCreated();

var ls = from i in db.Blogs
         join j in db.Posts.Where(i =>i.Title.Length > 10) on i.BlogId equals j.BlogId
         select new { i, j };
Console.WriteLine(ls.ToQueryString());
try
{
    ls.Where(i => i.i.BlogId == 1).First();
}
catch (Exception)
{
}


public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public int Rating { get; set; }
    public List<Post> Posts { get; set; }
}

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public int BlogId { get; set; }
    public Blog Blog { get; set; }
}

public class BloggingContext : DbContext
{
    public BloggingContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
}