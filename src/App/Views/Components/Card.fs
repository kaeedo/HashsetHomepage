[<RequireQualifiedAccess>]
module App.Views.Components.Card

open Fun.Blazor
open Model

let simple (additionalClasses: string) (cardContent: NodeRenderFragment) =
    div {
        class'
            $"{additionalClasses} min-h-72 max-h-136 p-4 overflow-hidden bg-white border-4 border-black drop-shadow-[8px_8px_0px_#ff5dfd]"

        cardContent
    }


let article (additionalClasses: string) (stub: ArticleStub) =
    div {
        class'
            $"{additionalClasses} min-h-72 max-h-136 p-4 overflow-hidden bg-white border-4 border-black drop-shadow-[8px_8px_0px_#ff5dfd]"

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
        }
    }
