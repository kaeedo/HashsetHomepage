namespace Hashset

open System.Threading.Tasks
open Microsoft.AspNetCore.Authentication
open System.Net.Http.Headers
open System.Text
open System.Security.Claims
open System
open Microsoft.Extensions.Configuration

type IUserService =
    abstract member AuthenticateAsync: string -> string -> Task<bool>

type UserService(configuration: IConfiguration) =
    let username = configuration.["AuthorUsername"]
    let password = configuration.["AuthorPassword"]
    let users = [ (username, password) ] |> Map.ofList

    interface IUserService with
        member _.AuthenticateAsync username password =
            task {
                let result =
                    match users.TryGetValue username with
                    | true, user when user = password -> true
                    | _ -> false

                return result
            }

type BasicAuthHandler(options, logger, encoder, clock, userService: IUserService) =
    inherit AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)

    override this.HandleAuthenticateAsync() =
        let request = this.Request
        let response = this.Response

        match request.Headers.TryGetValue "Authorization" with
        | (true, headerValue) ->
            task {
                let headerValue = AuthenticationHeaderValue.Parse(headerValue.[0])

                let bytes = Convert.FromBase64String headerValue.Parameter

                let credentials = (Encoding.UTF8.GetString bytes).Split(":")

                let! userFound = userService.AuthenticateAsync credentials.[0] credentials.[1]

                let authenticationResult =
                    match userFound with
                    | false -> AuthenticateResult.Fail("Invalid username or password")
                    | true ->
                        let claims = [|
                            Claim(ClaimTypes.NameIdentifier, credentials.[0])
                            Claim(ClaimTypes.Name, credentials.[0])
                        |]

                        let identity = ClaimsIdentity(claims, this.Scheme.Name)
                        let principal = ClaimsPrincipal identity

                        let ticket = AuthenticationTicket(principal, this.Scheme.Name)

                        AuthenticateResult.Success ticket

                return authenticationResult
            }
        | (false, _) ->
            task {
                response.Headers.WWWAuthenticate <- "Basic"
                return AuthenticateResult.Fail("Missing Authorization Header")
            }
