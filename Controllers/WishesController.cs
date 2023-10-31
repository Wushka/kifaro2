using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kifaro.Models;

namespace kifaro.Controllers
{
    public class WishesController : Controller
    {
        private readonly kifaroContext _context;

        public WishesController(kifaroContext context)
        {
            _context = context;
        }

        // GET: Wishes
        public async Task<IActionResult> Index(int user)
        {
            User cur_user = _context.User.Where(s => s.ID == user).First();
            ViewData["UserName"] = cur_user.Name;
            var retval = await _context.Wish.Where(s => s.UserID == user).ToListAsync();
            return View(retval);
        }

        // GET: Wishes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wish = await _context.Wish
                .SingleOrDefaultAsync(m => m.ID == id);
            if (wish == null)
            {
                return NotFound();
            }

            return View(wish);
        }

        // GET: Wishes/Create
        public IActionResult Create(int user)
        {
            ViewData["UserID"] = user;
            return View();
        }

        // POST: Wishes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Url,PictureUrl,Price,UserID")] Wish wish)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { user = wish.UserID });
            }
            return View(wish);
        }

        // GET: Wishes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wish = await _context.Wish.SingleOrDefaultAsync(m => m.ID == id);
            if (wish == null)
            {
                return NotFound();
            }
            ViewData["userID"] = wish.UserID;

            return View(wish);
        }

        // POST: Wishes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Url,PictureUrl,Price,Rank,UserID")] Wish wish)
        {
            if (id != wish.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WishExists(wish.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { user = wish.UserID });
            }
            return RedirectToAction(nameof(Index), new { user = wish.UserID });
        }

        // GET: Wishes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wish = await _context.Wish
                .SingleOrDefaultAsync(m => m.ID == id);
            if (wish == null)
            {
                return NotFound();
            }

            return View(wish);
        }

        // POST: Wishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wish = await _context.Wish.SingleOrDefaultAsync(m => m.ID == id);
            int user_id = wish.UserID;
            _context.Wish.Remove(wish);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { user=user_id});
        }

        private bool WishExists(int id)
        {
            return _context.Wish.Any(e => e.ID == id);
        }

        public async Task<IActionResult> Rankwishes(int user)
        {
            Random r = new Random();
            List<Wish> user_wishes = await _context.Wish.Where(s => s.UserID == user).ToListAsync();

            int r1 = r.Next(user_wishes.Count);
            int r2 = r.Next(user_wishes.Count);
            while (r1 == r2)
            {
                r2 = r.Next(0, user_wishes.Count);
            }
            
            Wish wish1 = user_wishes.ToList()[r1];
            Wish wish2 = user_wishes.ToList()[r2];
            Wish[] list = {wish1, wish2};
            return View(list);
        }

        public async Task<IActionResult> UpdateScores(int w1_id, int w2_id, bool user1WonMatch, int diff = 300, int kFactor = 24)
        {
            Wish? w1 = _context.Wish.SingleOrDefault(m => m.ID == w1_id);
            Wish? w2 = _context.Wish.SingleOrDefault(m => m.ID == w2_id);
            var i = _context.Wish.Count();
            double est1 = 1 / Convert.ToDouble(1 + 10 ^ (w2.Rank - w1.Rank) / diff);
            double est2 = 1 / Convert.ToDouble(1 + 10 ^ (w1.Rank - w2.Rank) / diff);
            int sc1 = 0;
            int sc2 = 0;
            if (user1WonMatch) sc1 = 1;
            else sc2 = 1;
            w1.Rank = Convert.ToInt32(Math.Round(w1.Rank + kFactor * (sc1 - est1)));
            w2.Rank = Convert.ToInt32(Math.Round(w2.Rank + kFactor * (sc2 - est2)));
            _context.SaveChanges();
            return RedirectToAction("rankwishes", new { user=w1.UserID});
        }
    }
}
