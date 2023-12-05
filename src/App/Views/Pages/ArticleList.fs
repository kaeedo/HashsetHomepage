[<RequireQualifiedAccess>]
module App.Views.Pages.ArticleList

open Fun.Blazor
open Model
open App.Views
open App.Views.Components

let private html (articles: ArticleStub list) =
    html.fragment [
        div {
            class' "grid grid-flow-row grid-cols-6 gap-6"

            childContent (
                articles
                |> List.mapi (fun i a ->
                    let leftOver = articles.Length % 3

                    let gridSpanClasses =
                        match i with
                        | 0 -> "col-span-6"
                        | x when x < 4 -> "col-span-2 row-span-2"
                        | x when x < 6 -> "col-span-3 row-span-2"
                        | x when x = articles.Length - 1 && leftOver = 1 -> "col-span-6 row-span-2"
                        | x when x >= articles.Length - 2 && leftOver = 2 -> "col-span-3 row-span-2"
                        | _ -> "col-span-2 row-span-1"

                    Card.article gridSpanClasses a)
            )
        }
    ]

let view (articles: ArticleStub list) =
    Layout.Create("All Articles", html articles)
