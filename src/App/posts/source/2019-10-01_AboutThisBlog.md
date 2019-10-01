About This Blog
======================

This post will serve as a what, how, and a why.

---

### What is this blog about

This is a personal blog where I will write about all my various programming adventures. The majority of the posts will most likely be about F#, as that is my language of choice, and I want to promote it and functional programming in general. However, that's not to say that it will be exclusively about F#, and will also contain other posts detailing my learnings of other languages, frameworks, and tools. I will probably also write about home automation, and any discoveries I make in the process of self hosting various services. At the time of writing, I don't have a tagging system or a Series feature implemented yet, but those are things I plan to do. I will be using those features to allow easy organization and filtering on the various types of posts I plan to write.

### How was this website built

As already mentioned, I'm a big fan of F#. Naturally, this site is built using F#. It uses [Giraffe](https://github.com/giraffe-fsharp/Giraffe) as the web server, and as the view engine. There is no frontend framework in play. Posts are written in markdown, and parsed using [FSharp.Literate](http://fsprojects.github.io/FSharp.Formatting/literate.html). It's hosted within a docker container and hosted in a VPS. The reverse proxy and SSL configuration is managed by [Traefik](https://traefik.io/). Ultimately, Cloudflare is acting as the HTTP Proxy to cache the various assets closer to the end client, as well as providing a few additional security measures.


### Why did I built this site
