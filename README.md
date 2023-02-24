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
- Unit tests using Moq: https://www.youtube.com/watch?v=9ZvDBSQa_so 
- Azure Service Bus: https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview
- NSubstitute: https://nsubstitute.github.io/
- AutoFixture: https://github.com/AutoFixture/AutoFixture
- FluentAssertions: https://fluentassertions.com/

Additional/optional material:
- Versioning APIs and Handlers: https://www.youtube.com/watch?v=WFEE5yVJwGU
- BenchmarkDotNet: https://www.youtube.com/watch?v=EWmufbVF2A4 

# Features
It is a fully functional service. It has the following features:

- Uses latest .Net version .Net 7 
- Based on the Onion Architecture structure
- Uses Docker and Docker Compose
- Uses Masstransit
- Uses SQL Server Cache (replacing Azure redis Cache for on-premise version)
- Uses EF core
- Uses Serilog
- Uses Swagger
- Uses Automapper for mapping DTOs
- Uses BenchmarkDotNet for performance testing (removed)
- Integration Test for event subscribing for ActiveMQ
- Unit Tests using MS Test with NSubstitute, AutoFixture and fluentAssertions

# Missing Features

- JWT Authentication
