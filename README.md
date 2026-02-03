# webapi-backend

Backend for iRAS-RAG

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Docker](https://www.docker.com/get-started) (required for integration tests with Testcontainers)
- [Git](https://git-scm.com/downloads)

### Installation Steps

1. **Clone the repository**
   ```sh
   git clone https://github.com/iRAS-RAG/webapi-backend.git
   cd webapi-backend
   ```

2. **Restore .NET dependencies**
   ```sh
   dotnet restore
   ```

3. **Restore .NET tools (including Husky.Net)**
   ```sh
   dotnet tool restore
   ```

4. **Install Husky.Net Git hooks**
   ```sh
   dotnet husky install
   ```

---

## Configuration

### Using ASP.NET Core User Secrets

To keep sensitive information (like connection strings, API keys, JWT secrets) out of source control, use ASP.NET Core User Secrets for local development.

#### How to Set Up User Secrets

1. **Initialize User Secrets for the RKZ.API project:**
   Run this command in the project directory (where the `.csproj` file is):
   ```sh
   dotnet user-secrets init
   ```

2. **Add secrets using the CLI:**
   For example:
   ```sh
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
   dotnet user-secrets set "Jwt:Secret" "your-jwt-secret"
   dotnet user-secrets set "ApiKeys:SomeService" "your-api-key"
   ```

3. **Access secrets in code as usual:**
   Use the standard configuration API, e.g.:
   ```csharp
   var connStr = Configuration["ConnectionStrings:DefaultConnection"];
   var jwtSecret = Configuration["Jwt:Secret"];
   ```

#### Notes
- User Secrets are only for development. Use environment variables or a secrets manager for production.
- Do not commit secrets to any `appsettings*.json` file.
- For the values themselves, refer to the [docs](https://github.com/iRAS-RAG/docs) repo

### Database Setup

- Update your connection string using User Secrets as described above.
- Apply migrations:
   ```sh
   dotnet ef database update --project IRasRag.Infrastructure --startup-project IRasRag.API
   ```

---

## Running the Application

```sh
dotnet run --project IRasRag.API
```

- The API will be available at `http://localhost:5027/api`.
- Use tools like Postman or [Swagger UI](http://localhost:5027/swagger/index.html) to interact with the endpoints.

---

## Testing

Integration tests use [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet) to spin up a real PostgreSQL database in Docker.
**Docker must be installed and running** for integration tests to work.

To run all tests:
```sh
dotnet test
```

---

## Customizing Git Hooks

You can configure hooks in the `.husky` directory.  
Example: To run tests before each commit, add the following to `.husky/pre-commit`:
```sh
dotnet test
```

---

## License

This project is licensed under the AGPL 3.0 License.

---
