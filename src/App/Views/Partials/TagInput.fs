[<RequireQualifiedAccess>]
module App.Views.Partials.TagInput

open Fun.Blazor
open Fun.Htmx

let simple (tag: string) =
    li {
        class' "mb-2"

        input {
            class' "border-2 rounded-sm px-4 py-2 mr-4"
            type' "text"
            name "Tags"
            value tag
        }

        input {
            class' "border-2 rounded-sm grow px-2 bg-gray"
            type' "button"
            hxOn "click" "this.parentElement.remove()"
            value "-"
        }
    }
