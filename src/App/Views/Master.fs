namespace Hashset.Views

open System

open Hashset

module Master =
    open Giraffe.GiraffeViewEngine

    let private getDate (date: DateTime option) =
        match date with
        | Some d -> d.ToString("dd MMMM, yyyy")
        | None -> String.Empty


    let view (masterData: MasterContent) (content: XmlNode) =
        html [ ] [
            head [] [
                title [] [ str <| sprintf "%s - Hashset.dev" masterData.PageTitle ]
                link [ _rel "stylesheet"; _type "text/css"; _href "/css/styles.css" ]
                meta [ _charset "utf-8" ]
                meta [ _name "viewport"; _content "width=device-width, initial-scale=1.0" ]
            ]
            body [ _class "Main-body" ] [
                header [ _class "Main-header" ] [
                    span [] [ str "Kai Itof" ]
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

                section [ _class "Main-section" ] [
                    content
                ]
            ]
            script [ _async; _defer; _src "/js/particles.min.js"; _type "text/javascript"; ] []
            script [ _async; _defer; _src "/js/tips.js"; _type "text/javascript" ] []
            script [ _async; _defer; _src "/js/main.js"; _type "text/javascript"; ] []
        ]
