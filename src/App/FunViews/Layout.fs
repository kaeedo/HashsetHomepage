namespace App.FunViews

open Fun.Blazor
open Fun.Htmx

type Layout =
    static member Create(bodyNode: NodeRenderFragment, ?headerNode: NodeRenderFragment) =
        html' {
            doctype "html"

            head {
                chartsetUTF8
                viewport "width=device-width, initial-scale=1.0"
                defaultArg headerNode (title { "Hashset" })
                stylesheet "/css/tailwind.css"
            }

            body {
                hxBoost true

                class' "bg-orange"

                Header.view ()

                div {
                    class' "container mx-auto p-10"
                    bodyNode

                }

                Footer.view ()

                script { src "https://unpkg.com/htmx.org@1.9.9" }
            }
        }

// Just deal with it for now
// script {
//     type' "module"
//     src "/js/wrapDetection.mjs"
// }
