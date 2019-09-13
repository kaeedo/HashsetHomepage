namespace Hashset

open Hashset.Views
open System
open Giraffe

module Controller =
    let homepage () : HttpHandler  =
        let latestPost = Posts.getLatestPost()

        let masterData =
            { MasterContent.Author = "Kai Ito"
              JobTitle = "Software Developer"
              PageTitle = latestPost.Title
              ArticleDate = Some DateTime.Now }

        Post.view latestPost
        |> Master.view masterData
        |> htmlView

    let posts (): HttpHandler =
        let masterData =
            { MasterContent.Author = "Kai Ito"
              JobTitle = "Software Developer"
              PageTitle = "About Me"
              ArticleDate = None }

        let posts = Posts.getPosts()

        About.view
        |> Master.view masterData
        |> htmlView

    let about (): HttpHandler =
        let masterData =
            { MasterContent.Author = "Kai Ito"
              JobTitle = "Software Developer"
              PageTitle = "About Me"
              ArticleDate = None }

        About.view
        |> Master.view masterData
        |> htmlView
