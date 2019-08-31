namespace Hashset

open Hashset.Views
open System
open Giraffe

module Controller =
    let homepage () : HttpHandler  =
        let masterData =
            { MasterContent.Author = "Kai Ito"
              JobTitle = "Software Developer"
              PageTitle= "Home"
              ArticleDate = DateTime.Now.ToShortDateString() }

        let asdf = Reader.getLatestPost()

        Home.view
        |> Master.view masterData
        |> htmlView
