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
                style [] [
                    str "
                    html {
                      overflow-y: scroll;
                      font-family: var(Arial, sans-serif);
                    }

                    a {
                      text-decoration: none;
                    }

                    .Main-body {
                      background: #eee;
                      box-sizing: border-box;
                    }

                    .Main-header {
                      position: fixed;
                      color: #f2f2f2;

                      display: flex;
                      justify-content: space-between;
                      width: calc(100% - 8px);

                      transition: background-color 0.5s;
                      transition: color 0.1s;
                      backface-visibility: hidden;
                      background: transparent;

                      margin-left: -8px;
                      padding: 8px;
                      font-size: 20pt;
                      font-weight: 700;
                    }
                    .Main-headerNav {
                      display: grid;
                      grid-template-columns: 1fr 1fr 1fr;
                      grid-column-gap: 2rem;
                      font-weight: normal;
                      padding-right: 8px;
                    }

                    @media screen and (max-width: 667px) {
                        .Main-headerNav {
                            grid-column-gap: 1rem;
                          }
                      }

                    .Main-headerLink {
                      color: inherit;
                    }

                    #headerBackground {
                      position: absolute;
                      top: 0;
                      z-index: -54654654654;

                      width: 100%;
                      background-color: #2c2e43;
                      background-size: cover;

                      height: 300px;
                      margin-left: calc(-1 * 8px);
                      margin-right: calc(-1 * 8px);
                    }

                    .Main-titleContainer {
                      margin-top: calc(-1 * 8px);
                      margin-left: calc(-1 * 8px);
                      margin-right: calc(-1 * 8px);
                      margin-bottom: 2rem;
                      display: flex;
                      padding: 7rem 0;
                    }
                    .Main-title {
                      margin: auto;
                      padding: 8px;
                      display: flex;
                      flex-direction: column;
                      justify-content: space-between;
                      color: #f2f2f2;
                    }
                    .Main-postTitle {
                      font-size: 42pt;
                      font-weight: 700;
                      max-width: 80rem;
                    }

                    @media screen and (max-width: 667px) {
                        .Main-postTitle {
                            font-size: 22pt;
                          }
                      }
                    .Main-section {
                      margin: auto;
                    }
                    .PostContents {
                      display: flex;
                      flex-direction: column;
                      margin: auto;
                      max-width: 80rem;
                    }
                    "
                ]
                link [ _rel "stylesheet"; _type "text/css"; _href "/css/styles.css" ]
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
            ]
            script [ _async; _defer; _src "/js/particles.min.js"; _type "text/javascript"; ] []
            script [ _async; _defer; _src "/js/tips.js"; _type "text/javascript" ] []
            script [ _async; _defer; _src "/js/main.js"; _type "text/javascript"; ] []
        ]
