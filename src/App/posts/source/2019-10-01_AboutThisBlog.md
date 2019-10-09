About This Blog
======================

### What is this blog about

This is a personal blog where I will write about all my various programming adventures. The majority of the posts will most likely be about F#, as that is my language of choice, and I want to promote it and functional programming in general. However, that's not to say that it will be exclusively about F#, and will also contain other posts detailing my learnings of other languages, frameworks, and tools. I will probably also write about home automation, and any discoveries I make in the process of self hosting various services. At the time of writing, I don't have a tagging system or a Series feature implemented yet, but those are things I plan to do. I will be using those features to allow easy organization and filtering on the various types of posts I plan to write.

### How was this website built

As already mentioned, I'm a big fan of F#. Naturally, this site is built using F#. It uses [Giraffe](https://github.com/giraffe-fsharp/Giraffe) as the web server, and as the view engine. There is no frontend framework in play. Posts are written in markdown, and parsed using [FSharp.Literate](http://fsprojects.github.io/FSharp.Formatting/literate.html). It's hosted within a docker container and hosted in a VPS. The reverse proxy and SSL configuration is managed by [Traefik](https://traefik.io/). Ultimately, Cloudflare is acting as the HTTP Proxy to cache the various assets closer to the end client, as well as providing a few additional security measures.


### Why did I built this site

The short version is: Not invented here syndrome. I take a very strong stance against modern web development. I go into some of the reasons in my other blog post about [Fable Preact](https://hashset.dev/posts/2018-12-31_Fable+React+is+nice.+But+what+about+Fable+Preact#My-problem-with-Web-Development). The gist of it is, a large number of websites, static site generators included, deliver a tremendous amount of client assets, most of which are unnecessary.

So the aim of this blog was to build a nice looking website, that delivers an easy to use blog writing experience, all while keeping the client assets to an absolute minimum. As of this writing, a full refresh loads about ~23kb of data, and renders almost immediately, including on slow mobile connections. I even still have some easy wins to reduce this further, as my CSS and JS files aren't even minified. Minimal Viable Product and all that. However, considering the amount of CSS and JS there is, this wouldn't even make much of a difference.

All in all, I'm very happy with the results. In the future, I'm going to be making those aforementioned easy wins, and possibly also setup a certain build process so that it becomes easy for anyone to host this site on their own.
