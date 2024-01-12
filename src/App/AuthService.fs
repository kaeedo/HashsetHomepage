namespace App

open System.Security.Claims
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Components.Authorization
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Authentication

type AuthService
    (client: Supabase.Client, customAuthStateProvider: AuthenticationStateProvider, accessor: IHttpContextAccessor) =
    member _.Login(email: string, password: string) =
        task {
            let! _ = client.Auth.SignIn(email, password)
            let! authState = customAuthStateProvider.GetAuthenticationStateAsync()

            let claimsPrincipal: ClaimsPrincipal = authState.User

            do! accessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal)

            return ()
        }

    member _.Logout() =
        task {
            do! client.Auth.SignOut()

            let! _ = customAuthStateProvider.GetAuthenticationStateAsync()

            do! accessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)

            return ()
        }
