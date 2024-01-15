[<RequireQualifiedAccess>]
module App.Views.Pages.Upsert

open App.Views.Components
open App.Views.Partials
open Fun.Blazor
open Fun.Htmx
open App.Views
open Microsoft.AspNetCore.Http
open Model

let private tagList (upsertDocument: UpsertDocument) =
    div {
        class' "flex mb-4"

        div {
            class' "flex w-40 self-center"

            label {
                class' "mr-4"
                "Tags"
            }

            button {
                class' "border-2 rounded-sm grow px-2 bg-gray mx-5"
                type' "button"
                hxGet "/partials/taginput"
                hxTarget "#tagList"
                hxSwap_beforeend
                "+"
            }
        }

        ul {
            id "tagList"

            childContent (
                upsertDocument.Tags
                |> List.map (fun t -> TagInput.simple t)
            )
        }
    }

let private upsertForm (upsertDocument: UpsertDocument) =
    html.inject (fun (accessor: IHttpContextAccessor) ->
        let articleId =
            accessor.HttpContext.Request.Query["id"]
            |> Seq.tryHead

        form {
            action "/upsert"
            method "POST"

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
                                hxGet "/articles/upsert"
                                hxTarget "body"
                                hxPushUrl true

                                "New"
                            }

                            yield!
                                (upsertDocument.ExistingIds
                                 |> Seq.sortByDescending fst
                                 |> Seq.map (fun (articleId, title) ->
                                     option {
                                         value (articleId.ToString())
                                         selected (title = upsertDocument.Title)
                                         hxGet $"/articles/upsert?id={articleId}"
                                         hxTarget "body"
                                         hxPushUrl true

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

                tagList upsertDocument

                div {
                    class' "flex justify-between"

                    input {
                        class' "border-2 rounded-sm grow px-2 bg-gray"
                        type' "Submit"
                    }

                    match articleId with
                    | None -> html.none
                    | Some ai ->
                        input {
                            type' "button"
                            class' "border-2 rounded-sm grow px-2 bg-gray"
                            value "Delete"

                            hxDelete $"/article/{ai}"
                            hxConfirm "Are you sure you wish to delete this article"
                        }
                }
            ]
        }
        |> Card.simple "col-span-2")

let private availableImages (images: string list) =
    div {
        childContent (
            images
            |> List.map (fun image ->
                div {
                    class' "mb-2"
                    label { image }

                    img { src image }

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
