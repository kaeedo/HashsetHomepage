namespace App

open System.Security.Claims
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Components.Authorization
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Authentication
open Supabase.Gotrue
open Supabase.Gotrue.Interfaces

type AuthService
    (
        client: Supabase.Client,
        customAuthStateProvider: AuthenticationStateProvider,
        supabaseSessionHandler: IGotrueSessionPersistence<Session>,
        accessor: IHttpContextAccessor
    ) =
    member _.Login(email: string, password: string) =
        task {
            let! session = client.Auth.SignIn(email, password)
            let! authState = customAuthStateProvider.GetAuthenticationStateAsync()
            supabaseSessionHandler.SaveSession(session)

            let claimsPrincipal: ClaimsPrincipal = authState.User

            do! accessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal)

            return session.AccessToken
        }

    member _.Logout() =
        task {
            do! client.Auth.SignOut()

            let! authState = customAuthStateProvider.GetAuthenticationStateAsync()
            supabaseSessionHandler.DestroySession()

            do! accessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)

            return ()
        }

    member _.GetUser() =
        task {
            let! authState = customAuthStateProvider.GetAuthenticationStateAsync()

            let! session =
                if isNull <| supabaseSessionHandler.LoadSession() then
                    client.Auth.RetrieveSessionAsync()
                else
                    task { return supabaseSessionHandler.LoadSession() }

            return session.User
        }
