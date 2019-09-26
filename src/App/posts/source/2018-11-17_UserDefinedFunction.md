User Defined Function (UDF) in SQLite and casting an F# function to a Func object
==================

I ran into an interesting problem recently. The task was to search for something within a SQLite database by comparing strings. Generally very easy to do in just SQL using the UPPER function:

    [lang=sql]
    SELECT * FROM User WHERE UPPER(Name) = @input

and `@input` comes from your user input as a prepared value. So in your code you would also uppercase the user input. Now the keen reader that is familiar with SQLite specifically will say to use `COLLATE NOCASE`. This is also a possibility, but I didn’t know about it when I first wrote my code ¯\\_(ツ)_/¯. Now both of these work fine, except for an important factor. The SQLite UPPER function only upper-cases ASCII characters. This is a problem when you need to handle letters like ü, Ü, å, Å. Those won’t get upper-cased, and it also won’t work when using `COLLATE NOCASE`. Meaning the comparison will make a case insensitive match with only ASCII characters, but when there’s a non-ASCII character that is a different case, it won’t match.

    [hide]
    open System

The solution that I found was to override the built in `UPPER` function. This is done with User Defined Functions, where you can write your own function or override an existing function and then have SQLite utilize that function. The way this looks is implementation specific, and on .Net Core utilizing the `Microsoft.Data.Sqlite` it looks like this:

    connection.Open()

    connection.CreateFunction<string, string>("UPPER",
        Func<string, string>(fun (args: string) ->
        args.ToUpper(CultureInfo.InvariantCulture)))

    let results =
        connection.Query<PostalCodeInformation>(sql, dict ["Input", box input])connection.Close()


Of note is that the custom function will only exist as long as the connection is open. The first argument is the name of the created function, in this case we are redefining the `UPPER` function. Now when we use the `UPPER` function in the SQL statement, it will delegate to our custom function instead of using the built in function.

The other problem was that since this library is written in C#, using it from F# provided a little problem. It wasn’t enough to simply pass in an anonymous function to the second parameter, as those compile to `FSharpFunc` in the IL, whereas the library call was expecting a `Func` object. Luckily we can directly case our anonymous function to a `Func` object with regular casting.

Perhaps there’s another way to solve this problem, but this was the solution I found, and I think works rather nicely. However I imagine that performance is not the greatest as now the SQL engine has to call an external reference.
