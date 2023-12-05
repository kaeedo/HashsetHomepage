[<RequireQualifiedAccess>]
module App.Views.Pages.About

open Fun.Blazor
open App.Views.Components
open App.Views



let private html =
    html.fragment [
        div {
            class' "grid grid-cols-3 gap-6"

            childContent [
                html.fragment [
                    h4 {
                        class' "text-lg mb-4"
                        "Hello, I'm Kai Ito"
                    }
                    p {
                        class' "mb-2"
                        "I am a Software Developer at "

                        a {
                            class' "underline"
                            href "https://valora.digital/"
                            target "_blank"
                            "Valora Digital"
                        }

                        ". I have been a Full Stack .Net web developer since 2012. Back then, I mainly used Asp.net MVC, and mostly jQuery on the frontend, with a little bit of Knockout.js sprinkled in. I have since taken a liking to Functional Programming, especially F#. In my experience, the functional paradigm has since proven to be invaluable in writing bug free software, especially when combined with a powerful type system like F#"
                    }
                    p {
                        "These days, I have become an advocate of online privacy, especially because of the massive amounts of tracking that a large number websites and smartphone apps practice. I strongly believe in Open Source Software, and try to use them as much as possible. You'll find a link to my Mastodon account below, and not to a Twitter or Facebook account for these reasons. For the moment, I'm still on Codeberg and Github, but ideally in the future, something like Forgejo will allow federated communication."
                    }
                ]
                |> Card.simple "col-span-2"

                html.fragment [
                    h4 {
                        class' "text-lg mb-4"
                        "Come find me at the following sites"
                    }
                    ul {
                        class' "pl-4"

                        li {
                            class' "flex mb-3 items-center"

                            div {
                                class' "w-10"

                                i { html.raw Svg.mastodon }
                            }

                            a {
                                class' "underline"
                                href "https://mstdn.social/@kaeedo"

                                "Mastodon"
                            }
                        }

                        li {
                            class' "flex mb-3 items-center"

                            div {
                                class' "w-10"

                                i { html.raw Svg.codeberg }
                            }

                            a {
                                class' "underline"
                                href "https://codeberg.org/CubeOfShame"

                                "Codeberg"
                            }
                        }

                        li {
                            class' "flex mb-3 items-center"

                            div {
                                class' "w-10"

                                i { html.raw Svg.github }
                            }

                            a {
                                class' "underline"
                                href "https://github.com/kaeedo"

                                "GitHub"
                            }
                        }

                        li {
                            class' "flex mb-3 items-center"

                            div {
                                class' "w-10"

                                i { html.raw Svg.rss }
                            }

                            span {
                                a {
                                    class' "underline"
                                    href "https://github.com/kaeedo"

                                    "RSS"
                                }

                                " | "

                                a {
                                    class' "underline"
                                    href "https://github.com/kaeedo"

                                    "ATOM"
                                }
                            }
                        }
                    }
                ]
                |> Card.simple ""
            ]
        }
    ]

let view () = Layout.Create("About", html)
