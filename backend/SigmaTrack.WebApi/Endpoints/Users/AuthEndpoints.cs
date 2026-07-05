using SigmaTrack.Application.Features.Auth.Login;
using SigmaTrack.Application.Features.Auth.Register;

namespace SigmaTrack.WebApi.Endpoints.Users
{
    public class AuthEndpoints : IEndpointModule
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/auth").WithTags("Authentication");

            group.MapPost("register", RegisterAsync)
                .WithName("Register")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status500InternalServerError);

            group.MapPost("login", LoginAsync)
                .WithName("Login")
                .Produces<LoginResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status500InternalServerError);
        }

        private static async Task<IResult> RegisterAsync(
            RegisterRequest registerRequest,
            IRegisterUseCase registerUseCase,
            CancellationToken cancellationToken)
        {
            await registerUseCase.ExecuteAsync(registerRequest, cancellationToken);
            return Results.Ok(new { message = "Registration successful" });
        }

        private static async Task<IResult> LoginAsync(
            LoginRequest loginRequest,
            ILoginUseCase loginUseCase,
            CancellationToken cancellationToken)
        {
            var response = await loginUseCase.ExecuteAsync(loginRequest, cancellationToken);
            return Results.Ok(response);
        }
    }
}