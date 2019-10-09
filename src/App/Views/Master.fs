namespace Hashset.Views

open System

open Hashset

module Master =
    open Giraffe.GiraffeViewEngine

    let private getDate (date: DateTime option) =
        match date with
        | Some d -> d.ToString("dd MMMM, yyyy")
        | None -> String.Empty


    let view (aboveTheFold: string) (masterData: MasterContent) (content: XmlNode) =
        html [ ] [
            head [] [
                title [] [ str <| sprintf "%s - Hashset.dev" masterData.PageTitle ]
                style [] [ str aboveTheFold ]
                link [ _rel "stylesheet"; _type "text/css"; _href "/css/styles.css"; _async ]
                meta [ _charset "utf-8" ]
                meta [ _name "viewport"; _content "width=device-width, initial-scale=1.0" ]
            ]
            body [ _class "Main-body" ] [
                header [ _class "Main-headerContainer" ] [
                    div [ _class "Main-header" ] [
                        span [] [ str "Kai Ito" ]
                        nav [ _class "Main-headerNav" ] [
                            a [ _class "Main-headerLink"; _href "/" ] [
                                str "Home"
                            ]
                            a [ _class "Main-headerLink"; _href "/posts" ] [
                                str "Posts"
                            ]
                            a [ _class "Main-headerLink"; _href "/about" ] [
                                str "About"
                            ]
                        ]
                    ]

                    div [ _class "Main-titleContainer" ] [
                        div [ _class "Main-title" ] [
                            div [ _class "Main-postTitle" ] [ str masterData.PageTitle ]
                            div [ _class "Main-postDate" ] [ str <| getDate masterData.ArticleDate ]
                            // div [ _class "Main-postTags" ] [ str "Tags" ]
                        ]
                    ]

                    div [ _id "headerBackground" ] []
                ]

                section [ _class "Main-section" ] [
                    content
                ]
                footer [ _class "Main-footer" ] [
                    div [ _class "Main-footerContents" ] [
                        span [] [ str <| sprintf "Blog Posts Copyright © %i" DateTime.Now.Year ]
                        span [] [
                            str "Source Code to website available "
                            a [ _class "Main-footerCodeLink"; _href "https://github.com/kaeedo/HashsetHomepage" ] [ str "here" ]
                        ]
                        span [ _class "Main-footerSocialLink" ] [
                            i [ _class "About-socialIcon" ] [ rawText """<svg width="32px" height="32px" fill="#f2f2f2" role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><title>Mastodon icon</title><path d="M23.193 7.879c0-5.206-3.411-6.732-3.411-6.732C18.062.357 15.108.025 12.041 0h-.076c-3.068.025-6.02.357-7.74 1.147 0 0-3.411 1.526-3.411 6.732 0 1.192-.023 2.618.015 4.129.124 5.092.934 10.109 5.641 11.355 2.17.574 4.034.695 5.535.612 2.722-.15 4.25-.972 4.25-.972l-.09-1.975s-1.945.613-4.129.539c-2.165-.074-4.449-.233-4.799-2.891a5.499 5.499 0 0 1-.048-.745s2.125.52 4.817.643c1.646.075 3.19-.097 4.758-.283 3.007-.359 5.625-2.212 5.954-3.905.517-2.665.475-6.507.475-6.507zm-4.024 6.709h-2.497V8.469c0-1.29-.543-1.944-1.628-1.944-1.2 0-1.802.776-1.802 2.312v3.349h-2.483v-3.35c0-1.536-.602-2.312-1.802-2.312-1.085 0-1.628.655-1.628 1.944v6.119H4.832V8.284c0-1.289.328-2.313.987-3.07.68-.758 1.569-1.146 2.674-1.146 1.278 0 2.246.491 2.886 1.474L12 6.585l.622-1.043c.64-.983 1.608-1.474 2.886-1.474 1.104 0 1.994.388 2.674 1.146.658.757.986 1.781.986 3.07v6.304z"/></svg>""" ]
                            a [ _rel "me"; _href "https://mastodon.technology/@kaeedo"; _class "Main-footerSocialAnchor" ] [ str "Follow me on Mastodon" ]
                        ]
                    ]
                ]
            ]

            script [ _async; _defer; _src "/js/particles.min.js"; _type "text/javascript"; ] []
            script [ _async; _defer; _src "/js/tips.js"; _type "text/javascript" ] []
            script [ _async; _defer; _src "/js/main.js"; _type "text/javascript"; ] []
        ]
