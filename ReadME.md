# Description
Bezkie is a library project
- customers can signup and login
- customers can search for books
- reserve a book

# Technologies used
- C# .NET 6
- MediatR
- MySQL
- Docker
- ORM - Entity Framework


# Implementation
`signup` endpoint allows customers to create a profile on the system <br>

`login` endpoint to retrieve jwt bearer token to be used in the `rsvp` endpoint <br>

The `rsvp` endpoint allows authenticated users to reserve a book.
if a book is reserved, another customer cannot reserve it. 24hrs after reservation if book status is not changed to borrowed book becomes available again.

the `search` endpoint allows all users to search for book availability.

# How to run
- [ ] configure connection strings in `appsettings.development.json` in API project to correspond to yours

- navigate to `bezkie.API` run `dotnet run`
- browse `http://localhost:3000/swagger/index.html` to access the swagger page on the API

## using docker
- [ ] Ensure `docker` and `docker-compose` are installed
- replace `<password>` with your desired value

the values used in this string is same with what we provisioned on the docker compose

- run `docker-compose up`
- browse `http://localhost:3000/swagger/index.html` to access the swagger page on the API

# Testing
To run the unit tests
  - open terminal at root of solution, then run `dotnet test`

# Challenges
- n/a
