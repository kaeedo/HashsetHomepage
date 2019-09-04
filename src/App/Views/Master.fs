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
                    div [ _class "Main-headerContainer" ] [
                        div [ _class "Main-headerAbout"] [
                            span [ _class "Main-headerName" ] [ str masterData.Author ]
                            span [ _class "Main-headerJob" ] [ str masterData.JobTitle ]
                        ]
                        div [ _class "Main-headerTitle" ] [
                            h1 [ _class "Main-postTitle" ] [ str masterData.PageTitle ]
                            h6 [ _class "Main-postInfo" ] [
                                span [ _class "Main-postedDate" ] [ str (masterData.ArticleDate) ]
                                span [ _class "Main-pageTitle" ] [ str pageTitle ]
                            ]
                        ]
                    ]
                ]
                section [ _class "Main-section" ] [
                    nav [ _class "Main-sideNav" ] [
                        ul [ _class "Main-links Card" ] [
                            li [ _class "Main-link" ] [
                                span [ _class (sprintf "Main-linkText %s" (if masterData.PageTitle = "Home" then "Main-linkSelected" else String.Empty)) ] [ str "Home" ]
                            ]
                            li [ _class "Main-link" ] [
                                span [ _class (sprintf "Main-linkText %s" (if masterData.PageTitle = "Posts" then "Main-linkSelected" else String.Empty)) ] [ str "Posts" ]
                            ]
                            li [ _class "Main-link" ] [
                                span [ _class (sprintf "Main-linkText %s" (if masterData.PageTitle = "About" then "Main-linkSelected" else String.Empty)) ] [ str "About" ]
                            ]
                        ]
                    ]
                    article [ _class "Main-content Card" ] [ content ]
                ]
            ]
            script [ _async; _defer; _src "js/tips.js"; _type "text/javascript" ] []
        ]
