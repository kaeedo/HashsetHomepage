[<RequireQualifiedAccess>]
module App.Views.Pages.Login

open Fun.Blazor
open App.Views.Components
open App.Views
open Microsoft.AspNetCore.Http

let private html =
    html.inject (fun (accessor: IHttpContextAccessor) ->
        html.fragment [
            div {
                form {
                    action "/login"
                    method "POST"

                    input {
                        type' "hidden"

                        value (
                            accessor.HttpContext.Request.Query["ReturnUrl"]
                                .ToString()
                        )

                        name "returnUrl"
                    }

                    div {
                        input {
                            placeholder "Username"
                            name "username"
                        }
                    }

                    div {
                        input {
                            placeholder "Password"
                            type' "password"
                            name "password"
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
        ])

let view () = Layout.Create(h1 { "Login" }, html)
