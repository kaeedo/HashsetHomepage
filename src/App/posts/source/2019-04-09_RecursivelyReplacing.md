Recursively replacing parts of a URL
======================

I’m slowly building a .Net Core global tool to do some REST API testing. One idea I had for this project was to be able to generate multiple URLs with different variables. e.g. `http://www.example.com/?id={{INT}}` would generate several URLs with the `{{INT}}` part replaced with randomly generated integers. The solution that I came up with for this, was to use regular expressions. `{{(.*?)}}` is the regex I came up with to find all parts enclosed by double brackets.

Now came the task of actually replacing the brackets. As I have a strong C# background, an approach I like to use is to code what I want to accomplish in an OOP or imperative style, and then to refactor it to a functional style.

Here my imperative approach:

```
let mutable match = variableRegex.Match(url)
let mutable currentIndex = 0
while match.Success do
    let variableToReplace = match.Groups.[0]
    let length = variableToReplace.Index - currentIndex
    let subUrl = url.Substring(currentIndex, length)
    builder.Append(subUrl) |> ignore
    let replaceWith =
        match match.Groups.[1].Value with
        | "INT" -> "INTEGER" // replace with int generator
        | _ -> "OBJECT" // optionally other data types
    builder.Append(replaceWith) |> ignore
    currentIndex <- subUrl.Length + variableToReplace.Length
    match <- match.NextMatch()
builder.ToString()
```

And here after refactoring to a functional recursive Approach:

```
let rec build currentIndex (currentMatch: Match) =
    if currentMatch.Success then
        let variableToReplace = currentMatch.Groups.[0]
        let length = variableToReplace.Index - currentIndex
        let subUrl = url.Substring(currentIndex, length)
        builder.Append(subUrl) |> ignore
        let replaceWith =
            match currentMatch.Groups.[1].Value with
            | "INT" -> "INTEGER" // replace with int generator
            | _ -> "OBJECT" // optionally other data types
        builder.Append(replaceWith) |> ignore
        build (subUrl.Length + variableToReplace.Length) (currentMatch.NextMatch())
    else
        builder.ToString()
build 0 (variableRegex.Match(url))
```

This approach has really helped me in not ripping my hair out while I’m trying to simultaneously figure out what it is I need to do, while also figuring out how to properly recurse over whatever it is I’m doing.

Something specific to take note of, is that the imperative approach has two mutable variables. Coincidentally, the recursive version has two values passed in as parameters. Or maybe this isn’t a coincidence, and is a very good starting point as to how to go about the refactor.

The keen reader would also point out that the recursive version isn’t actually functional. There are side effects. It is appending a string to a StringBuilder. This is a place where I opted for (premature) optimization, as appending strings is expensive. The purely functional approach would simply also pass in the string to build as a parameter.
