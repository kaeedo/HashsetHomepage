Recursive Async
========================

There was recently a project I was working on that required a recursive call. The problem was, that the function returned an async workflow, and in my inexperience, I was running into the problem that I didn’t know what each exit point should return exactly. This is a problem, because in F# all code paths must return the same type, and I wasn’t sure when I needed to wait for an async call, or return the async workflow itself. The idea behind the function was that it would query the database for a specific string, and if it didn’t find anything, it would shorten the string and try again, until the string was a certain length. After some looking around online, I came across [this FSSnip](http://www.fssnip.net/hr/title/Async-function-that-retries-work) that describes a recusive async function:

```
let rec retry work resultOk retries =
    async {
        let! res = work
        if (resultOk res) || (retries = 0) then return res
        else return! retry work resultOk (retries - 1)
    }
```

Here, `work` is the async work that we want to recursively call. We then check if the result was ok using a specific resultOk function, or check if a set number of retries has been reached. If yes, we simply return that result of the async work. Otherwise, we bind the same function, with the same async work to the current async builder and return that.

Once I understood what was going on, I was able to come up with my own solution that worked better for my use case:

```
let getPostalCodeInformation (postalCodeInput: string) =
    async {
        let query (input: string) =
            async {
                let! results = connection.QueryAsync("some SQL script") |> Async.AwaitTask

                return results |> List.ofSeq
            }

        let rec queryUntilMatch (input: string) =
            match input with
            | _ when input.Length <= 3 -> query input
            | _ ->
                let results = (query input) |> Async.RunSynchronously

                if results |> List.isEmpty
                then
                    let newInput = input.Substring(0, int (Math.Ceiling(float input.Length / 2.0)))
                    queryUntilMatch newInput
                else async { return results }

        return! (queryUntilMatch sanitizedInput)
    }
```

So here’s my version, obviously not perfect, and looking at it now, I already want to make some modifications. But you can see on line 12 where it returns the query result once the string is below a certain threshold, and otherwise it returns a recursive call to itself, which is not an async workflow. However, if at any point it finds something before the threshold, it returns the result wrapped in an async workflow, because the query function itself is also one. That’s the key here.
