namespace App.Views


module Home =
    open Giraffe.GiraffeViewEngine

    let view =
        html [ _style "background-color: #353535" ] [
            head [] [
                title [] [ str "Hashset" ]
            ]
            body [] [
                div [ _style "color: #3c6e71; font-size: 15rem; width: 100%; height: auto; bottom: 0px; top: 0px; left: 0; position: absolute" ] [
                    div [ _style "position: absolute; width: 56%; top: 50%; left: 50%; transform: translate(-50%, -50%); font-family: Tahoma" ] [ str "Coming Soon" ]
                ]
            ]
        ]
