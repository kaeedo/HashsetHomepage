namespace App.Views


module Home =
    open Giraffe.GiraffeViewEngine

    let private pageTitle = "Hashset"

    let view content =
        html [ ] [
            head [] [
                title [] [ str pageTitle ]
                link [ _rel "stylesheet"; _type "text/css"; _href "css/styles.css" ]
            ]
            body [ _class "Main-body" ] [
                header [ _class "Main-header" ] [
                    div [ _class "Main-headerTitle" ] [
                        h1 [] [ Text pageTitle ]
                    ]
                ]
                section [ _class "Main-section" ] [
                    nav [ _class "Main-sideNav" ] [
                        ul [ _class "Main-links Card" ] [
                            li [] [ Text "Home" ]
                            li [] [ Text "About" ]
                            li [] [ Text "Projects" ]
                            li [] [ Text "Github" ]
                        ]
                    ]
                    article [ _class "Main-content Card" ] content
                ]
            ]
            script [ _async; _defer; _src "js/tips.js"; _type "text/javascript" ] []
        ]
