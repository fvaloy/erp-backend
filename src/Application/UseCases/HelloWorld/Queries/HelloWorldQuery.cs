using MediatR;

namespace Application.UseCases.HelloWorld.Queries;

public record HelloWorldQuery : IRequest<string>;

public class HelloWorldQueryHandler : IRequestHandler<HelloWorldQuery, string>
{
    public Task<string> Handle(HelloWorldQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult("Hello World!");
    }
}