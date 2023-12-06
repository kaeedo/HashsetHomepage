[<RequireQualifiedAccess>]
module App.Views.Components.TagList

open Fun.Blazor
open Model
open System

let simple (tags: Tag list) =
    div {
        class' "flex flex-wrap"

        childContent (
            tags
            |> List.map (fun t ->
                let encoded = Web.HttpUtility.UrlEncode(t.Name, Text.Encoding.ASCII)

                a {
                    class'
                        "relative border-2 py-1 px-2 min-w-max bg-blue drop-shadow-[4px_4px_0px_#dd7dff] hover:bg-purple"

                    href $"/articles?tag=%s{encoded}"

                    t.Name
                })
        )
    }
