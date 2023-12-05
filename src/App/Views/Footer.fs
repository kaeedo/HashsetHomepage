module App.Views.Footer

open Fun.Blazor
open Fun.Htmx
open System


let view () =
    footer {
        class' "mb-8"

        ul {
            hxBoost false
            class' "container mx-auto border-2 bg-green rounded-3xl w-max px-8 py-4"

            childContent [
                li {
                    class' "flex mb-3"

                    div { class' "w-10 h-0" }
                    div { sprintf "Blog Posts Copyright © %i" DateTime.Now.Year }
                }
                li {
                    class' "flex mb-3"

                    childContent [
                        div { class' "w-10 h-0" }
                        div {
                            childContent [
                                span { "Source Code available on " }

                                a {
                                    class' "underline"
                                    target "_blank"
                                    href "https://codeberg.org/CubeOfShame/Hashset"
                                    "Codeberg"
                                }

                                span { " or " }

                                a {
                                    class' "underline"
                                    target "_blank"
                                    href "https://github.com/kaeedo/HashsetHomepage"
                                    "GitHub"
                                }
                            ]
                        }
                    ]
                }
                li {
                    class' "flex mb-3"

                    div {
                        class' "w-10"

                        i { html.raw Svg.mastodon }
                    }

                    div {
                        a {
                            rel "me"
                            href "https://mstdn.social/@kaeedo"

                            span {
                                class' "underline"
                                "Follow me"
                            }

                            " on Mastodon"
                        }
                    }
                }
                li {
                    class' "flex"

                    childContent [
                        div {
                            class' "w-10"

                            i { html.raw Svg.rss }
                        }
                        span {
                            a {
                                class' "underline"
                                href "/rss"
                                "RSS"
                            }

                            " | "

                            a {
                                class' "underline"
                                href "/atom"
                                "ATOM"
                            }
                        }
                    ]
                }
            ]
        }
    }
