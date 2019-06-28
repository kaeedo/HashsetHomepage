namespace App


open Elmish
open Elmish.Debug
open Elmish.Navigation
open Elmish.UrlParser
open Elmish.HMR
open Fable.React
open Fable.React.Props

type Model = { Count: int; CurrentPage: Page }

type Msg =
    | Increment
    | Decrement

module App =
    let urlUpdate (result: Option<Page>) model =
        match result with
        | None ->
            model, Router.modifyUrl model.CurrentPage
        | Some page ->
            let model = { model with CurrentPage = page }
            match page with
            | Home ->
                model, Cmd.none

    let init result =
        urlUpdate result { Count = 0; CurrentPage = Home }

    let update (msg: Msg) (model: Model) =
        match msg with
        | Increment -> { model with Count = model.Count + 1 }, Cmd.none
        | Decrement -> { model with Count = model.Count - 1 }, Cmd.none

    // VIEW (rendered with React)

    let view (model:Model) dispatch =
        div []
            [ button [ OnClick (fun _ -> dispatch Increment) ] [ str "+" ]
              div [] [ str (string model) ]
              button [ OnClick (fun _ -> dispatch Decrement) ] [ str "-" ] ]

    Program.mkProgram init update view
    |> Program.toNavigable (parseHash Router.pageParser) urlUpdate
    #if DEBUG
    |> Program.withConsoleTrace
    #endif
    |> Program.withReactSynchronous "elmish-app"
    #if DEBUG
    |> Program.withDebugger
    #endif
    |> Program.run
