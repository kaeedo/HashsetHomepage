module App.Views.Footer

open Fun.Blazor
open Fun.Htmx
open System

let view () =
    footer {
        class' "mb-8"

        div {
            class' "mx-auto max-w-fit border-2 bg-green rounded-xl w-max px-4 py-2 md:rounded-3xl md:px-8 md:py-4"

            childContent [
                div {
                    class' "flex mb-3"

                    div { class' "hidden sm:block w-10 h-0" }
                    div { sprintf "Blog Posts Copyright © %i" DateTime.Now.Year }
                }
                div {
                    class' "flex mb-3"

                    childContent [
                        div { class' "hidden sm:block w-10 h-0" }
                        span {
                            "Source Code available on "

                            a {
                                class' "underline"
                                target "_blank"
                                href "https://codeberg.org/CubeOfShame/Hashset"
                                "Codeberg"
                            }

                            " or "

                            a {
                                class' "underline"
                                target "_blank"
                                href "https://github.com/kaeedo/HashsetHomepage"
                                "GitHub"
                            }
                        }
                    ]
                }
                div {
                    class' "flex mb-3"

                    div {
                        class' "hidden sm:block w-10"

                        i { html.raw Svg.mastodon }
                    }

                    div {
                        a {
                            class' "underline"
                            rel "me"
                            href "https://mstdn.social/@kaeedo"
                            "Follow me"
                        }

                        " on Mastodon"
                    }
                }
                div {
                    class' "flex"

                    childContent [
                        div {
                            class' "hidden sm:block w-10"

                            i { html.raw Svg.rss }
                        }
                        span {
                            hxBoost false

                            a {
                                class' "underline"
                                href "/rss"
                                "RSS"
                            }

                            " | "

                            a {
                                class' "underline"
                                href "/atom"
                                "Atom"
                            }
                        }
                    ]
                }
            ]
        }
    }
