[<RequireQualifiedAccess>]
module App.Views.Pages.Upsert

open App.Views.Components
open Fun.Blazor
open App.Views
open Model

let private upsertForm (upsertDocument: UpsertDocument) =
    form {
        action "/upsert"
        method "POST"
        enctype "multipart/form-data"

        childContent [
            div {
                class' "flex mb-4"

                label {
                    class' "w-40 self-center"
                    for' "Id"
                    "Id"
                }

                select {
                    id "Id"
                    name "Id"
                    class' "border-2 rounded-sm grow px-4 py-2"

                    childContent [
                        option {
                            value 0
                            "New"
                        }

                        yield!
                            (upsertDocument.ExistingIds
                             |> Seq.sortByDescending (fun (id, _) -> id)
                             |> Seq.map (fun (idd, title) ->
                                 option {
                                     value (idd.ToString())
                                     selected (title = upsertDocument.Title)

                                     title
                                 }))
                    ]
                }
            }

            div {
                class' "flex mb-4"

                label {
                    class' "w-40 self-center"
                    for' "ArticleDate"
                    "Article Date"
                }

                input {
                    type' "date"
                    id "ArticleDate"
                    name "ArticleDate"
                    class' "border-2 rounded-sm grow px-4 py-2"

                    value (upsertDocument.ArticleDate.ToString("yyyy-MM-dd"))
                }
            }

            div {
                class' "flex mb-4"

                label {
                    class' "w-40 self-center"
                    for' "Title"
                    "Title"
                }

                input {
                    type' "text"
                    id "Title"
                    name "Title"
                    class' "border-2 rounded-sm grow px-4 py-2"

                    value upsertDocument.Title
                }
            }

            div {
                class' "flex mb-4"

                label {
                    class' "w-40 self-center"
                    for' "Description"
                    "Description"
                }

                input {
                    type' "text"
                    id "Description"
                    name "Description"
                    class' "border-2 rounded-sm grow px-4 py-2"

                    value upsertDocument.Description
                }
            }

            div {
                class' "flex mb-4"

                label {
                    class' "w-40 self-center"
                    for' "Images"
                    "Upload Images"
                }

                input {
                    type' "file"
                    id "Images"
                    name "Images"
                    multiple true
                    class' "border-2 rounded-sm grow px-4 py-2"
                }
            }

            div {
                class' "flex mb-4"

                label {
                    class' "w-40 self-center"
                    for' "Source"
                    "Source"
                }

                textarea {
                    rows 20
                    id "Source"
                    name "Source"
                    class' "border-2 rounded-sm grow px-4 py-2"

                    upsertDocument.Source
                }
            }

            div {
                class' "flex mb-4"

                div {
                    class' "flex w-40 self-center"

                    label {
                        class' "mr-4"
                        "Tags"
                    }

                    button {
                        class' "border-2 rounded-sm grow px-2 bg-gray"
                        "+"
                    }
                }

                ul {
                    childContent (
                        upsertDocument.Tags
                        |> List.map (fun t ->
                            li {
                                div {
                                    input {
                                        type' "text"
                                        name "Tags"
                                        value t
                                    }

                                    input {
                                        type' "button"
                                        value "-"
                                    }
                                }
                            })
                    )
                }
            }

            div {
                class' "flex justify-between"

                input {
                    class' "border-2 rounded-sm grow px-2 bg-gray"
                    type' "Submit"
                }

                input {
                    type' "button"
                    class' "border-2 rounded-sm grow px-2 bg-gray"
                    value "Delete"
                }
            }
        ]
    }
    // script [
    //     _async
    //     _defer
    //     _src "/js/upsert.js"
    //     _type "text/javascript"
    // ] []
    |> Card.simple "col-span-2"

let private availableImages (images: string list) =
    div {
        childContent (
            images
            |> List.map (fun image ->
                div {
                    class' "mb-2"
                    label { image }
                    img { src $"/images/%s{image}" }
                })
        )
    }
    |> Card.simple ""

let private html (upsertDocument: UpsertDocument) (images: string list) =
    div {
        class' "grid grid-cols-3 gap-6"
        upsertForm upsertDocument
        availableImages images
    }

let view (upsertDocument: UpsertDocument) (images: string list) =
    Layout.Create(h1 { "Upsert" }, html upsertDocument images)
