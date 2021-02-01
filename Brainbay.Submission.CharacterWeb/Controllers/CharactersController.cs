using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Brainbay.Submission.DataAccess.Models.Domain;
using Brainbay.Submission.DataAccess;
using Brainbay.Submission.DataAccess.Models.Enums;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Brainbay.Submission.CharacterWeb.Controllers
{
    public class CharactersController : Controller
    {
        private const string CacheKeyCharacters = "CharacterList";
        private readonly RickAndMortyContext _context;
        private readonly IMemoryCache memoryCache;

        public CharactersController(RickAndMortyContext context, IMemoryCache memoryCache)
        {
            _context = context;
            this.memoryCache = memoryCache;
        }

        // GET: Characters
        public async Task<IActionResult> Index(int? locationId)
        {

            Response.Headers["X-cache-info"] = "from-cache";
            var characters = await memoryCache.GetOrCreateAsync(CacheKeyCharacters,
                async entry =>
             {
                 entry.AbsoluteExpiration = DateTime.Now.AddMinutes(5); // Absolute expiration after 5 minutes
                 Response.Headers["X-cache-info"] = "from-database";
                 return await _context.Characters.ToListAsync();
             });

            return base.View(characters.Where(n => locationId == null
                        || n.LocationUrl?.AbsoluteUri?.EndsWith("/" + locationId) == true));
        }

        // GET: Characters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var character = await _context.Characters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (character == null)
            {
                return NotFound();
            }

            return View(character);
        }

        // GET: Characters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Characters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Status,Species,Type,Gender,LocationUrl,OriginUrl,Image,Episode,Url,Created")] Character character)
        {
            if (ModelState.IsValid)
            {
                _context.Add(character);
                await _context.SaveChangesAsync();
                // Invalidate the cache
                memoryCache.Remove(CacheKeyCharacters);

                return RedirectToAction(nameof(Index));
            }
            return View(character);
        }

        // GET: Characters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var character = await _context.Characters.FindAsync(id);
            if (character == null)
            {
                return NotFound();
            }
            return View(character);
        }

        // POST: Characters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status,Species,Type,Gender,LocationUrl,OriginUrl,Image,Episode,Url,Created")] Character character)
        {
            if (id != character.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(character);
                    await _context.SaveChangesAsync();
                    // Invalidate the cache
                    memoryCache.Remove(CacheKeyCharacters);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CharacterExists(character.Id))
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
            return View(character);
        }

        // GET: Characters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var character = await _context.Characters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (character == null)
            {
                return NotFound();
            }

            return View(character);
        }

        // POST: Characters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var character = await _context.Characters.FindAsync(id);
            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();
            // Invalidate the cache
            memoryCache.Remove(CacheKeyCharacters);

            return RedirectToAction(nameof(Index));
        }

        private bool CharacterExists(int id)
        {
            return _context.Characters.Any(e => e.Id == id);
        }
    }
}
