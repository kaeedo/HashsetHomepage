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
        | _ -> "col-span-2 row-span-1 h-72"

    let span =
        let currentArticle = articles[idx]

        let isLastThreeArticle =
            articles
            |> List.skip (articles.Length - 3)
            |> List.contains currentArticle

        printfn $"idx: {idx}, isLastThree: {isLastThreeArticle}, leftOver: {leftOver}"

        if leftOver = 1 then
            if articles.Length - 1 = idx then
                span.Replace("col-span-2", "col-span-6")
            else
                span
        elif leftOver = 2 then
            if
                articles.Length - 1 = idx
                || articles.Length - 2 = idx
            then
                span.Replace("col-span-2", "col-span-3")
            else
                span
        else
            span

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
