namespace App.Views


module Home =
    open Giraffe.GiraffeViewEngine

    let private pageTitle = "Hashset.dev"


    let view content =
        html [ ] [
            head [] [
                title [] [ str pageTitle ]
                link [ _rel "stylesheet"; _type "text/css"; _href "css/styles.css" ]
            ]
            body [ _class "Main-body" ] [
                header [ _class "Main-header" ] [
                    div [ _class "Main-headerContainer" ] [
                        div [ _class "Main-headerAbout"] [
                            span [ _class "Main-headerName" ] [ str "Kai Ito" ]
                            span [] [ str "Software Developer" ]
                        ]
                        div [ _class "Main-headerTitle" ] [
                            h1 [] [ str pageTitle ]
                        ]
                    ]
                ]
                section [ _class "Main-section" ] [
                    nav [ _class "Main-sideNav" ] [
                        ul [ _class "Main-links Card" ] [
                            li [ _class "Main-link" ] [ str "Hofme" ]
                            li [ _class "Main-link" ] [ str "About" ]
                            li [ _class "Main-link" ] [ str "Projects" ]
                            li [ _class "Main-link" ] [ str "Github" ]
                        ]
                    ]
                    article [ _class "Main-content Card" ] content
                ]
            ]
            script [ _async; _defer; _src "js/tips.js"; _type "text/javascript" ] []
        ]
