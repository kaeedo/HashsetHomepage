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
                    h1 { "Hello, I'm Kai Ito" }
                    p {
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
                    h4 { "Come find me at the following sites" }
                    //     """
                    //     <svg width="16" height="16" version="1.1" viewBox="0 0 4.2333 4.2333" xmlns="http://www.w3.org/2000/svg" xmlns:cc="http://creativecommons.org/ns#" xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#" xmlns:xlink="http://www.w3.org/1999/xlink">
                    //     <title>Codeberg logo</title>
                    //     <defs>
                    //     <linearGradient id="a" x1="42519" x2="42575" y1="-7078.8" y2="-6966.9" gradientUnits="userSpaceOnUse">
                    //     <stop stop-color="#2185d0" stop-opacity="0" offset="0"/>
                    //     <stop stop-color="#2185d0" stop-opacity=".3" offset=".49517"/>
                    //     <stop stop-color="#2185d0" stop-opacity=".3" offset="1"/>
                    //     </linearGradient>
                    //     </defs>
                    //     <g transform="matrix(.065514 0 0 .065514 -2.2324 -1.4318)">
                    //     <path transform="matrix(.37058 0 0 .37058 -15690 2662.1)" d="m42519-7078.8a0.76087 0.56792 0 0 0-0.738 0.6739l33.586 125.89a87.182 87.182 0 0 0 39.381-33.764l-71.565-92.52a0.76087 0.56792 0 0 0-0.664-0.2793z" fill="url(#a)" stop-color="#000000" style="font-variation-settings:normal;paint-order:stroke markers fill"/>
                    //     <path transform="matrix(1.4006 0 0 1.4006 -15690 2662.1)" d="m11249-1883.7c-12.74 0-23.067 10.328-23.067 23.067 0 4.3335 1.22 8.5795 3.522 12.251l19.232-24.864c0.138-0.1796 0.486-0.1796 0.624 0l19.233 24.865c2.302-3.6721 3.523-7.9185 3.523-12.252 0-12.74-10.327-23.067-23.067-23.067z" fill="#2185d0" stop-color="#000000" stroke-width="17.006" style="paint-order:markers fill stroke"/>
                    //     </g>
                    //     </svg>
                    //     """
                    // ]
                    |> Card.simple ""
                ]
            ]
        }
    ]

let view () = Layout.Create("About", html)
