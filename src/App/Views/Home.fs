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
                header [ _class "Main-header" ] [ Text pageTitle ]
                nav [ _class "Main-sideNav" ] [
                    span [] [ Text "eabrg klsrhje gkjhesr gkjhe" ]
                ]
                div [ _class "Main-content" ] content
            ]
            script [ _async; _defer; _src "js/tips.js"; _type "text/javascript" ] []
        ]
