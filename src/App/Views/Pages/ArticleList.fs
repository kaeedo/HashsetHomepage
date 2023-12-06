[<RequireQualifiedAccess>]
module App.Views.Pages.ArticleList

open Fun.Blazor
open Model
open App.Views
open App.Views.Components

let private articleStub (stub: ArticleStub) =
    html.fragment [
        div {
            h3 {
                class' "text-2xl font-bold mb-2"
                stub.Title
            }
        }
        div {
            class' "mb-4"
            stub.Date.ToString("dd MMM yyyy")
        }

        div {
            class' "mb-4 w-fit"
            TagList.simple stub.Tags
        }

        div {
            class' "mb-2"
            stub.Description
        }

        div {
            class' "grid"

            div {
                class' "col-start-1 row-start-1"

                html.raw stub.Excerpt
            }


            div { class' "bg-gradient-to-t from-white col-start-1 row-start-1" }
        }
        div {
            class' "absolute bottom-4 right-4 flex justify-end w-full"

            childContent [
                (Button.round "→" (sprintf "article/%i" stub.Id))
            ]
        }
    ]

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

                    Card.simple gridSpanClasses (articleStub a))
            )
        }
    ]

let view (articles: ArticleStub list) =
    Layout.Create(h1 { "All Articles" }, html articles)
