namespace App

open FSlugify.SlugGenerator


module Utils =
    let getUrl id title =
        let title = slugify DefaultSlugGeneratorOptions title

        $"%i{id}_%s{title}"
