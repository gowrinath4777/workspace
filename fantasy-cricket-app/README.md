### Prerequisites
1. **Install .NET 6 SDK**: Ensure you have the .NET 6 SDK installed. You can download it from the [.NET official website](https://dotnet.microsoft.com/download/dotnet/6.0).
2. **Install Node.js and npm**: Download and install Node.js from the [Node.js official website](https://nodejs.org/). npm is included with Node.js.
3. **Install Angular CLI**: Install Angular CLI globally using npm:
   ```bash
   npm install -g @angular/cli
   ```

### Step 1: Create the Backend (.NET 6 Web API)

1. **Create a new .NET 6 Web API project**:
   ```bash
   dotnet new webapi -n FantasyCricketApi
   cd FantasyCricketApi
   ```

2. **Add Entity Framework Core**:
   ```bash
   dotnet add package Microsoft.EntityFrameworkCore.InMemory
   dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
   dotnet add package Swashbuckle.AspNetCore
   ```

3. **Create the necessary folders**:
   ```bash
   mkdir Controllers Data Models Services
   ```

4. **Create the `ApplicationDbContext`**:
   Create a file named `ApplicationDbContext.cs` in the `Data` folder:
   ```csharp
   using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
   using Microsoft.EntityFrameworkCore;

   public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
   {
       public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

       public DbSet<Match> Matches { get; set; }
       public DbSet<Player> Players { get; set; }
       public DbSet<MatchPlayer> MatchPlayers { get; set; }
       public DbSet<Contest> Contests { get; set; }
       public DbSet<UserTeam> UserTeams { get; set; }
       public DbSet<UserTeamPlayer> UserTeamPlayers { get; set; }

       protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           base.OnModelCreating(modelBuilder);
           // Configure composite keys and relationships here
       }
   }
   ```

5. **Create Models**:
   Create model classes in the `Models` folder (e.g., `ApplicationUser.cs`, `Match.cs`, `Player.cs`, etc.) based on the domain model outlined in the README.md.

6. **Create Controllers**:
   Create controllers in the `Controllers` folder (e.g., `AuthController.cs`, `MatchesController.cs`, etc.) following the API design specified in the README.md.

7. **Configure Services in `Program.cs`**:
   Update `Program.cs` to configure services, CORS, and authentication:
   ```csharp
   var builder = WebApplication.CreateBuilder(args);

   builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseInMemoryDatabase("FantasyDb"));
   builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
       .AddEntityFrameworkStores<ApplicationDbContext>();
   builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie();
   builder.Services.AddControllers();
   builder.Services.AddSwaggerGen();

   var app = builder.Build();
   app.UseSwagger();
   app.UseSwaggerUI();
   app.UseAuthentication();
   app.UseAuthorization();
   app.MapControllers();
   app.Run();
   ```

8. **Run the API**:
   ```bash
   dotnet run
   ```

### Step 2: Create the Frontend (Angular 10)

1. **Create a new Angular project**:
   Open a new terminal and run:
   ```bash
   ng new fantasy-cricket-app --routing --style=scss
   cd fantasy-cricket-app
   ```

2. **Install Angular Material (optional)**:
   If you want to use Angular Material for UI components:
   ```bash
   ng add @angular/material
   ```

3. **Create necessary folders**:
   ```bash
   mkdir src/app/components src/app/models src/app/services
   ```

4. **Create Angular Services**:
   Create services in the `services` folder (e.g., `auth.service.ts`, `match.service.ts`, etc.) to handle API calls.

5. **Create Angular Components**:
   Create components in the `components` folder (e.g., `header`, `dashboard`, `contests`, etc.) using Angular CLI:
   ```bash
   ng generate component components/header
   ng generate component components/dashboard
   ng generate component components/contests
   ```

6. **Set Up Routing**:
   Update `app-routing.module.ts` to define routes based on the application structure outlined in the README.md.

7. **Implement UI Logic**:
   Implement the logic for each component and service based on the requirements in the README.md.

8. **Run the Angular Application**:
   ```bash
   ng serve
   ```

### Step 3: Testing and Finalization

1. **Test the API**: Use tools like Postman or Swagger UI to test the API endpoints.
2. **Test the Angular Application**: Ensure all components and services work as expected.
3. **Implement Error Handling**: Add error handling and validation as specified in the README.md.
4. **Document the Application**: Update the README.md with setup instructions, usage, and any known issues.

### Step 4: Future Enhancements

Consider implementing future enhancements as outlined in the README.md, such as real-time updates, leaderboards, and social features.

### Conclusion

By following these steps, you will have a basic implementation of a fantasy cricket web application using .NET 6 for the backend and Angular 10 for the frontend, adhering to the specifications provided in the README.md file.