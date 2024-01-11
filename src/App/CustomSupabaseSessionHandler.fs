namespace App

open System.Text.Json
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc.ViewFeatures
open Supabase.Gotrue
open Supabase.Gotrue.Interfaces

type CustomSupabaseSessionHandler(accessor: IHttpContextAccessor, tempDataFactory: ITempDataDictionaryFactory) =
    let tempData = tempDataFactory.GetTempData(accessor.HttpContext)
    let sessionKey = "SUPABASE_SESSION"

    interface IGotrueSessionPersistence<Session> with
        member this.DestroySession() =
            let _ = tempData.Remove(sessionKey)
            tempData.Save()

        member this.LoadSession() =
            let session = tempData.Peek(sessionKey).ToString()
            tempData.Save()
            let session = JsonSerializer.Deserialize<Session>(session)

            session
        // if
        //     not <| isNull session
        //     && session.ExpiresAt() <= DateTime.Now
        // then
        //     null
        // else
        //     session

        member this.SaveSession(session) =
            let serialized = JsonSerializer.Serialize(session)
            tempData[sessionKey] <- serialized
            tempData.Save()
