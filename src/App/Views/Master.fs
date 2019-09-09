namespace Hashset.Views

open System

type MasterContent =
    { Author: string
      JobTitle: string
      PageTitle: string
      ArticleDate: string }

module Master =
    open Giraffe.GiraffeViewEngine

    let private pageTitle = "hashset.dev"

    let view (masterData: MasterContent) (content: XmlNode) =
        html [ ] [
            head [] [
                title [] [ str masterData.PageTitle ]
                link [ _rel "stylesheet"; _type "text/css"; _href "css/styles.css" ]
            ]
            body [ _class "Main-body" ] [
                header [ _class "Main-header" ] [
                    div [ _class "Main-headerTopBar" ] [
                        span [] [ str masterData.Author ]
                        nav [ _class "Main-headerNav" ] [
                            a [ _class "Main-headerLink" ] [
                                str "Home"
                            ]
                            a [ _class "Main-headerLink" ] [
                                str "Posts"
                            ]
                            a [ _class "Main-headerLink" ] [
                                str "About"
                            ]
                        ]
                    ]
                ]

                div [ _class "Main-titleContainer" ] [
                    div [ _class "Main-title" ] [
                        div [ _class "Main-postTitle" ] [ str masterData.PageTitle ]
                        div [ _class "Main-postDate" ] [ str masterData.ArticleDate ]
                        // div [ _class "Main-postTags" ] [ str "Tags" ]
                    ]
                ]

                section [ _class "Main-section" ] [
                    article [ _class "Main-content Card" ] [ content ]
                ]
            ]
            script [ _async; _defer; _src "js/tips.js"; _type "text/javascript" ] []
        ]
