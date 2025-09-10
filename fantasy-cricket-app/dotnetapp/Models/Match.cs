using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FantasyCricketApi.Models
{
    public class Match
    {
        public int MatchId { get; set; }

        [Required]
        public string TeamA { get; set; }

        [Required]
        public string TeamB { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Navigation properties
        public virtual ICollection<MatchPlayer> MatchPlayers { get; set; }
        public virtual ICollection<Contest> Contests { get; set; }
    }
}