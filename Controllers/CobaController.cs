using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using TodoApi.Models.Requests;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace TodoApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CobaController : ControllerBase {
        private readonly AppDBContext _context;

        public CobaController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MhsModel>>> index() {
            return await _context.mhs.Where(d => d.nama.Contains("be"))
            .Include(d => d.nilais)
            .ToListAsync();
        }
        
        [HttpGet("manual")]
        public IActionResult GetTodoItems()
        {
            int jumlah = _context.mhs.Count();
            MhsModel singleData = _context.mhs.Where(d => d.nrp == "215180350").First();
            
            var d = new {
                status = 200, message = "HALO",
                mhs = singleData, count = jumlah,

            };
            return Ok(d);
        }
        
        [Produces("application/json")]
        [HttpGet("query-param")]
        public ActionResult getQueryParam(string page, int coba) {
            var d = new {
                page = page
            };
            return Ok(d);
        }

        [HttpPost("query-param")]
        public ActionResult getPostQueryParam(string page, int coba) {
            var d = new {
                page = page
            };
            MhsModel m = new MhsModel() {nama = "Abraham", nrp = "21518045", 
                alamat = "Jl. Klampis", telepon = "031594776"};
            _context.mhs.Add(m);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost("post-json")] 
        public ActionResult postJson([FromBody] PostJSON data,[FromQuery(Name = "c")] string coba) {
            var result = new {
                data = data,
                msg = "ok"
            };
            return Ok(result);
        }

        [HttpPost("post-json-raw")] 
        public ActionResult postJsonRaw([FromBody] JsonElement data) {
            JsonElement d;
            var result = new {
                data = data,
                msg = "ok",
                coba = data.TryGetProperty("coba", out d)
            };
            return Ok(result);
        }

        [HttpGet("headers")]
        public ActionResult getHeader([FromHeader] string coba, 
            [FromHeader] string Auth) {
            var result = new {
                coba = coba,
                lain = Request.Headers["coba"][0]
            };
            return Ok(result);
        }

        /// <summary>
        /// Hapus Mahasiswa spesific!
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Message success/failure</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Coba/upload
        ///
        /// </remarks>
        /// <response code="200">File valid</response>
        /// <response code="400">File tidak valid, pendek!</response>
        [HttpPost("upload")]
        public ActionResult uploadFile(IFormFile file) {
            long size = file.Length;
            if (size <= 0) 
                return BadRequest(new {
                    panjang = 0,
                    message = "File pendek!"
                });
            else {
                // https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-6.0#file-upload-scenarios
                // Ambil random filename yang di temp belum dipakai
                var filePath = Path.GetTempFileName();
                using (var stream = System.IO.File.Create(filePath))
                {
                    // Ini lokasi dimana file disimpan, jadi... 
                    file.CopyToAsync(stream);
                }
                var result = new {panjang = size, path = filePath};
                return Ok(result);
            }
        }
    
        [Authorize]
        [HttpGet("profile")]
        public ActionResult getProfile([FromHeader] string Authorization) {
            string auth = Authorization.Split(" ")[1];
            JwtSecurityToken jwt = new JwtSecurityToken(auth);
            return Ok(jwt);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult login([FromBody] LoginRequest d) {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("971923712907391273921"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, d.user),
                        new Claim(JwtRegisteredClaimNames.Jti, d.user),
                        new Claim("Random Payload", "Custom"),
                    };
            var token = new JwtSecurityToken("LocalDev",
            "LocalDev",
            claims,
            expires: DateTime.Now.AddMinutes(1),
            signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}