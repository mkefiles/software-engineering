# Project Overview

## Helpful Tips

- When starting a project in IDE, make sure to update *.csproj* to disable implicit usings:
  - `<ImplicitUsings>disable</ImplicitUsings>`
- The *Postman-Collection.json* contains the necessary *Collection* to import into Postman for testing

## Setup

### Initial Steps
The following are the initial steps taken for building the project in VS Code (the editor used in the Microsoft ASP.NET tutorials):

1. Install *C# Dev Kit* and *REST Client* extensions
2. In Command Palette, select ".NET: New Project"
3. For the project, select Web API (**not** the AOT option)
4. Add SwashBuckle for ASP.NET Core: `dotnet add package Swashbuckle.AspNetCore --version 10.0.1`
5. Add ASP.NET EFC DB Provider for SQLite: `dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 10.0.0`
6. Install EFC Tools: `dotnet tool install --global dotnet-ef`
7. Add logic for EFC to create DB: `dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.0`
8. Add necessary code
9. Generate the "Migration": `dotnet ef migrations add InitialCreate`
10. Create DB / Schema (based on Migration): `dotnet ef database update`

**Additional Callout:**
I ended up moving the entire *Project* to a new parent-directory both to further experiment with *Solution Files* **and** to match the directory layout in the TSG Master Repo. This entailed running `dotnet new sln` (from the parent-directory) and, then, `dotnet sln add SocialMediaBlog/SocialMediaBlog.csproj` so the *Solution File* would contain/point to the proper *Project*.

### Recurring Steps

- In the event that **any** changes are made to an "Entity", you should:
  - Create a new migration: `dotnet ef migrations add <MigrationName>`
  - Update the Database / Schema: `dotnet ef database update`

## General
This is a simple Social Media Blog API (backend only) that uses C#, ASP.NET Core, SwashBuckle (for SwaggerUI) and SQLite. This app uses the *Controller-based API* approach in lieu of the *Minimal API* approach.

## Project
### Database / Entity Structure

```
accountId integer primary key auto_increment,
username varchar(255) not null unique,
password varchar(255)
```

```
messageId integer primary key auto_increment,
postedBy integer,
messageText varchar(255),
timePostedEpoch long,
foreign key (postedBy) references Account(accountId)
```

### User Stories

#### New User Registrations
The *user* should be able to create a new Account using:

- End-point: `POST` - `localhost:5099/register`

... and passing a JSON payload that does **not** contain an Account ID:

```JSON
{
  "Username": "mlfiles",
  "Password": "mike3603"
}
```

If successful:

- Criteria:
  - The username is not blank
  - The password is, at least, four characters long
  - The username does not already exist
- Response:
  - HTTP Status Code: `200` (Ok)
  - Body: Contain JSON repr. of Account (incl. Account ID)
  - Persisted to database

If failed:

- Reason / Response:
  - Duplicate username - `409` (Conflict)
  - Misc. - `400` (Client Error)

#### Existing User Login
The *user* should be able to verify their login.

- End-point: `POST` - `localhost:5099/login`

... and passing a JSON payload:

```JSON
{
    "Username": "mlfiles",
    "Password": "mike0131"
}
```

If successful:

- Criteria:
  - The username and password provided match a real account existing on the database
- Response:
  - HTTP Status Code: `200` (Ok)
  - Body: Contain JSON repr. of Account (incl. Account ID)

If failed:

- Reason / Response:
  - Misc. - `401` (Unauthorized)

#### Create New Message
The *user* should be able to process the creation of new message(s).

- End-point: `POST` - `localhost:5099/messages`

... and passing a JSON payload that does **not** contain a Message ID:

```JSON
{
    "PostedBy": 2,
    "MessageText": "I like long walks on the beach!"
}
```

If successful:

- Criteria:
  - The messageText is not blank
  - The messageText is not over 255 characters
  - The postedBy refers to a real/existing user
- Response:
  - HTTP Status Code: `200` (Ok)
  - Body: Contain JSON repr. of Message (incl. Message ID)
  - Persisted to database

If failed:

- Reason / Response:
  - Misc. - `400` (Client Error)

#### Retrieve All Messages
The *user* should be able to request all message(s).

- End-point: `GET` - `localhost:5099/messages`

If successful:

- Response:
  - HTTP Status Code: `200` (Ok)
  - Body: Contain JSON repr. of Messages (incl. Message ID)

If failed:

- Response:
  - HTTP Status Code: `200` (Ok)
  - Body: Empty List

#### Retrieve Message By ID
The *user* should be able to request a specific message by its ID.

- End-point: `GET` - `localhost:5099/messages/{messageId}`

If successful:

- Response:
  - HTTP Status Code: `200` (Ok)
  - Body: Contain JSON repr. of Message (incl. Message ID)

If failed:

- Response:
  - HTTP Status Code: `200` (Ok)
  - Body: Empty

#### Delete Message By ID
The *user* should be able to delete a specific message by its ID.

- End-point: `DELETE` - `localhost:5099/messages/{messageId}`

If successful:

- Response:
  - HTTP Status Code: `200` (Ok)
  - Body: Number of Rows Updated (e.g., `1`)
  - Persist deletion to database

If failed:

- Response:
  - HTTP Status Code: `200` (Ok)
  - Body: Empty

**Note:**
- The `DELETE` verb is intended to be *idempotent* (i.e., multiple calls to the `DELETE` endpoint should respond with the same type of response), which is why an unsuccessful delete responds with empty

#### Update Message Text By ID
The *user* should be able to update a specific message by its ID.

- End-point: `PATCH` - `localhost:5099/messages/{messageId}`

... and passing a JSON payload that contains a new messageText value (it cannot be guaranteed to contain any other information):

```JSON
{
    "MessageText": "My wife is gr8!! :)"
}
```

If successful:

- Criteria:
  - The message id already exists
  - The new messageText is not blank
  - The new messageText is not over 255 characters
- Response:
  - HTTP Status Code: `200` (Ok)
  - Body: Number of Rows Updated (e.g., `1`)
  - Persist update to database

If failed:

- Response:
  - HTTP Status Code: `400` (Client Error)
  - Body: Empty

#### Retrieve All Messages By User
The *user* should be able to request all message(s) by a specific User.

- End-point: `GET` - `localhost:5099/accounts/{accountId}/messages`

If successful:

- Response:
  - HTTP Status Code: `200` (Ok)
  - Body: Contain JSON repr. of Messages (incl. Message ID)

If failed:

- Response:
  - HTTP Status Code: `200` (Ok)
  - Body: Empty List