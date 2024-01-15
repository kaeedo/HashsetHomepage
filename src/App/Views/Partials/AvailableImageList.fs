[<RequireQualifiedAccess>]
module App.Views.Partials.AvailableImageList

open Fun.Blazor
open Fun.Htmx

let view (images: string list) =
    div {
        id "AvailableImageList"
        class' "max-h-204 overflow-y-auto"

        childContent (
            div {
                id "indicator"
                class' "htmx-indicator loader"
            }
            :: form {
                class' "flex mb-4"
                action "/images"
                method "POST"
                enctype "multipart/form-data"
                hxTarget "#AvailableImageList"
                hxSwap_outerHTML
                hxPushUrl false

                input {
                    type' "file"
                    id "Images"
                    name "Images"
                    multiple true
                    class' "border-2 rounded-sm grow px-4 py-2 mr-2"
                }

                button {
                    hxIndicator "#indicator"
                    class' "border-2 rounded-sm grow px-2 bg-gray"

                    "Add"
                }
            }
            :: (images
                |> List.map (fun image ->
                    div {
                        class' "relative mb-2"

                        div {
                            class' "absolute flex right-2"

                            button {
                                class' "mr-1 border-2 rounded-sm grow px-2 bg-gray"
                                type' "button"
                                hxOn "click" $"navigator.clipboard.writeText('{image}');"
                                "Copy"
                            }

                            button {
                                class' " border-2 rounded-sm grow px-2 bg-gray"
                                type' "button"
                                hxConfirm "Are you sure you wish to delete this image?"
                                hxDelete $"/images/{image.Split('/') |> Array.last}"
                                hxTarget "#AvailableImageList"
                                hxSwap_outerHTML
                                "X"
                            }
                        }

                        label { image }

                        img { src image }
                    }))
        )
    }
