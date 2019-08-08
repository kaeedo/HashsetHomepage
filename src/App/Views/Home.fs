namespace Hashset.Views

module Home =
    open Giraffe.GiraffeViewEngine

    let private pageTitle = "hashset.dev"

    let view =
        p [] [ str "This is the homepage" ]
