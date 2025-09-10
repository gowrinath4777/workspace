1) Purpose & Scope
Build a two‑player fantasy cricket web app (Dream11‑style, simplified) where:
Admin creates matches, defines the player pool, manually updates scores (dummy data), and declares results.
Users register/login, create or join a contest, pick a team (e.g., 11 players) from the match’s player pool, and view live score updates and results.
Frontend: Angular 10 (responsive, reactive forms)
Backend: .NET 6 Web API with EF Core and Swagger (session/cookie auth)
Timebox: 32 hours (as per your breakdown)
2) High‑Level Architecture
Client (Angular 10)
Routing-driven SPA with feature modules: Auth, Matches, Contests, Admin  
Services for API calls, guards for auth/roles, toasts for feedback, reactive forms for validation  
Cookie-based session auth → all HTTP calls use withCredentials
API (.NET 6 Web API)
Controllers per resource: Auth, Matches, PlayerPools (Admin), Admin (scores/results), Contests, UserTeams  
Business logic in services (e.g., IContestService, IScoreService)  
Persistence via Entity Framework Core (ApplicationDbContext)  
Authentication via ASP.NET Core Identity (cookie/session), roles for Admin vs User  
Swagger for documentation and manual testing
Database
EF Core entities: Match, Player, MatchPlayer, Contest, UserTeam, UserTeamPlayer, ApplicationUser  
Dev: InMemory (fast start); can switch to SQL Server with connection string
3) Domain Model (Names & Relationships)
Entities (exact names)
ApplicationUser (extends IdentityUser): IsAdmin flag for role-based access.
Match: MatchId, TeamA, TeamB, Date; has many MatchPlayers and Contests.
Player: PlayerId, Name, Role (enum: Batsman | Bowler | AllRounder | WicketKeeper), Team.
MatchPlayer: joins Match + Player; holds current Score for that player in that match (admin-updated).
Contest: ContestId, MatchId, CreatedByUserId, JoinedByUserId?, Status (enum: Pending | Ongoing | Completed), WinnerUserId?; has many UserTeams.
UserTeam: the team a user submits for a specific contest.
UserTeamPlayer: link table for selected MatchPlayer entries in a UserTeam.
Key relationships & constraints
A Match has a player pool via MatchPlayers.
A Contest belongs to one Match, and supports two users (creator and joiner).
A UserTeam belongs to one Contest and one User (one team per user per contest).
Selected players in a team must be from the same match (via MatchPlayer).
Scores live in MatchPlayer.Score (per match); team totals are computed by summing selected MatchPlayer.Score.
4) Business Rules
Team selection
Exactly 11 players (configurable later if needed).
All players must be MatchPlayer records for the contest’s match.
One team per user per contest.
Contest lifecycle
Pending → created; waiting for second user and teams.
Ongoing → both users have joined (optionally when both submit teams).
Completed → admin declares result; set WinnerUserId.
Scores
Admin-only manual updates to MatchPlayer.Score (dummy data).
Team score = sum of scores for its selected MatchPlayerIds.
Users
Register/login; session-based auth via cookies.
Can create one contest per match and join another available contest.
Admin
Creates matches; adds players to match’s player pool.
Updates scores (individually or in bulk).
Declares contest results (after both teams exist).
5) API Design (Controllers & Routes — names only, no code)
Controllers (names)
AuthController
MatchesController
PlayerPoolsController (admin operations for matches & pools)
AdminController (scores & results)
ContestsController
UserTeamsController
Key Routes (HTTP + path, summarized)
Auth
POST /api/auth/register — Create user (supports IsAdmin in dev).
POST /api/auth/login — Issue auth cookie (session-based).
POST /api/auth/logout — Clear session.
GET /api/auth/me — Current user context & role.
Matches (public)
GET /api/matches — List matches.
GET /api/matches/{id} — Match details.
GET /api/matches/{id}/players — Player pool (MatchPlayers) for match.
Admin — Player Pools & Matches (Admin only)
POST /api/admin/matches — Create match (TeamA, TeamB, Date).
POST /api/admin/matches/{matchId}/players — Add player to match pool.
POST /api/admin/matches/{matchId}/players/bulk — Bulk add players.
Admin — Scores & Results (Admin only)
POST /api/admin/scores — Update a MatchPlayer score.
POST /api/admin/results/declare — Compute totals and set WinnerUserId.
Contests (Authenticated)
GET /api/contests/by-match/{matchId} — Contests for a match.
POST /api/contests — Create contest (by a user) for a match.
POST /api/contests/{contestId}/join — Join as the second user.
GET /api/contests/{contestId} — Contest details (status, participants).
User Teams (Authenticated)
POST /api/user-teams/submit — Submit selected MatchPlayerIds for a contest.
GET /api/user-teams/by-contest/{contestId} — View my team for the contest.
GET /api/user-teams/{userTeamId}/score — Current computed score.
Security
[Authorize] on user endpoints; [Authorize(Roles = "Admin")] on admin endpoints.
All endpoints CSRF-safe via same-site cookie practices (front-end uses withCredentials).
6) Services (Backend Business Logic — names & responsibilities)
IContestService / ContestService
Create contest; join contest; submit team (validate rules); compute team score.
IScoreService / ScoreService
Update MatchPlayer.Score.
When declaring results: verify both teams exist, compute totals, update Contest.Status to Completed and set WinnerUserId.
(Business logic lives here, not controllers.)
7) Persistence Layer
ApplicationDbContext (exact name)  
DbSets: Matches, Players, MatchPlayers, Contests, UserTeams, UserTeamPlayers  
Identity integration with ApplicationUser  
Composite keys and FK relationships for UserTeamPlayer (UserTeamId, MatchPlayerId)
Seeding (Dev)
Seed one admin, sample users, a sample match, and a small player pool so the app runs instantly for demos.
8) Authentication & Authorization (Session-Based)
ASP.NET Core Identity issues a cookie on login.  
Angular always sends cookies (withCredentials: true).  
CORS allows http://localhost:4200 with credentials.  
Role: Admin role used to protect admin endpoints and routes.  
Session: you can enable server session for auxiliary state if needed (optional).
9) Frontend Information Architecture (Angular 10)
Feature Areas
Auth: Login, Register
Matches: List, Detail (with player pool)
Contests: List (by match), Create, Detail, Team Select
Admin: Dashboard (matches, player pool, scores, results)
Route Map (paths & components — names only)
/login → LoginComponent
/register → RegisterComponent
/matches → MatchListComponent
/matches/:id → MatchDetailComponent (includes player pool and contests for the match)
/contests → ContestListComponent (optional browse by match filter)
/contests/create → ContestCreateComponent
/contests/:id → ContestDetailComponent (join status, both teams’ progress, live totals)
/admin → AdminDashboardComponent
/admin/matches → AdminMatchesComponent (create match)
/admin/players → AdminPlayersComponent (add players to match pool)
/admin/scores → AdminScoresComponent (update MatchPlayer scores)
/admin/results → AdminResultsComponent (declare results)
** → NotFoundComponent
Guards
AuthGuard — protects user routes (/contests/*)
AdminGuard — protects admin module (/admin/*)
Client Services (names & responsibilities)
AuthService — register/login/logout/me (keeps current user state)
MatchService — matches and match player pools
ContestService — list, create, join, detail
AdminService — create match, add players, update scores, declare results
ToastService — notifications/snackbars
UX & Validation
Consistent Angular components, responsive layout.
Reactive form validation with control-level errors (clear messages).
Toasts/modals for actions; disable primary actions until valid (e.g., exactly 11 selected).
Loading states and empty-state views.
Accessible labels, keyboard navigation.
10) End‑to‑End User Journeys
A) Admin Flow
Login as Admin → lands on Admin Dashboard.
Create Match (TeamA, TeamB, Date).
Add Players to Match (individually or bulk) → creates MatchPlayer entries.
During/after the match, Update Scores on MatchPlayer (dummy inputs).
Once both users have teams, Declare Result for a contest → system computes totals and sets WinnerUserId.
Monitor Results and contest statuses; repeat as needed.
B) User Flow
Register/Login → session cookie stored.
Browse Matches → open a match to see its player pool and contests.
Create Contest for that match or Join an existing one.
Team Selection → pick exactly 11 players from the match’s pool and submit.
View Live Updates → as admin updates MatchPlayer.Score, team totals refresh on the contest detail page.
View Results after Admin declares → winner and final totals displayed.
11) Data Flow (Illustrative Example)
Admin creates Match M1 and adds 22 players (both teams) → creates MatchPlayers M1P*.
User A creates Contest C1 for M1. User B joins C1.
Both users submit UserTeam with 11 MatchPlayerIds each.
Admin updates scores for MatchPlayer (e.g., M1P17 score = 54).
Client fetches team totals (sum of selected MatchPlayer.Score) → displays updated scoreboard.
Admin declares result for C1 → system sets WinnerUserId, C1 status = Completed.
Users see the outcome and their team totals.
12) Error Handling & Edge Cases
Auth: invalid login → clear message; expired session → redirect to login.
Contest: joining a full contest (already has two users) → friendly error.
Team: fewer/more than 11 players → block submit with inline validation.
Cross-match players: prevent selecting players not in the contest’s match.
Admin actions: ensure only admins can create matches, add players, update scores, declare results.
Race conditions: when both users try to join same contest → server validates and returns appropriate error if already joined.
Status rules: result declaration only allowed when both teams exist.
13) Swagger Usage
Auto-generate API docs.
Tag endpoints by controller: Auth, Matches, Admin, Contests, UserTeams.
Provide example payloads (e.g., CreateMatch, UpdateScore, SubmitTeam).
Use Swagger UI to test flows end‑to‑end during dev.
14) Non‑Functional Requirements Mapping
Responsive UI: mobile-first layout; grid for lists; sticky actions on small screens.
Performance: API pagination (future), memoized client lists, minimal payloads.
Security: cookie-based auth, CORS with credentials, role-based authorization.
Reliability: validation both client and server; idempotent admin calls where possible.
Accessibility: labeled inputs, keyboard navigation, ARIA where applicable.
Observability: server logs for controller actions; client toast/error capture.
15) Environments & Config
Dev
 
API base: https://localhost:<port>/api  
CORS: allow http://localhost:4200 with credentials  
EF Core InMemory DB (fast iteration)  
Prod (later)
 
SQL Server connection  
HTTPS enforced, SameSite=None; Secure on cookies  
Reverse proxy headers, proper CORS origin(s)
16) Testing Plan
API (manual via Swagger): happy path + negative cases for each endpoint.
UI (manual): each flow (admin + user), validations, guard redirects.
Integration: create match → add pool → create contest → join → submit teams → update scores → declare result → verify winner.
Regression: re‑run after fixes; verify team count rule and role guards.
17) Project Execution Plan (32 Hours)
4h – Requirements & Design: finalize entities, routes, flows (this document).
10h – Backend API: entities, DbContext, services, controllers, auth, admin ops, Swagger, seed data.
12h – Frontend: shell, routing, guards, services, matches & contests pages, admin pages, team selection UX.
2h – Swagger Docs & Testing: polish endpoint descriptions, example payloads.
2h – Dummy Score Logic: admin score update UI + live totals display.
2h – Final Testing & Bug Fixes: end-to-end passes, validation tuning, UX polish.
18) Deliverables & Acceptance Criteria
Deliverables
Angular 10 SPA with defined routes/components and working flows.
.NET 6 Web API with Swagger, role-based auth, and seeded data.
Complete README with setup/run instructions.
Acceptance Criteria
Admin can create a match, add player pool, update scores, and declare a result.
User can register/login, create/join contest, select exactly 11 players, and see results.
Authentication persists via session cookie; unauthorized actions blocked.
Swagger exposes all endpoints and supports end-to-end manual testing.
19) Assumptions & Constraints
Two users per contest only (creator + one joiner).
Scores are manually updated (no live feed integration).
Team size fixed at 11 (can be made configurable later).
No money/entry fees or payments in this scope.
20) Future Enhancements (Out of Scope Now)
Real-time updates (SignalR/WebSockets).
Points breakdown by event types (4s, wickets, catches).
More contest sizes, leaderboards, and prize logic.
Social features (invites, chat), notifications, and emails.
Team composition rules (min/max per role/team).

Nice — I inspected the screenshots and your project description and wrote a complete, precise file-by-file guide for everything I can see in the images (both the Angular src tree and the dotnetapp backend folder). For each file/folder I list:

What it is (purpose), and

What it should contain (exact responsibilities, recommended exports/shape, and key implementation notes).


I left out the Angular interceptors content because you asked me to ignore them. If you want the interceptors documented later I can add them.


---

Top-level (repo root)

Purpose: coordinates frontend + backend projects, tooling and build files.

README.md

What it should contain: high-level project overview, how to run both frontend and backend (commands), prerequisites (Node, npm, .NET 6, EF tools), ports used (Angular 4200, API https://localhost:<port>), CORS/withCredentials note, seed data, known issues, local dev tips.

Short quick start: install, npm install in Angular, dotnet run for API, ng serve for client (or provide combined run scripts).


.gitignore

What it should contain: node_modules, dist/, bin/obj for .NET, .vs/, *.user, environment secrets, *.db (if using local DB file), coverage/, *.log.


angular.json

What it is: Angular CLI workspace config.

Should contain: projects entries, build options (index, main, styles), aot / tsConfig per environment, assets list (favicon, assets), scripts, dev server configuration. Ensure styles includes your styles.scss (or styles.css) and Tailwind import (if using Tailwind). The copy in your post already includes Tailwind reference node_modules/tailwindcss/dist/tailwind.min.css — keep if using Tailwind.


package.json

What it is: frontend dependencies & scripts.

Should contain: project name, scripts (start/build/test/lint), dependencies (@angular/*, rxjs, zone.js, lucide-angular etc.), devDependencies (Angular CLI, typescript, karma/jasmine config). Keep the versions consistent with Angular 10 (as in your package.json).


tsconfig.json, tsconfig.app.json, tsconfig.spec.json, tslint.json

What they do: TypeScript compilation settings and linting rules.

Should contain: proper target/module, paths, Angular compiler options and "skipLibCheck": true recommended for faster builds. tslint.json (Angular 10 era) with rules or you can migrate to eslint if preferred.




---

Frontend — angularapp / src (or src shown in image)

(Your src root files visible: index.html, main.ts, polyfills.ts, styles.css / styles.scss, assets/, environments/, app/)

src/index.html

Purpose: single-page app entry HTML.

Should contain: <base href="/">, meta tags (viewport), link to favicon, root <app-root></app-root>, scripts placeholder if needed for third-party libs. Keep minimal and accessible.


src/main.ts

Purpose: Angular bootstrap file.

Should contain: platformBrowserDynamic().bootstrapModule(AppModule), optional enableProdMode() depending on env, global error handling import if needed. If using SSR or HMR, auxiliary config goes here.


src/polyfills.ts

Purpose: polyfills for older browsers used by Angular 10 / RxJS.

Should contain: Angular recommended polyfills (zone.js import, core-js polyfills if needed). Keep file as generated by Angular CLI for v10.


src/styles.scss (or styles.css)

Purpose: global styles, theme, utility imports.

Should contain: Tailwind import if using; global variables, shared layouts, CSS resets, utility classes, CSS variables for colors/spacing. Use @import for component-independent global CSS. You already use app.component.scss for floating button animation — keep separation.


src/assets/

Purpose: images, fonts, JSON, mock data.

Should contain: logos (favicon already separate), sample player images, seed JSON for dev (optional), icons, any static files. Keep organized subfolders like images/, icons/, mock-data/.


src/environments/

Purpose: configuration for dev / prod builds.

Files:

environment.ts — dev constants (apiBaseUrl: https://localhost:<port>/api, debug flags, useInMemoryDb true).

environment.prod.ts — production constants (real API URL).


Should not contain secrets (API keys). Use environment variables for secrets on server.



---

src/app (folder visible in the screenshots)

This is your Angular application shell. The screenshot shows subfolders: components, interceptors (ignored), models, services, and root files: app-routing.module.ts, app.component.*, app.module.ts.

src/app/app.module.ts

Purpose: root NgModule.

Should contain:

BrowserModule, AppRoutingModule, FormsModule, ReactiveFormsModule, HttpClientModule, BrowserAnimationsModule, third-party modules (Lucide module).

Declarations: AppComponent, global layout components (HeaderComponent, Footer if present).

Providers: AuthGuard/AdminGuard, HTTP interceptors (you said ignore interceptors), services (ToastService), if using withCredentials globally you may set HttpClient defaults in services.

bootstrap: [AppComponent].



src/app/app-routing.module.ts

Purpose: declares application routes.

Should contain: RouterModule.forRoot(routes) with routes described in your high-level doc:

/login, /register, /matches, /matches/:id, /contests, /contests/create, /contests/:id, /admin/* (lazy load AdminModule), wildcard route to NotFoundComponent.

Add canActivate: [AuthGuard] on protected routes and canActivate: [AdminGuard] for admin.



src/app/app.component.ts, .html, .scss

Purpose: shell containing header, main container, footer, router outlet.

Should contain:

Template with <app-header></app-header> and <router-outlet>.

Floating action button (as you have).

Global layout styles in .scss (floating animation you included).




---

src/app/components/ (folder visible)

Purpose: UI components. The screenshot indicates many component files you already have (HeaderComponent, Dashboard, Contests, Leaderboard, StatsCard, SportsSelection, LiveMatches). For each component, include the three typical files: .ts .html .scss and optional .spec.ts.

For each component below list what it should contain:

header/ (HeaderComponent)

header.component.ts — class with methods for route detection (isActive(route: string)), menu toggles for mobile. Inject Router.

header.component.html — navigation, brand/logo, user avatar, Add Cash button, mobile nav with lucide icons.

header.component.scss — styles for header, sticky behavior, responsive layout.

Notes: Use routerLinkActive or Router.url checks for active link. Make accessible (aria-labels on toggles).


dashboard/ (DashboardComponent)

TS: fetch userStats$ observable from DataService, compose Observables for cards.

HTML: grid of <app-stats-card>, <app-sports-selection>, <app-live-matches>.

Styles: responsive grid.


contests/ (ContestsComponent)

TS: loads contests from ContestService (local DataService for demo), filtering logic (All / High / Medium / Low), progress percentage calculation (you already implemented).

HTML: list of contest items, join/create buttons.

Styles: progress bars, badges for difficulty.


leaderboard/ (LeaderboardComponent)

TS: loads leaderboard from LeaderboardService/DataService, functions to style top 3.

HTML: list of players, medals for top 3, earnings/points display.

Styles: medal colors and responsive layout.


stats-card/ (StatsCardComponent)

TS: @Input props (title, value, icon, gradientClass, textClass, iconClass).

HTML: presentational card with icon and values.

SCSS: hover animation and gradient classes.


sports-selection/ (SportsSelectionComponent)

TS: fetch sports from DataService, subscribe to selectedSport$, selectSport() to set sport in service.

HTML: grid of sport tiles (icon, name, contests, prize).

SCSS: .active-sport gradient style and hover states.

Note: don't subscribe inside getSportClass(); subscribe once in ngOnInit and store selection. You already changed this — good.


live-matches/ (LiveMatchesComponent)

TS: load matches from MatchService or DataService, helper getStatusClass().

HTML: list/tiles for matches with team names, time, status badge and Join Contest button.

SCSS: animation for .live-badge (pulse), responsive.


Other typical components you should have (not shown but required by routes):

login/, register/ — reactive forms, AuthService calls, validation messages.

match-list/, match-detail/ — list of matches and per-match pool + contests.

contest-create/, contest-detail/ — create or join contest, team selection UI (drag/drop or checkbox list).

user-team/ — team submit component with selection validation (exactly 11 players).

admin/* feature components (AdminDashboard, AdminMatches, AdminPlayers, AdminScores, AdminResults).



---

src/app/models/ (models folder visible)

Purpose: TypeScript interfaces/models used across the client.

Suggested files and what they should contain:

interfaces.ts (you already posted) — UserStats, Sport, Contest, Match, LeaderboardEntry. Good to split into domain models:

user.model.ts — export interface User { id: string; username: string; email: string; isAdmin?: boolean; }

match.model.ts — Match, MatchPlayer (id, playerId, name, role, score), Player if separate.

contest.model.ts — Contest (contestId, matchId, createdByUserId, joinedByUserId, status, winnerUserId).

user-team.model.ts — UserTeam + UserTeamPlayer.

api-payloads.ts — request/response DTO types for strong typing with HTTP calls.


Naming consistency matters — types should mirror backend DTOs.


---

src/app/services/ (services folder visible)

Purpose: services for API calls and client state.

Files & contents:

data.service.ts (you included a helpful mock DataService)

Purpose: in-memory stub for quick UI prototyping. Should expose Observables for selectedSport$, userStats$, and methods getSports(), getContests(), getMatches(), getLeaderboard().

Keep it separate from real MatchService / ContestService.


auth.service.ts

Methods: register(payload), login(credentials), logout(), me(); manage user state (BehaviorSubject), handle cookies (withCredentials true in HTTP calls).

Should store minimal user info (id, isAdmin).


match.service.ts

Methods: getMatches(), getMatchById(id), getMatchPlayers(matchId).


contest.service.ts

Methods: getContestsByMatch(matchId), createContest(payload), joinContest(contestId).


user-team.service.ts

Methods: submitTeam(payload), getTeamByContest(contestId), getScore(userTeamId).


admin.service.ts

Methods for admin endpoints: createMatch, addPlayerToMatch, bulkAddPlayers, updateScores, declareResult.


toast.service.ts

Provide UI notifications via Material Snackbar or custom toast component.


http-options.ts (optional)

Centralized withCredentials and headers for HttpClient.



Note: You said ignore interceptors — so do not mention them here. But in practice you may want an interceptor for adding CSRF headers / handling 401 redirects.


---

Backend — dotnetapp folder (visible in images)

The dotnetapp folder appears to be a .NET Web API project. Files visible: .config, Controllers, Data, Exceptions, Models, Properties, Services, Program.cs, appsettings.Development.json, appsettings.json, dotnetapp.csproj, dotnetapp.sln, README.md.

I’ll list the exact files/folders you should have in that backend project and what they must contain to fulfill your fantasy cricket app.


---

dotnetapp/Program.cs

Purpose: program entry — .NET 6 minimal host bootstrap.

Should contain:

WebApplication builder creation: var builder = WebApplication.CreateBuilder(args);

Register services: AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("FantasyDb")), AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>(), AddAuthentication(Cookie), AddAuthorization, AddControllers(), AddSwaggerGen().

DI registrations for your custom services: IContestService, IScoreService, IMatchService, IUserTeamService, IAdminService.

CORS policy to allow http://localhost:4200 with credentials: AllowCredentials(), WithOrigins("http://localhost:4200").

app.UseAuthentication(); app.UseAuthorization(); app.UseCors("AllowAngular");

Map controllers app.MapControllers();

Seed data (create sample admin, users, sample match and 22 players).


Comments: ensure cookie options SameSite = SameSiteMode.None; SecurePolicy = CookieSecurePolicy.Always in production; for dev SameSite lax or none as needed.



---

dotnetapp/appsettings.json & appsettings.Development.json

Purpose: configuration for logging, connection strings, identity/cookie settings.

Should contain:

Logging section, AllowedHosts, ConnectionStrings (for InMemory dev no connection string required; for SQL Server provide template).

Identity cookie settings if customizing.




---

dotnetapp/dotnetapp.csproj

Purpose: project file with package references.

Should contain references to:

Microsoft.AspNetCore.App (implicit in SDK),

Microsoft.EntityFrameworkCore.InMemory (dev),

Microsoft.EntityFrameworkCore.SqlServer (for prod),

Microsoft.AspNetCore.Identity.EntityFrameworkCore,

Swashbuckle.AspNetCore (Swagger),

AutoMapper (optional),

FluentValidation (optional for DTO validation).


Target framework net6.0.



---

dotnetapp/Controllers/ (folder visible)

Create controllers named like you described. Each controller should be annotated with route and [ApiController]. Use DTOs for input/output.

AuthController

Routes:

POST /api/auth/register — Model: RegisterDto (Username, Email, Password, IsAdmin? for dev). Creates ApplicationUser using UserManager, optionally seed IsAdmin role.

POST /api/auth/login — LoginDto returns Cookie (sets auth cookie via SignInManager.PasswordSignInAsync). Use HttpOnly session cookie.

POST /api/auth/logout — SignInManager.SignOutAsync.

GET /api/auth/me — returns current user info from User claims.


Security: no [Authorize] on register/login; [Authorize] on me.


MatchesController

Routes:

GET /api/matches — list matches.

GET /api/matches/{id} — match details.

GET /api/matches/{id}/players — returns MatchPlayer pool for match.


Implementation: use IMatchService to read DbContext.


PlayerPoolsController (Admin)

Routes:

POST /api/admin/matches — create match (accept DTO: TeamA, TeamB, Date).

POST /api/admin/matches/{matchId}/players — add a single player to match pool.

POST /api/admin/matches/{matchId}/players/bulk — add many players at once (bulk DTO).


Protect with [Authorize(Roles = "Admin")].


AdminController (Admin-only scores & results)

Routes:

POST /api/admin/scores — update a MatchPlayer score (single/bulk). DTO: { MatchPlayerId, Score } or array.

POST /api/admin/results/declare — declare result for contest. Input: { ContestId } or more details. Implementation: compute scores for both user teams, set Contest.WinnerUserId, Contest.Status = Completed.


Protect with [Authorize(Roles = "Admin")].


ContestsController

Routes:

GET /api/contests/by-match/{matchId} — returns contests for a match.

POST /api/contests — create contest for a match by current user. Input: { MatchId }.

POST /api/contests/{contestId}/join — join contest as second user. Must check capacity and return error if full.

GET /api/contests/{contestId} — contest details (status, participants, maybe teams summary).


Protect create/join endpoints with [Authorize].


UserTeamsController

Routes:

POST /api/user-teams/submit — submit a team. DTO: { ContestId, SelectedMatchPlayerIds: number[] }. Validate exactly 11 players and all players belong to the contest’s match.

GET /api/user-teams/by-contest/{contestId} — return the logged in user’s team for contest (or both teams if admin).

GET /api/user-teams/{userTeamId}/score — compute and return current score (sum of MatchPlayer.Score for selected players).


Protect endpoints with [Authorize].


Controller implementation notes:

Return proper status codes: 201 Created on create, 400 Bad Request for validation errors, 403 for unauthorized admin actions, 409 Conflict if contest already full.

DTOs and model mapping: use DTO classes for request/response and map to EF entities.



---

dotnetapp/Data/ (Data folder visible)

ApplicationDbContext.cs

Should inherit from IdentityDbContext<ApplicationUser> (or IdentityDbContext<ApplicationUser, IdentityRole, string>).

DbSets: DbSet<Match> Matches, DbSet<Player> Players, DbSet<MatchPlayer> MatchPlayers, DbSet<Contest> Contests, DbSet<UserTeam> UserTeams, DbSet<UserTeamPlayer> UserTeamPlayers.

Configure composite keys for UserTeamPlayer (UserTeamId, MatchPlayerId) via OnModelCreating. Configure relationships (one-to-many, cascade rules).

Seed sample admin/user/match/players for dev (use HasData or do a runtime seeder at startup).


Migrations/ — if using EF Core migrations (optional for InMemory). Keep if switching to SQL Server.

SeedData.cs

A helper to create default roles (Admin), a default admin user with password (dev only), sample match and 22 players (11 per team) and create corresponding MatchPlayer records.




---

dotnetapp/Models/ (folder visible)

This is the domain model. Each entity as C# class.

ApplicationUser.cs

public class ApplicationUser : IdentityUser { public bool IsAdmin { get; set; } } (optional; role membership via IdentityRole is preferred).


Match.cs

Properties: int MatchId, string TeamA, string TeamB, DateTime Date, ICollection<MatchPlayer> MatchPlayers, ICollection<Contest> Contests.


Player.cs

int PlayerId, string Name, PlayerRole Role (enum), string Team (club side), other meta (batting/bowling style).


MatchPlayer.cs

int MatchPlayerId, int MatchId, int PlayerId, decimal Score (or int), nav props to Player and Match.


Contest.cs

int ContestId, int MatchId, string CreatedByUserId, string JoinedByUserId, ContestStatus Status, string WinnerUserId, ICollection<UserTeam> UserTeams.


UserTeam.cs

int UserTeamId, int ContestId, string UserId, DateTime SubmittedAt, ICollection<UserTeamPlayer>.


UserTeamPlayer.cs

Composite PK: (UserTeamId, MatchPlayerId); holds selected MatchPlayerId and maybe position/order.


Enums: PlayerRole { Batsman, Bowler, AllRounder, WicketKeeper }, ContestStatus { Pending, Ongoing, Completed }.


Validation annotations: add [Required], [MaxLength] etc. DTOs should be in Models/Dto/ or Dtos/.


---

dotnetapp/Services/ (folder visible)

Create interface + implementation per service described earlier.

IContestService & ContestService

Responsibilities: Create contest, Join contest (with checks), Validate team selection (exactly 11 and belong to same match), compute team totals.


IScoreService & ScoreService

Responsibilities: admin update MatchPlayer.Score single/bulk, events for notifying clients (optional, out of scope).


IMatchService & MatchService

Responsibilities: create match, get match details, list matches, add players to match pool.


IUserTeamService & UserTeamService

Responsibilities: submit team, get team details, calculate team score.


IAuthService (optional)

Wraps Identity operations for register/login.



Implementation notes:

Keep business logic in services; controllers act as thin HTTP adapters.

Services should return result DTOs or throw well-defined exceptions for error scenarios (use Exceptions folder).



---

dotnetapp/Exceptions/ (folder visible)

Purpose: custom exceptions to represent domain errors.

Files:

DomainException.cs (base), NotFoundException, ValidationException, ConflictException, UnauthorizedException.


Use custom middleware to map exceptions to HTTP responses (e.g., ExceptionHandlingMiddleware that returns appropriate status codes and JSON error message).



---

dotnetapp/Properties/

launchSettings.json — configures the launch URL and ports for Kestrel / IIS Express. Should include profiles for dotnet run and IISExpress. Useful for Swagger dev URL.



---

dotnetapp/README.md

Purpose: how to run the server, migrate DB (if using SQL), seed notes, Swagger URL, default admin credentials (dev only), ports, CORS configuration.



---

dotnetapp.sln

Solution file referencing dotnetapp project.



---

Cross-cutting & implementation guidance

DTOs & Validation

Use DTOs for request/response and map them to entities with AutoMapper or manual mapping.

Validate DTOs with FluentValidation or [ApiController] model state checks. Return meaningful error structure { errors: [ { field, message } ] }.


Authentication & Authorization

Use ASP.NET Core Identity with cookie authentication:

AddIdentityCore<ApplicationUser>() + AddSignInManager.

Configure cookie options: options.Cookie.HttpOnly = true, options.Cookie.SameSite = SameSiteMode.None (for localhost + cross-site), options.Cookie.SecurePolicy = CookieSecurePolicy.Always (prod).


Role management: create Admin role and seed one admin user.

Protect API endpoints with [Authorize] or [Authorize(Roles="Admin")].

Angular should call HTTP methods with { withCredentials: true } and ensure the server CORS allows credentials.


Security notes

Never include plaintext production secrets in appsettings.json. Use environment variables.

For dev seed admin credentials, note in README and delete before publishing.

CSRF: When using cookies, you should protect state changing endpoints. For simplicity, you can keep CORS strict and rely on SameSite cookies, but consider implementing anti-CSRF tokens if expanding.


Live updates (future)

You can add SignalR to push score updates in real time. For scope here, admin will manually update scores and client polls (or refetch on demand).


Testing

Add integration tests for contest lifecycle:

create match -> add players -> user A creates contest -> user B joins -> both submit teams -> admin update scores -> declare result -> verify winner.


Unit tests for services to validate the 11-player rule and cross-match selection validations.



---

Quick mapping: files seen in screenshots → what they should contain (compact checklist)

Frontend (src/ region shown)

app/app-routing.module.ts → routes & guards

app/app.component.ts|html|scss → shell + router-outlet + floating FAB

app/app.module.ts → imports, declarations, Lucide pick

app/components/* → component TS/HTML/SCSS as described for header, dashboard, contests, leaderboard, stats-card, sports-selection, live-matches, and additional pages (login/register/match/contest/admin).

app/models/* → TS interfaces mirror backend DTOs.

app/services/* → AuthService, MatchService, ContestService, AdminService, UserTeamService, DataService (mock), ToastService.

assets/ → images/icons/mock JSON

environments/environment.ts & environment.prod.ts → API URLs & flags

index.html, main.ts, polyfills.ts, styles.scss → bootstrap, global styles


Backend (dotnetapp/ region shown)

Program.cs → host + services + CORS + Swagger + seeding

appsettings.json & appsettings.Development.json → logging & connection placeholders

Controllers/ → AuthController, MatchesController, PlayerPoolsController (admin), AdminController (scores/results), ContestsController, UserTeamsController

Data/ApplicationDbContext.cs → DbSets & model configuration & seeding

Models/* → ApplicationUser, Match, Player, MatchPlayer, Contest, UserTeam, UserTeamPlayer (with enums)

Services/* → IContestService, IScoreService, IMatchService, IUserTeamService, Auth service etc.

Exceptions/* → domain exception types; exception middleware to map exceptions to HTTP codes

Properties/launchSettings.json → local debug ports

dotnetapp.csproj & .sln → project config and package refs



---

