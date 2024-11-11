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
        fragment {
            doctype "html"

            html' {
                head {
                    chartsetUTF8
                    viewport "width=device-width, initial-scale=1.0"
                    defaultArg headerNode (title { "Hashset" })

#if DEBUG
                    script { src "https://cdn.tailwindcss.com?plugins=typography" }

                    script {
                        type' "text/javascript"

                        @"
                            tailwind.config = {
                                theme: {
                                    extend: {
                                        spacing: {
                                            112: ""28rem"",
                                            128: ""32rem"",
                                            136: ""34rem"",
                                            204: ""51rem""
                                            
                                        }
                                    },
                                    // https://mdigi.tools/lighten-color/#000000
                                    colors: {
                                        transparent: ""transparent"",
                                        current: ""currentColor"",
                                        black: ""#000000"",
                                        gray: ""#a6a6a6"",
                                        white: ""#ffffff"",
                                        purple: ""#cd76ea"",
                                        ""purple-dark"": ""#54106b"", // 65% darker
                                        green: ""#00ff75"",
                                        yellow: ""#fff500"",
                                        red: ""#ff5e5e"",
                                        orange: ""#ffb443"",
                                        ""blue-light"": ""#baf2ff"", // 65% lighter
                                        blue: ""#39dbff"",
                                        ""blue-dark"": ""#00596d"", // 65% darker
                                    }
                                }
                            }
                        "
                    }
#else
                    stylesheet "/css/tailwind.css"
#endif

                    Layout.favicon
                }

                body {
                    hxBoost true

                    class' "bg-orange"

                    Header.view pageTitle

                    div {
                        class' "container mx-auto p-4 py-8 md:p-10"
                        bodyNode
                    }

                    Footer.view ()

                    script { src "https://unpkg.com/htmx.org@2.0.3" }
                }
            }
        }
