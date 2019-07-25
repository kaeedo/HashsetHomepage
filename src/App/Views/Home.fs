namespace App.Views


module Home =
    open Giraffe.GiraffeViewEngine

    let view content =
        html [ _style "background-color: #353535" ] [
            head [] [
                title [] [ str "Hashset" ]
                link [ _rel "stylesheet"; _type "text/css"; _href "css/styles.css" ]
            ]
            body [] [
                div [ _style "color: #3c6e71; width: 100%; height: auto; bottom: 0px; top: 0px; left: 0; position: absolute" ] [
                    div [ _style "position: absolute; width: 56%; top: 50%; left: 50%; transform: translate(-50%, -50%);" ] content
                ]
            ]
            script [ _async; _defer; _src "js/tips.js"; _type "text/javascript" ] []
        ]
