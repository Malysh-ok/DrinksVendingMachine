using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain.DbContexts;
using Domain.Entities;
using Domain.Entities.Enums;

namespace App.Main.Controllers
{
    public class NewController : Controller
    {
        private readonly AppDbContext _context;

        public NewController(AppDbContext context)
        {
            _context = context;
        }

        // GET: New
        public async Task<IActionResult> Index()
        {
              return _context.Coins != null
                  ? View(await _context.Coins.ToListAsync()) 
                  : Problem("Entity set 'AppDbContext.Coins'  is null.");
        }

        // GET: New/Details/5
        public async Task<IActionResult> Details(CoinEnm id)
        {
            if (id == null || _context.Coins == null)
            {
                return NotFound();
            }

            var coin = await _context.Coins
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coin == null)
            {
                return NotFound();
            }

            return View(coin);
        }

        // GET: New/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: New/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Count")] Coin coin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coin);
        }

        // GET: New/Edit/5
        public async Task<IActionResult> Edit(CoinEnm id)
        {
            if (id == null || _context.Coins == null)
            {
                return NotFound();
            }

            var coin = await _context.Coins.FindAsync(id);
            if (coin == null)
            {
                return NotFound();
            }
            return View(coin);
        }

        // POST: New/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CoinEnm id, [Bind("Id,Count")] Coin coin)
        {
            if (id != coin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoinExists(coin.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(coin);
        }

        // GET: New/Delete/5
        public async Task<IActionResult> Delete(CoinEnm id)
        {
            if (id == null || _context.Coins == null)
            {
                return NotFound();
            }

            var coin = await _context.Coins
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coin == null)
            {
                return NotFound();
            }

            return View(coin);
        }

        // POST: New/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(CoinEnm id)
        {
            if (_context.Coins == null)
            {
                return Problem("Entity set 'AppDbContext.Coins'  is null.");
            }
            var coin = await _context.Coins.FindAsync(id);
            if (coin != null)
            {
                _context.Coins.Remove(coin);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoinExists(CoinEnm id)
        {
          return (_context.Coins?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
