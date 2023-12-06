[<RequireQualifiedAccess>]
module App.Views.Components.TagList

open Microsoft.AspNetCore.Http
open Fun.Blazor
open Model
open System

let simple (tags: Tag list) =
    div {
        class' "flex flex-wrap"

        childContent (
            tags
            |> List.map (fun t ->
                html.inject (fun (accessor: IHttpContextAccessor) ->
                    let selectedTag =
                        accessor.HttpContext.Request.Query["tag"]
                        |> Seq.tryHead

                    let encoded = Web.HttpUtility.UrlEncode(t.Name, Text.Encoding.ASCII)

                    let link, bgClass =
                        match selectedTag with
                        | Some tag when tag = t.Name -> "/articles", "bg-purple"
                        | Some _
                        | None -> $"/articles?tag=%s{encoded}", "bg-blue"

                    a {
                        class'
                            $"relative border-2 py-1 px-2 min-w-max {bgClass} drop-shadow-[4px_4px_0px_#dd7dff] hover:bg-purple"

                        href link

                        t.Name
                    }))
        )
    }
