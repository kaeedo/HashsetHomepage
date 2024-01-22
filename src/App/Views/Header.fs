module App.Views.Header

open Fun.Blazor
open Microsoft.AspNetCore.Http

let navItem (label: string) (link: string) =
    li {
        class' "after:block after:w-12 after:mx-auto after:border-b-4"

        a {
            href link
            label
        }
    }

let view (pageTitle: NodeRenderFragment) =
    header {
        class' "bg-green border-b-4 border-black h-64 flex items-center"

        div {
            class'
                "container mx-auto flex lg:justify-between lg:flex-row flex-col-reverse max-lg:gap-4 max-lg:items-center top-4 absolute md:relative"

            div {
                class' "hidden lg:block border-4 w-max py-6 px-12 bg-orange drop-shadow-[8px_8px_0px_#000000]"

                h1 {
                    class' "text-3xl font-bold"
                    "Hashset"
                }

                h2 {
                    class' "text-2xl"
                    "⤷ Kai Ito"
                }
            }

            div {
                class'
                    "flex items-end font-mono lg:text-6xl text-4xl font-black w-max lg:drop-shadow-[2px_2px_0px_#ff5dfd]"

                pageTitle
            }

            nav {
                class' "flex items-center border-4 w-max py-6 px-8 bg-orange drop-shadow-[8px_8px_0px_#000000]"

                ul {
                    class' "flex justify-between text-2xl gap-8"

                    childContent [
                        navItem "Home" "/"
                        navItem "Articles" "/articles"
                        html.inject (fun (accessor: IHttpContextAccessor) ->
                            if accessor.HttpContext.User.Identity.IsAuthenticated then
                                navItem "New" "/articles/upsert"
                            else
                                html.none)
                        navItem "About" "/about"
                    ]
                }
            }
        }
    }
