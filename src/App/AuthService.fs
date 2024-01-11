namespace App

open Microsoft.AspNetCore.Components.Authorization
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc.ViewFeatures
open Supabase.Gotrue
open Supabase.Gotrue.Interfaces

type AuthService
    (
        client: Supabase.Client,
        customAuthStateProvider: AuthenticationStateProvider,
        supabaseSessionHandler: IGotrueSessionPersistence<Session>,
        accessor: IHttpContextAccessor,
        tempDataFactory: ITempDataDictionaryFactory
    ) =
    let tempData = tempDataFactory.GetTempData(accessor.HttpContext)

    member _.Login(email: string, password: string) =
        task {
            let! session = client.Auth.SignIn(email, password)
            let! authState = customAuthStateProvider.GetAuthenticationStateAsync()
            supabaseSessionHandler.SaveSession(session)
            return ()
        }

    member _.Logout() =
        task {
            do! client.Auth.SignOut()

            tempData.Remove("token") |> ignore
            tempData.Save()
            let! authState = customAuthStateProvider.GetAuthenticationStateAsync()
            supabaseSessionHandler.DestroySession()
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
