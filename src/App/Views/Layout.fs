namespace App.Views

open Fun.Blazor
open Fun.Htmx

type Layout =
    static member private favicon =
        html.fragment [
            link {
                rel "android-chrome-192x192"
                size "180x180"
                href "/android-chrome-192x192.png"
            }

            link {
                rel "android-chrome-512x512"
                size "180x180"
                href "/android-chrome-512x512.png"
            }

            link {
                rel "apple-touch-icon"
                size "180x180"
                href "/apple-touch-icon.png"
            }

            link {
                rel "icon"
                size "32x32"
                href "/favicon-32x32.png"
            }

            link {
                rel "icon"
                size "16x16"
                href "/favicon-16x16.png"
            }

            link {
                rel "manifest"
                href "/site.webmanifest"
            }

            meta {
                name "msapplication-TileColor"
                content "#da532c"
            }

            meta {
                name "theme-color"
                content "#ffffff"
            }
        ]

    static member Create(pageTitle: NodeRenderFragment, bodyNode: NodeRenderFragment, ?headerNode: NodeRenderFragment) =
        html' {
            doctype "html"

            head {
                chartsetUTF8
                viewport "width=device-width, initial-scale=1.0"
                defaultArg headerNode (title { $"Hashset" })
                stylesheet "/css/tailwind.css"
                Layout.favicon
            }

            body {
                hxBoost true

                class' "bg-orange"

                Header.view pageTitle

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
