module App.FunViews.Header

open Fun.Blazor
open Model

let navItem (label: string) =
    li {
        class' "after:content-[''] after:block after:w-12 after:mx-auto after:border-b-4"
        a { label }
    }

let view () =
    header {
        class' "bg-green border-b-4 border-black h-64 flex items-center"

        div {
            class' "container mx-auto flex justify-between"

            div {
                class' "border-4 w-max py-6 px-12 bg-orange drop-shadow-[8px_8px_0px_#000000]"

                h1 {
                    class' "text-3xl font-bold"
                    "Hashset"
                }

                h2 {
                    class' "text-2xl"
                    "⤷ Kai Ito"
                }
            }

            nav {
                class' "flex items-center border-4 w-max py-6 px-8 bg-orange drop-shadow-[8px_8px_0px_#000000]"

                ul {
                    class' "flex justify-between text-2xl gap-8"

                    childContent [
                        navItem "Home"
                        navItem "Articles"
                        navItem "About"
                    ]
                }
            }
        }
    }
