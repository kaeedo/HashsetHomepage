namespace Hashset

open Hashset.Views
open System
open System.IO
open System.Text.Json
open Giraffe

module Controller =
    let renderPostPage post =
        let masterData =
            { MasterContent.PageTitle = post.Title
              ArticleDate = Some post.ArticleDate }

        Post.view post
        |> Master.view masterData
        |> htmlView

    let homepage () : HttpHandler  =
        let latestPost = Posts.getLatestPost()

        renderPostPage latestPost

    let post postName : HttpHandler =
        renderPostPage <| Posts.getPost postName

    let posts (): HttpHandler =
        // TODO: Server side paging
        let masterData =
            { MasterContent.PageTitle = "All Posts"
              ArticleDate = None }

        let getFirstParagraph (content: string) =
            let firstIndex = content.IndexOf("<p>") + 3
            let lastIndex = content.IndexOf("</p>")
            let count = lastIndex - firstIndex

            content.Substring(firstIndex, count)

        let posts =
            Posts.getPosts()
            |> Seq.map (fun p ->
                use fs = p.OpenText()
                let fileContents = fs.ReadToEnd()

                JsonSerializer.Deserialize<ParsedDocument>(fileContents)
            )
            |> Seq.map (fun (p: ParsedDocument) ->
                { PostStub.Title = p.Title.Trim()
                  Date = p.ArticleDate
                  Description = getFirstParagraph p.Document }
            )

        LatestPosts.view posts
        |> Master.view masterData
        |> htmlView

    let about (): HttpHandler =
        let masterData =
            { MasterContent.PageTitle = "About Me"
              ArticleDate = None }

        About.view
        |> Master.view masterData
        |> htmlView
