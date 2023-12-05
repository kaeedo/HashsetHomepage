[<RequireQualifiedAccess>]
module App.Views.Components.Button

open Fun.Blazor

let round (label: string) link =
    a {
        class'
            "px-4 py-2 border-[3px] bg-green rounded-full border-black text-xl font-extrabold drop-shadow-[4px_4px_0px_#000] active:drop-shadow-[1px_2px_0px_#000] active:translate-y-1 active:translate-x-1"

        href "#"
        label
    }
