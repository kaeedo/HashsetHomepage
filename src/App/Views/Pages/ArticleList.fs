[<RequireQualifiedAccess>]
module App.Views.Pages.ArticleList

open Fun.Blazor
open Model
open App.Views
open App.Views.Components

let private articleStub (stub: ArticleStub) =
    html.fragment [
        div {
            childContent (
                h3 {
                    childContent (
                        a {
                            class' "text-2xl font-bold mb-2"
                            href $"article/%i{stub.Id}"
                            stub.Title
                        }
                    )
                }
            )
        }
        div {
            class' "mb-4"
            stub.Date.ToString("dd MMM yyyy")
        }

        div {
            class' "mb-4 w-fit"
            childContent (TagList.simple stub.Tags)
        }

        div {
            class' "mb-2 prose max-w-none"
            stub.Description
        }

        div {
            class' "grid"

            div {
                class' "col-start-1 row-start-1 prose max-w-none"

                html.raw stub.Excerpt
            }


            div { class' "bg-gradient-to-t from-white col-start-1 row-start-1" }
        }
        div {
            class' "absolute bottom-4 right-4 flex justify-end w-full"

            childContent [
                (Button.round "→" $"article/%i{stub.Id}")
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
                    let xlClasses =
                        let leftOver = articles.Length % 3

                        match i with
                        | 0 -> "xl:col-span-6"
                        | x when x < 4 -> "xl:col-span-2 xl:row-span-2"
                        | x when x < 6 -> "xl:col-span-3 xl:row-span-2"
                        | x when x = articles.Length - 1 && leftOver = 1 -> "xl:col-span-6 xl:row-span-2"
                        | x when x >= articles.Length - 2 && leftOver = 2 -> "xl:col-span-3 xl:row-span-2"
                        | _ -> "xl:col-span-2 xl:row-span-1"

                    let lgClasses =
                        let leftOver = articles.Length % 2

                        match i with
                        | 0 -> "lg:col-span-6"
                        | x when x < 3 -> "lg:col-span-3 lg:row-span-2"
                        | x when x < 4 -> "lg:col-span-6"
                        | x when x = articles.Length - 1 && leftOver = 1 -> "lg:col-span-6"
                        | _ -> "lg:col-span-3 lg:row-span-1"

                    let baseClasses = "col-span-6"

                    Card.simple ($"{xlClasses} {lgClasses} {baseClasses}") (articleStub a))
            )
        }
    ]

let view (articles: ArticleStub list) =
    Layout.Create(h1 { "All Articles" }, html articles)
