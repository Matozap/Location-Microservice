# Location Microservice

![Build](https://img.shields.io/github/actions/workflow/status/Matozap/Location-Microservice/main-build.yml?style=for-the-badge&logo=github&color=0D7EBF)
![Build](https://img.shields.io/github/issues/Matozap/Location-Microservice?style=for-the-badge&logo=github&color=0D7EBF)
![Build](https://img.shields.io/github/last-commit/Matozap/Location-Microservice?style=for-the-badge&logo=github&color=0D7EBF)




A microservice that is responsible for handling requests for location information, such as country, state and city, 
and maintaining a database of location information which should not be updated by external systems.

Exposes an API in 2 flavors: controllers and Azure functions and it allows other services or applications to query the location information. 
The API starts with the basic types of queries, such as:
- GetAll
- Get
- Create
- Update
- Delete
- Disable

It can be integrated with other services via message brokers, such as RabbitMQ or Azure Service Bus, to provide eventual consistency about a specific location.

Overall, this microservice provides a scalable and flexible solution for handling location information in a distributed system, allowing other services or applications to easily and reliably retrieve location information as needed.

----------------------------------


### Architecture

- Uses Onion Architecture project structure

### Design Patterns

- CQRS
- Mediator
- Dependency Injection
- Options
- Repository
- Outbox
- Pub-Sub

### Development

- Uses latest .Net version .Net 7 
- Uses Docker and Docker Compose to load all external components required
- Uses controllers (Web API)
- Uses Azure Functions (dotnet-isolated)
- Message broker with Azure Service Bus, RabbitMQ or InMemory out of the box just changing the settings
- Distributed Cache with Redis, Sql Server or InMemory out of the box with just changing the settings
- Uses EF core
- Uses Serilog
- Uses Swagger
- Uses Mapster for mapping DTOs
- Uses BenchmarkDotNet for performance testing (removed)
- Unit Tests using XUnit with NSubstitute, AutoFixture and fluentAssertions

#### Not included Features

- JWT Authentication

-------------------------------------

### Usage With Docker

`docker compose up -d` 
It will load the external components: 
- Databases (SQL server, MySql and Postgres), 
- Cache (Redis and SQL Server), 
- Message broker (RabbitMQ) 
- The application's Web API.

`docker compose down -v` to stop all containers and remove all their volumes

`docker rmi location.service:1.0` to delete the application image from Docker

### Usage Without Docker

Run or debug the application maintaining the _appsettings.Development.json_ with all options using In-Memory 
or add the required types and connection strings to point to the desired external services.

---------------------------------------

### Learning Material
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

#### Additional material:
- Versioning APIs and Handlers: https://www.youtube.com/watch?v=WFEE5yVJwGU
- Azure Functions: https://azure.microsoft.com/en-us/products/functions/
