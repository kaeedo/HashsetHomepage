namespace App

open FSlugify.SlugGenerator

open Fun.Blazor
open Microsoft.AspNetCore.Http
open Giraffe
open Microsoft.Extensions.DependencyInjection

module Utils =
    let getUrl id title =
        let title = slugify DefaultSlugGeneratorOptions title

        sprintf "%i_%s" id title

type View =
    static member inline Build(node: NodeRenderFragment) : HttpHandler =
        fun nxt ctx ->
            task {
                do! ctx.WriteFunDom(node)
                return! nxt ctx
            }

    static member inline Build(render: HttpContext -> NodeRenderFragment) : HttpHandler =
        fun nxt ctx ->
            task {
                let node = render ctx
                return! View.Build (node) nxt ctx
            }

    static member inline Build(render: unit -> NodeRenderFragment) : HttpHandler =
        fun nxt ctx ->
            task {
                let node = render ()
                return! View.Build (node) nxt ctx
            }
