[<RequireQualifiedAccess>]
module App.Views.Pages.Article

open Microsoft.AspNetCore.Http
open Fun.Blazor
open App.Views
open Model
open App.Views.Components

let private openGraph (article: ParsedDocument) =
    html.inject (fun (accessor: IHttpContextAccessor) ->
        let host = accessor.HttpContext.Request.Host.Value

        let permaLink =
            $"https://%s{host}/article/%s{App.Utils.getUrl article.Id article.Title}"

        html.fragment [
            meta {
                "property", "og:site_name"
                content "Hashset - Kai Ito"
            }
            meta {
                "property", "og:title"
                content article.Title
            }
            meta {
                "property", "og:type"
                content "article"
            }
            meta {
                "property", "og:description"
                content article.Description
            }
            meta {
                "property", "og:url"
                content permaLink
            }
            meta {
                name "description"
                content article.Description
            }
            link {
                rel "canonical"
                href permaLink
            }
        ])

let private article (article: ParsedDocument) =
    html.fragment [
        div {
            class' "prose prose-sm md:prose-base lg-prose-xl max-w-none"

            html.raw article.Document
        }
    ]

let private articleHtml (parsedDocument: ParsedDocument) =
    html.fragment [
        openGraph parsedDocument
        html.inject (fun (accessor: IHttpContextAccessor) ->
            if accessor.HttpContext.User.Identity.IsAuthenticated then
                a {
                    href $"/articles/upsert?id={parsedDocument.Id}"
                    "Edit"
                }
            else
                html.none)
        (Card.simple "" (article parsedDocument))
    ]

let private title (article: ParsedDocument) =
    div {
        h1 {
            class' "lg:text-6xl text-2xl mb-2"
            article.ArticleDate.ToString("dd MMM yyyy")
        }

        h2 {
            class' "text-base font-normal"

            TagList.simple article.Tags
        }
    }

let view (article: ParsedDocument) =
    Layout.Create(title article, articleHtml article)
