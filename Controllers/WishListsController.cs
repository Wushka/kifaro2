using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using kifaro.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace kifaro.Controllers
{
    public class WishListsController : Controller
    {
        private readonly kifaroContext _context;

        public WishListsController(kifaroContext context)
        {
            _context = context;
        }
        // GET: /<controller>/

        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        public IActionResult Kirascorner(int user)
        {
            User cur = _context.User.Where(s => s.ID == user).First();
            HttpContext.Items["userID"] = user;
            return View(cur);
        }

        public class EloRating
        {
            // Updates the scores in the passed matchup.
            // The Matchup to update
            // Whether User 1 was the winner (false if User 2 is the winner)
            // The desired Diff
            // The desired KFactor
            public static void UpdateScores(Matchup matchup, bool user1WonMatch, int diff = 400, int kFactor = 10)
            {
                double est1 = 1 / Convert.ToDouble(1 + 10 ^ (matchup.User2Score - matchup.User1Score) / diff);
                double est2 = 1 / Convert.ToDouble(1 + 10 ^ (matchup.User1Score - matchup.User2Score) / diff);
                int sc1 = 0;
                int sc2 = 0;
                if (user1WonMatch) sc1 = 1;
                else sc2 = 1;
                matchup.User1Score = Convert.ToInt32(Math.Round(matchup.User1Score + kFactor * (sc1 - est1)));
                matchup.User2Score = Convert.ToInt32(Math.Round(matchup.User2Score + kFactor * (sc2 - est2)));
            }
            // Updates the scores in the match, using default Diff and KFactors (400, 100)
            // The Matchup to update
            // Whether User 1 was the winner (false if User 2 is the winner)
            public static void UpdateScores(Matchup matchup, bool user1WonMatch)
            {
                UpdateScores(matchup, user1WonMatch, 400, 10);
            }
            public class Matchup
            {
                public int User1Score { get; set; }
                public int User2Score { get; set; }
            }
        }
    }

}
