[<RequireQualifiedAccess>]
module App.Views.Components.TagList

open Fun.Blazor
open Model

let simple (tags: Tag list) =
    div {
        class' "flex flex-wrap"

        childContent (
            tags
            |> List.map (fun t ->
                a {
                    class'
                        "relative border-2 py-1 px-2 min-w-max bg-blue drop-shadow-[4px_4px_0px_#dd7dff] hover:bg-purple"

                    href "#"

                    t.Name
                })
        )
    }
