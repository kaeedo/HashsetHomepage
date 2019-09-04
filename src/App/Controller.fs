namespace Hashset

open Hashset.Views
open System
open Giraffe

module Controller =
    let homepage () : HttpHandler  =
        let latestPost = Reader.getLatestPost()

        let masterData =
            { MasterContent.Author = "Kai Ito"
              JobTitle = "Software Developer"
              PageTitle= "Home"
              ArticleDate = DateTime.Now.ToShortDateString() }

        Home.view latestPost
        |> Master.view masterData
        |> htmlView
