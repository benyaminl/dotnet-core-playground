#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MhsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public MhsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Mhs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MhsModel>>> Getmhs()
        {
            return await _context.mhs.ToListAsync();
        }

        // GET: api/Mhs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MhsModel>> GetMhsModel(string id)
        {
            var mhsModel = await _context.mhs
                .Include(i => i.nilais)
                .FirstOrDefaultAsync(i => i.nrp == id);

            if (mhsModel == null)
            {
                return NotFound();
            }

            return mhsModel;
        }

        // PUT: api/Mhs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMhsModel(string id, MhsModel mhsModel)
        {
            if (id != mhsModel.nrp)
            {
                return BadRequest();
            }

            _context.Entry(mhsModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MhsModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Mhs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MhsModel>> PostMhsModel(MhsModel mhsModel)
        {
            _context.mhs.Add(mhsModel);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MhsModelExists(mhsModel.nrp))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMhsModel", new { id = mhsModel.nrp }, mhsModel);
        }

        /// <summary>
        /// Hapus Mahasiswa spesific!
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Nothing</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/Mhs/5
        ///
        /// </remarks>
        /// <response code="200">Mahasiswa terhapus</response>
        /// <response code="400">Jika ID nya tidak ada</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMhsModel(string id)
        {
            var mhsModel = await _context.mhs.FindAsync(id);
            if (mhsModel == null)
            {
                return NotFound();
            }

            _context.mhs.Remove(mhsModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MhsModelExists(string id)
        {
            return _context.mhs.Any(e => e.nrp == id);
        }
    }
}
