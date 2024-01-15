[<RequireQualifiedAccess>]
module App.Views.Components.Card

open Fun.Blazor

let simple (additionalClasses: string) (cardContent: NodeRenderFragment) =
    div {
        class' $"{additionalClasses} p-4 bg-white border-4 border-black drop-shadow-[8px_8px_0px_#ff5dfd] max-h-fit"

        cardContent
    }
