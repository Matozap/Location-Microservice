# Location Microservice

# Prerequisites:
- Update Rider or Visual studio to the latest version.
- Download .Net 7 SDK: https://dotnet.microsoft.com/download/dotnet

# Learning Material 
- Mediatr and CQRS: https://www.youtube.com/watch?v=YzOBrVlthMk
- Onion Architecture: https://www.codewithmukesh.com/blog/onion-architecture-in-aspnet-core/ 
- Masstransit: https://masstransit.io/quick-starts
- Dependency injection in .Net: https://auth0.com/blog/dependency-injection-in-dotnet-core/
- Serilog: https://www.codewithmukesh.com/blog/serilog-in-aspnet-core-3-1/
- Unit tests using NSubstitute: https://nsubstitute.github.io/ 
- Azure Service Bus: https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview
- NSubstitute: https://nsubstitute.github.io/
- AutoFixture: https://github.com/AutoFixture/AutoFixture
- FluentAssertions: https://fluentassertions.com/

Additional/optional material:
- Versioning APIs and Handlers: https://www.youtube.com/watch?v=WFEE5yVJwGU
- Azure Functions: https://azure.microsoft.com/en-us/products/functions/

# Features
It is a fully functional service. It has the following features:

- Uses latest .Net version .Net 7 
- Based on the Onion Architecture structure
- Uses Docker and Docker Compose
- Event bus allows Azure Service Bus, RabbitMQ or InMemory out of the box just changing the settings
- Cache (IDistributedCache) allows Redis, Sql Server or InMemory out of the box with just changing the settings
- Uses EF core
- Uses Serilog
- Uses Swagger
- Uses Mapster for mapping DTOs
- Uses BenchmarkDotNet for performance testing (removed)
- Unit Tests using XUnit with NSubstitute, AutoFixture and fluentAssertions

# Not included Features

- JWT Authentication
