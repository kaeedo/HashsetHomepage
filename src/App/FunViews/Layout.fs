namespace App.FunViews

open Fun.Blazor

type Layout =
    static member Create(bodyNode: NodeRenderFragment, ?headerNode: NodeRenderFragment) =
        html' {
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
                bodyNode

                nav {
                    class' "flex items-center gap-3 container mx-auto py-2"

                    childContent [
                        a {
                            href ""
                            class' "text-primary text-lg font-medium"
                            "GiraffeHtmxBlazor"
                        }
                        div { class' "flex-grow" }
                        a {
                            href ""
                            class' "link link-primary"
                            "Home"
                        }
                        a {
                            href "dashboard"
                            class' "link link-primary"
                            "Dashboard"
                        }
                    ]
                }

                script { src "https://unpkg.com/htmx.org@1.9.9" }
            }
        }
