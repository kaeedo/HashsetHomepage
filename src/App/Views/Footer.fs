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
                    div {
                        class' "flex mb-3"

                        div { class' "w-10 h-0" }
                        div { sprintf "Blog Posts Copyright © %i" DateTime.Now.Year }
                    }
                }
                li {
                    div {
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
                }
                li {
                    div {
                        class' "flex mb-3"

                        div {
                            class' "w-10"

                            i {
                                html.raw
                                    """<svg width="32px" height="32px" fill="#6364FF" role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                                        <rect height="14" width="17" x="3" y="3" fill="#fff" />
                                        <path d="M23.193 7.879c0-5.206-3.411-6.732-3.411-6.732C18.062.357 15.108.025 12.041 0h-.076c-3.068.025-6.02.357-7.74 1.147 0 0-3.411 1.526-3.411 6.732 0 1.192-.023 2.618.015 4.129.124 5.092.934 10.109 5.641 11.355 2.17.574 4.034.695 5.535.612 2.722-.15 4.25-.972 4.25-.972l-.09-1.975s-1.945.613-4.129.539c-2.165-.074-4.449-.233-4.799-2.891a5.499 5.499 0 0 1-.048-.745s2.125.52 4.817.643c1.646.075 3.19-.097 4.758-.283 3.007-.359 5.625-2.212 5.954-3.905.517-2.665.475-6.507.475-6.507zm-4.024 6.709h-2.497V8.469c0-1.29-.543-1.944-1.628-1.944-1.2 0-1.802.776-1.802 2.312v3.349h-2.483v-3.35c0-1.536-.602-2.312-1.802-2.312-1.085 0-1.628.655-1.628 1.944v6.119H4.832V8.284c0-1.289.328-2.313.987-3.07.68-.758 1.569-1.146 2.674-1.146 1.278 0 2.246.491 2.886 1.474L12 6.585l.622-1.043c.64-.983 1.608-1.474 2.886-1.474 1.104 0 1.994.388 2.674 1.146.658.757.986 1.781.986 3.07v6.304z"/>
                                    </svg>"""
                            }
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
                }
                li {
                    div {
                        class' "flex"

                        childContent [
                            div {
                                class' "w-10"

                                i {
                                    html.raw
                                        """<svg role="img" xmlns="http://www.w3.org/2000/svg" width="24px" height="24px" viewBox="0 0 8 8">
                                            <circle cx="2" cy="6" r="1" class="symbol" fill="#2c2e43"/>
                                            <path fill="#2c2e43" d="M1 4a3 3 0 013 3h1a4 4 0 00-4-4z" class="symbol"/>
                                            <path fill="#2c2e43" d="M1 2a5 5 0 015 5h1a6 6 0 00-6-6z" class="symbol"/>
                                        </svg>"""

                                }
                            }
                            div {
                                childContent [
                                    a {
                                        class' "underline"
                                        href "/rss"
                                        "RSS"
                                    }

                                    span { " | " }

                                    a {
                                        class' "underline"
                                        href "/atom"
                                        "ATOM"
                                    }
                                ]
                            }
                        ]
                    }
                }
            ]
        }
    }
