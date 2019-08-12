Power of Active Patterns
========================

I’ve been working on a [dotnet core global tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools). One of the things that such a tool has, is many command line options. I found Argu to parse these command line options. However I ran into the problem that I was left with a very large amount of if/else statements. Perhaps I don’t know what the “proper” way is of handling this, but this was my naive approach.


```
let hasInfo = arguments.Contains Info
let hasPostalCode = arguments.Contains PostalCode
let hasUpdate = arguments.Contains Update
let hasList = arguments.Contains List
let hasClearDatabase = arguments.Contains ClearDatabase

if hasInfo
then printfn "info"
elif hasPostalCode && hasUpdate
then printfn "%s" <| parser.PrintUsage()
elif hasPostalCode && not hasUpdate
then getPostalCodeInformation <| arguments.GetResult PostalCode
elif hasList
then
    match arguments.GetResult List with
    | None -> printfn "%s" <| parser.PrintUsage()
    | Some list ->
        match list with
        | Supported -> ConsolePrinter.printCountries DataDownload.supportedCountries
        | Available ->
            Database.ensureDatabase()
            match Query.getAvailableCountries () with
            | Ok countries ->
                match countries with
               | None -> printfn "No countries have been updated yet"
               | Some c -> ConsolePrinter.printCountries c
            | Error e ->
                ConsolePrinter.printErrorMessage e ErrorLog.writeException
elif hasClearDatabase
then
    Database.clearDatabase()
elif not hasPostalCode && hasUpdate
then
    match arguments.GetResult Update with
    | None -> printfn "%s" <| parser.PrintUsage()
    | Some countryCode ->
        updateCountry countryCode
```

The entire time I was thinking that there must be a better way. After mulling over it awhile, I suddenly remembered Active Patterns. What’s interesting to note, is that I had never used Active Patterns before, but I still somehow knew that this was finally a perfect use case for these. This allowed me to decouple the command line option, from its functionality into easily maintained blocks.


```
let (|ShowInformation|HasPostalCode|UpdateCountry|ListAvailable|ListSupported|HasClearDatabase|HelpRequested|) (input: ParseResults<Arguments>) =
    if input.Contains Info then ShowInformation
    elif input.Contains PostalCode then HasPostalCode
    elif input.Contains Update then UpdateCountry
    elif input.Contains List
    then
        let listRequest = input.GetResult List
        let isAvailableRequest = listRequest |> Option.exists (fun l -> l = Available)
        let isSupportedRequest = listRequest |> Option.exists (fun l -> l = Supported)
    if not isAvailableRequest && not isSupportedRequest
    then HelpRequested
    elif isAvailableRequest
    then ListAvailable
    else ListSupported
    elif input.Contains ClearDatabase then HasClearDatabase
    else HelpRequested
```

```
match arguments with
| ShowInformation -> printfn "info"
| HasPostalCode -> getPostalCodeInformation <| arguments.GetResult PostalCode
| UpdateCountry ->
    match arguments.GetResult Update with
    | None -> printfn "%s" <| parser.PrintUsage()
    | Some countryCode ->
        updateCountry countryCode
| ListAvailable ->
    Database.ensureDatabase()
    match Query.getAvailableCountries () with
    | Ok countries ->
        match countries with
        | None -> printfn "No countries have been updated yet"
        | Some c -> ConsolePrinter.printCountries c
    | Error e ->
        ConsolePrinter.printErrorMessage e ErrorLog.writeException
| ListSupported -> ConsolePrinter.printCountries DataDownload.supportedCountries
| HasClearDatabase -> Database.clearDatabase()
| _ -> printfn "%s" <| parser.PrintUsage()
```


Much better.
