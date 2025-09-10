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