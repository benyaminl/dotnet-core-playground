
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel;
using System.Web;
using System.Reactive.Linq; // Linq? Ini baru sih 
using System.Diagnostics;

namespace TodoApi.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FilesController : ControllerBase {

        MinioClient _minio;
        ILogger<FilesController> _log;
        public FilesController(MinioClient minio, ILogger<FilesController> log) {
            _minio = minio;
            _log = log;
        }

        [HttpGet("")] 
        public async Task<IActionResult> getBucketList(){
            var list = _minio;
            try {
                await list.ListBucketsAsync();
            } catch (Exception e) {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(e, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame?.GetFileLineNumber();
                // example of using double exception to point the error
                string msg = e.Message + " | Minio Get File List - Line : " + line;

                throw new Exception(msg,e);
            }
            return Ok(list);
        }

        [HttpPost("")]
        public async Task<IActionResult> addFileToS3(IFormFile f){
            // var list = await _minio.ListBucketsAsync();
            MemoryStream ms = new MemoryStream();
            await f.OpenReadStream().CopyToAsync(ms);
            byte[] bs = ms.ToArray();
            
            Aes aesEncryption = Aes.Create();
            aesEncryption.KeySize = 256;
            aesEncryption.GenerateKey();
            var ssec = new SSEC(aesEncryption.Key);
            System.IO.MemoryStream filestream = new System.IO.MemoryStream(bs);
            // 
            var nameTemp = f.FileName.Split("/");
            int index = nameTemp.Length-1;
            var name = nameTemp[index];

            await _minio.PutObjectAsync("coba-bucket", "img/"+name, filestream, filestream.Length, f.ContentType);
            return Ok(new {msg = "done", type = f.ContentType});
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> removeFile(string fileName) {
            fileName = HttpUtility.UrlDecode(fileName);
            try {
                await _minio.RemoveObjectAsync("coba-bucket", fileName);
            } catch (Exception e) {
                return BadRequest(e);
            }

            return Ok(new { file =  fileName});
        }

        [HttpPost("{fileName}/share")]
        public async Task<IActionResult> shareFile(string fileName) {
            fileName = HttpUtility.UrlDecode(fileName);
            string link = "";
            var result = new { msg = new List<Object>()};
            try {
                IObservable<Item> d = _minio.ListObjectsAsync("coba-bucket", fileName.Substring(0, fileName.Length - 2), true);
                IDisposable subscription = d.Subscribe(item => {
                    Console.WriteLine("OnNext: {0}", item.Key);
                    result.msg.Add(item.Key);
                },
                ex => Console.WriteLine("OnError: {0}", ex.Message),
                () => Console.WriteLine("OnComplete: {0}"));
                int jumlah = await d.Count();
                if (jumlah <= 0) {
                    throw new FileNotFoundException("File " + fileName + " not found");
                }

                link = await _minio.PresignedGetObjectAsync("coba-bucket", fileName, 60*60*24*2);
            } catch(InvalidOperationException e) {
                return Problem(e.ToString(), statusCode: 500, title: "oops - " + fileName.Substring(0, fileName.Length - 2));
                // return NotFound(new {status = 404, msg = "File " + fileName + " not found"});
            } catch (FileNotFoundException f) {
                return NotFound(new {status = 404, msg = f.Message});
            }

            return Ok(new {result = result, link= link});
        }

        [HttpGet("add")]
        public async Task<IActionResult> shareUploadLink(string fileName) {
            // in 2 hours, link dead!
            string link = await _minio.PresignedPutObjectAsync("coba-bucket", fileName, 60*60*2);
            
            return Ok(new {link = link});
        }
    }
}