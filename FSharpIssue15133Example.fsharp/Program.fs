open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open System.Linq

[<CLIMutable>]
type Blog =
    {
        BlogId: int
        Url: string
        Rating: int
        Posts: List<Post>
    }

and [<CLIMutable>] Post =
    {
        PostId: int
        Title: string
        Content: string
        BlogId: int
        Blog: Blog
    }

type BloggingContext(options) =
    inherit DbContext(options)


    [<DefaultValue>] val mutable private _Blog : DbSet<Blog>
    member this.Blogs
        with get() = this._Blog
        and set v = this._Blog <- v

    [<DefaultValue>] val mutable private _Posts : DbSet<Post>
    member this.Posts 
        with get() = this._Posts
        and set v = this._Posts <- v


let builder = HostApplicationBuilder()
builder.Services.AddSqlite<BloggingContext>("Data Source=./1.db") |> ignore
builder.Logging.AddConsole() |> ignore

let app = builder.Build()
let db = app.Services.GetRequiredService<BloggingContext>()
db.Database.EnsureCreated() |> ignore

let ls = query {
        for i in db.Blogs do
            join j in db.Posts.Where(fun i -> i.Title.Length > 10) on (i.BlogId = j.BlogId)
            select {|i=i;j=j|}
}

printfn "%s" (ls.ToQueryString())


try
    ls.Where(fun i -> i.i.BlogId = 1).First() |> ignore
with _ -> ()