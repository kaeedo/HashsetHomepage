module App.FunViews.ArticleList

open Fun.Blazor
open Model

let private button (label: string) link =
    a {
        class'
            "px-4 py-2 border-[3px] bg-green rounded-full border-black text-xl font-extrabold drop-shadow-[4px_4px_0px_#000] active:drop-shadow-[1px_2px_0px_#000] active:translate-y-1 active:translate-x-1"

        href "#"
        label
    }

let private tagList (tags: Tag list) =
    div {
        class' "flex flex-wrap"

        childContent (
            tags
            |> List.map (fun t ->
                a {
                    class' "relative border-2 py-1 px-2 min-w-max bg-blue drop-shadow-[4px_4px_0px_#dd7dff]"
                    href "#"

                    t.Name
                })
        )
    }

let private articleCard (articles: ArticleStub list) idx (stub: ArticleStub) =
    let leftOver = articles.Length % 3

    let span =
        match idx with
        | 0 -> "col-span-6"
        | x when x < 4 -> "col-span-2 row-span-2"
        | x when x < 6 -> "col-span-3 row-span-2"
        | x when x = articles.Length - 1 && leftOver = 1 -> "col-span-6 row-span-2"
        | x when x >= articles.Length - 2 && leftOver = 2 -> "col-span-3 row-span-2"
        | _ -> "col-span-2 row-span-1"

    div {
        class'
            $"{span} min-h-72 max-h-136 p-4 overflow-hidden bg-white border-4 border-black drop-shadow-[8px_8px_0px_#ff5dfd]"

        div {
            childContent [
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
                    tagList stub.Tags
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
                        (button "→" (sprintf "article/%i" stub.Id))
                    ]
                }
            ]
        }
    }

let view (articles: ArticleStub list) =
    html.fragment [
        div {
            class' "grid grid-flow-row grid-cols-6 gap-6"

            childContent (articles |> List.mapi (articleCard articles))
        }
    ]
    |> Layout.Create
