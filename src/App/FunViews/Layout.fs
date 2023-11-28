namespace App.FunViews

open Fun.Blazor

type Layout =
    static member Create(bodyNode: NodeRenderFragment, ?headerNode: NodeRenderFragment) =
        html' {
            doctype "html"

            head {
                chartsetUTF8
                viewport "width=device-width, initial-scale=1.0"
                defaultArg headerNode (title { "Hashset" })
#if DEBUG
                script { src "https://cdn.tailwindcss.com/" }
#else
                stylesheet "/css/tailwind.css"
#endif
            }

            body {
                class' "container mx-auto p-10"
                bodyNode

                script { src "https://unpkg.com/htmx.org@1.9.9" }
            }
        }
