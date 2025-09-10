### Prerequisites
Ensure you have the following installed on your machine:
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Node.js](https://nodejs.org/) (includes npm)
- [Angular CLI](https://angular.io/cli) (install via npm: `npm install -g @angular/cli`)

### Step 1: Create the Backend (.NET 6 Web API)

1. **Create a new .NET 6 Web API project:**
   ```bash
   dotnet new webapi -n FantasyCricketApi
   cd FantasyCricketApi
   ```

2. **Add Entity Framework Core and Identity:**
   ```bash
   dotnet add package Microsoft.EntityFrameworkCore.InMemory
   dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
   dotnet add package Swashbuckle.AspNetCore
   ```

3. **Create the necessary folders:**
   ```bash
   mkdir Controllers Data Models Services
   ```

4. **Create the `ApplicationDbContext`:**
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

5. **Create the models:**
   Create the necessary model classes in the `Models` folder (e.g., `ApplicationUser.cs`, `Match.cs`, `Player.cs`, etc.) based on the domain model outlined in the README.md.

6. **Create the controllers:**
   Create controllers in the `Controllers` folder (e.g., `AuthController.cs`, `MatchesController.cs`, etc.) following the API design specified in the README.md.

7. **Configure services in `Program.cs`:**
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

   app.UseAuthentication();
   app.UseAuthorization();
   app.UseSwagger();
   app.UseSwaggerUI();

   app.MapControllers();
   app.Run();
   ```

8. **Seed data (optional):**
   Implement a seeding mechanism to add initial data for development.

### Step 2: Create the Frontend (Angular 10)

1. **Create a new Angular project:**
   ```bash
   ng new fantasy-cricket-app --routing --style=scss
   cd fantasy-cricket-app
   ```

2. **Install necessary dependencies:**
   ```bash
   npm install @angular/material @angular/cdk @angular/flex-layout
   ```

3. **Create the necessary folders:**
   ```bash
   mkdir src/app/components src/app/models src/app/services
   ```

4. **Create the main application structure:**
   - Create components for each feature (e.g., `HeaderComponent`, `DashboardComponent`, `ContestsComponent`, etc.) in the `components` folder.
   - Create services for API calls in the `services` folder (e.g., `auth.service.ts`, `match.service.ts`, etc.).
   - Create models in the `models` folder (e.g., `user.model.ts`, `match.model.ts`, etc.).

5. **Set up routing:**
   Update `app-routing.module.ts` to define routes based on the application structure outlined in the README.md.

6. **Implement the UI:**
   - Use Angular Material components for the UI.
   - Implement reactive forms for user registration and login.
   - Create views for matches, contests, and admin functionalities.

7. **Set up HTTP client:**
   Configure the `HttpClientModule` in `app.module.ts` and create an `HttpOptions` service if needed.

### Step 3: Run the Application

1. **Run the backend API:**
   ```bash
   dotnet run
   ```

2. **Run the Angular frontend:**
   ```bash
   ng serve
   ```

3. **Access the application:**
   Open your browser and navigate to `http://localhost:4200` for the Angular app and `https://localhost:<port>/api` for the API (replace `<port>` with the actual port number).

### Step 4: Testing and Validation

- Test the application by registering users, creating matches, joining contests, and updating scores.
- Use Swagger UI to test the API endpoints.

### Step 5: Finalize and Document

- Ensure all features are implemented as per the README.md.
- Document the setup and usage instructions in the `README.md` file of both the frontend and backend projects.

By following these steps, you will have a functional two-player fantasy cricket web application built with .NET 6 and Angular 10.