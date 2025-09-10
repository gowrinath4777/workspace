namespace FantasyCricketApi.Models.Dtos
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class CreateContestDto
    {
        public int MatchId { get; set; }
    }

    public class UpdateScoreDto
    {
        public int MatchId { get; set; }
        public int PlayerId { get; set; }
        public int Score { get; set; }
    }

    public class SubmitTeamDto
    {
        public int ContestId { get; set; }
        public List<int> MatchPlayerIds { get; set; }
    }
}
