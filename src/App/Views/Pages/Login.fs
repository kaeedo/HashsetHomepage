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
                        class' "flex mb-4"

                        label {
                            class' "w-40 self-center"
                            for' "username"
                            "Username"
                        }

                        input {
                            type' "text"
                            name "username"
                            class' "border-2 rounded-sm grow px-4 py-2"
                        }
                    }

                    div {
                        class' "flex mb-4"

                        label {
                            class' "w-40 self-center"
                            for' "password"
                            "Password"
                        }

                        input {
                            type' "password"
                            name "password"
                            class' "border-2 rounded-sm grow px-4 py-2"
                        }
                    }

                    input {
                        class' "border-2 rounded-sm grow px-2 bg-gray"
                        type' "Submit"
                        value "Login"
                    }
                }
            }
            |> Card.simple ""
        ])

let view () = Layout.Create(h1 { "Login" }, html)
