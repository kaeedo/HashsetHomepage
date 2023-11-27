namespace Model

open FSlugify.SlugGenerator

module Utils =
    let getUrl id title =
        let title = slugify DefaultSlugGeneratorOptions title

        sprintf "%i_%s" id title
