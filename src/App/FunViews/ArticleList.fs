module App.FunViews.ArticleList

open Fun.Blazor
open Model

let private articleCard (articles: ArticleStub list) idx (stub: ArticleStub) =
    let leftOver = articles.Length % 3

    let span =
        match idx with
        | 0 -> "col-span-6"
        | x when x < 4 -> "col-span-2 row-span-2"
        | x when x < 6 -> "col-span-3 row-span-2"
        | x when x = articles.Length - 1 && leftOver = 1 -> "col-span-6 row-span-2"
        | x when x >= articles.Length - 2 && leftOver = 2 -> "col-span-3 row-span-2"
        | _ -> "col-span-2 row-span-1 h-72"

    div {
        class' $"{span} border border-gray-900 p-4 overflow-hidden"

        childContent [
            div { stub.Title }
            div { stub.Description }
            div { html.raw stub.Excerpt }
        ]
    }

let view (articles: ArticleStub list) =
    html.fragment [
        div {
            class' "grid grid-flow-row grid-cols-6 gap-4"

            childContent (articles |> List.mapi (articleCard articles))
        }
    ]
    |> Layout.Create
