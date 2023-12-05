namespace Hashset.OldViews

module About =
    open Giraffe.ViewEngine

    let view =
        div [ _class "PostContents" ] [
            p [] [
                str "Come find me at the following sites:"
                ul [ _class "About-socialList" ] [
                    span [ _class "About-socialText" ] [ str "Codeberg" ]
                    li [ _class "About-socialLink" ] [
                        a [
                            _href "https://github.com/kaeedo/"
                            _class "About-socialAnchor"
                        ] [
                            span [ _class "About-socialText" ] [ str "Github" ]
                        ]
                    ]
                    li [ _class "About-socialLink" ] [
                        a [
                            _rel "me"
                            _href "https://mstdn.social/@kaeedo"
                            _class "About-socialAnchor"
                        ] [
                            span [ _class "About-socialText" ] [ str "Mastodon" ]
                        ]
                    ]
                    li [ _class "About-socialLink" ] [
                        i [ _class "About-socialIcon" ] [
                            rawText
                                """<svg role="img" xmlns="http://www.w3.org/2000/svg" width="32px" height="32px" viewBox="0 0 8 8"><circle cx="2" cy="6" r="1" class="symbol" fill="#2c2e43"/><path fill="#2c2e43" d="M1 4a3 3 0 013 3h1a4 4 0 00-4-4z" class="symbol"/><path fill="#2c2e43" d="M1 2a5 5 0 015 5h1a6 6 0 00-6-6z" class="symbol"/></svg>"""
                        ]
                        a [
                            _href "/rss"
                            _class "About-socialAnchor"
                        ] [ str "RSS" ]
                        span [
                            _class "Main-footerSyndicateSeparator"
                        ] [ str "|" ]
                        a [
                            _href "/atom"
                            _class "About-socialAnchor"
                        ] [ str "Atom" ]
                    ]
                ]
            ]
        ]
