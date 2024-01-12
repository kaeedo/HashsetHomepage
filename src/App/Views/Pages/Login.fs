[<RequireQualifiedAccess>]
module App.Views.Pages.Login

open Fun.Blazor
open App.Views.Components
open App.Views

let private html =
    html.fragment [
        div {
            form {
                action "/login"
                method "POST"
                enctype "multipart/form-data"

                div {
                    input {
                        placeholder "Username"

                    }
                }

                div {
                    input {
                        placeholder "Password"
                        type' "password"
                    }
                }

                div {
                    input {
                        type' "Submit"
                        value "Login"
                    }
                }
            }
        }
        |> Card.simple ""
    ]

let view () = Layout.Create(h1 { "Login" }, html)
