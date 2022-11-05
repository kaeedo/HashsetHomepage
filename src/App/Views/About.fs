namespace Hashset.Views

module About =
    open Giraffe.ViewEngine

    let view =
        div [ _class "PostContents" ] [
            h1 [] [
                str "Hello, I'm Kai Ito"
            ]
            p [] [
                str "I am a Software Developer at "
                a [ _href "https://valora.digital/"
                    _target "_blank" ] [
                    str "Valora Digital"
                ]
                str
                    ". I have been a Full Stack .Net web developer since 2012. Back then, I mainly used Asp.net MVC, and mostly jQuery on the frontend, with a little bit of Knockout.js sprinkled in.
                I have since taken a liking to Functional Programming, especially F#. In my experience, the functional paradigm has since proven to be invaluable in writing bug free software, especially when combined with a powerful type system like F#"
            ]
            p [] [
                str
                    "These days, I have become an advocate of online privacy, especially because of the massive amounts of tracking that a large number websites and smartphone apps practice. I strongly believe in Open Source Software, and try to use them as much as possible.
                You'll find a link to my Mastodon account below, and not to a Twitter of Facebook account for these reasons. For the moment, I'm still on Github, but ideally in the future, something like Gitea will allow federated communication."
            ]
            p [] [
                str "Come find me at the following sites:"
                ul [ _class "About-socialList" ] [
                    li [ _class "About-socialLink" ] [
                        a [ _href "https://github.com/kaeedo/"
                            _class "About-socialAnchor" ] [
                            i [ _class "About-socialIcon" ] [
                                rawText
                                    """<svg width="32px" height="32px" role="img" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><title>GitHub icon</title><path d="M12 .297c-6.63 0-12 5.373-12 12 0 5.303 3.438 9.8 8.205 11.385.6.113.82-.258.82-.577 0-.285-.01-1.04-.015-2.04-3.338.724-4.042-1.61-4.042-1.61C4.422 18.07 3.633 17.7 3.633 17.7c-1.087-.744.084-.729.084-.729 1.205.084 1.838 1.236 1.838 1.236 1.07 1.835 2.809 1.305 3.495.998.108-.776.417-1.305.76-1.605-2.665-.3-5.466-1.332-5.466-5.93 0-1.31.465-2.38 1.235-3.22-.135-.303-.54-1.523.105-3.176 0 0 1.005-.322 3.3 1.23.96-.267 1.98-.399 3-.405 1.02.006 2.04.138 3 .405 2.28-1.552 3.285-1.23 3.285-1.23.645 1.653.24 2.873.12 3.176.765.84 1.23 1.91 1.23 3.22 0 4.61-2.805 5.625-5.475 5.92.42.36.81 1.096.81 2.22 0 1.606-.015 2.896-.015 3.286 0 .315.21.69.825.57C20.565 22.092 24 17.592 24 12.297c0-6.627-5.373-12-12-12"/></svg>"""
                            ]
                            span [ _class "About-socialText" ] [
                                str "Github"
                            ]
                        ]
                    ]
                    li [ _class "About-socialLink" ] [
                        a [ _rel "me"
                            _href "https://mstdn.social/@kaeedo"
                            _class "About-socialAnchor" ] [
                            i [ _class "About-socialIcon" ] [
                                rawText
                                    """<svg width="32px" height="32px" role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><title>Mastodon icon</title><path d="M23.193 7.879c0-5.206-3.411-6.732-3.411-6.732C18.062.357 15.108.025 12.041 0h-.076c-3.068.025-6.02.357-7.74 1.147 0 0-3.411 1.526-3.411 6.732 0 1.192-.023 2.618.015 4.129.124 5.092.934 10.109 5.641 11.355 2.17.574 4.034.695 5.535.612 2.722-.15 4.25-.972 4.25-.972l-.09-1.975s-1.945.613-4.129.539c-2.165-.074-4.449-.233-4.799-2.891a5.499 5.499 0 0 1-.048-.745s2.125.52 4.817.643c1.646.075 3.19-.097 4.758-.283 3.007-.359 5.625-2.212 5.954-3.905.517-2.665.475-6.507.475-6.507zm-4.024 6.709h-2.497V8.469c0-1.29-.543-1.944-1.628-1.944-1.2 0-1.802.776-1.802 2.312v3.349h-2.483v-3.35c0-1.536-.602-2.312-1.802-2.312-1.085 0-1.628.655-1.628 1.944v6.119H4.832V8.284c0-1.289.328-2.313.987-3.07.68-.758 1.569-1.146 2.674-1.146 1.278 0 2.246.491 2.886 1.474L12 6.585l.622-1.043c.64-.983 1.608-1.474 2.886-1.474 1.104 0 1.994.388 2.674 1.146.658.757.986 1.781.986 3.07v6.304z"/></svg>"""
                            ]
                            span [ _class "About-socialText" ] [
                                str "Mastodon"
                            ]
                        ]
                    ]
                    li [ _class "About-socialLink" ] [
                        i [ _class "About-socialIcon" ] [
                            rawText
                                """<svg role="img" xmlns="http://www.w3.org/2000/svg" width="32px" height="32px" viewBox="0 0 8 8"><circle cx="2" cy="6" r="1" class="symbol" fill="#2c2e43"/><path fill="#2c2e43" d="M1 4a3 3 0 013 3h1a4 4 0 00-4-4z" class="symbol"/><path fill="#2c2e43" d="M1 2a5 5 0 015 5h1a6 6 0 00-6-6z" class="symbol"/></svg>"""
                        ]
                        a [ _href "/rss"
                            _class "About-socialAnchor" ] [
                            str "RSS"
                        ]
                        span [ _class "Main-footerSyndicateSeparator" ] [
                            str "|"
                        ]
                        a [ _href "/atom"
                            _class "About-socialAnchor" ] [
                            str "Atom"
                        ]
                    ]
                ]
            ]
        ]
