namespace App

open System

open Browser
open Fable.React.Props
open Elmish.Navigation
open Elmish.UrlParser

type Page =
    | Home

module Router =
    let private toHash page =
        match page with
        | Home -> "#/"

    let pageParser: Parser<Page->Page, Page> =
        map Home (s String.Empty)

    let href route =
        Href (toHash route)

    let modifyUrl route =
        route |> toHash |> Navigation.modifyUrl

    let newUrl route =
        route |> toHash |> Navigation.newUrl

    let modifyLocation route =
        window.location.href <- toHash route
