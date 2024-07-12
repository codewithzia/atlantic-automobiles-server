using Atlantic.Data.MongoFs;
using Atlantic.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UploadStream;

namespace Atlantic.Api.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        const int BUF_SIZE = 4096;
        private readonly FilesService _fileService;


        class StreamModel
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }



        public FilesController(FilesService FilesService)
        {
            _fileService = FilesService;
        }


        [HttpPost("stream")]
        [AllowAnonymous]
        public async Task<IActionResult> Stream()
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();


                byte[] buffer = new byte[BUF_SIZE];
                List<MongoFileObject> photos = new List<MongoFileObject>();

                var model = await this.StreamFiles<StreamModel>((Func<IFormFile, Task>)(async x =>
                {
                    var stream = x.OpenReadStream();

                    string newFileName = DateTime.Now.Ticks + "_" + Guid.NewGuid().ToString();
                    var gridFsObjectId = await _fileService.UploadFile(stream, newFileName);

                    MongoFileObject photo = new MongoFileObject()
                    {
                        FileName = x.FileName,
                        DateCreated = DateTime.Today,
                        PhotoObjectId = gridFsObjectId
                    };
                    photos.Add(photo);
                    _fileService.Create(photo);

                }));

                return Ok(new
                {
                    Files = photos.Select(x => new
                    {
                        x.Id,
                        x.FileName,
                        x.PhotoObjectId,
                        x.DateCreated
                    })
                });
            }
            catch (Exception)
            {

                throw;
            }

        }

        [AllowAnonymous]
        [HttpGet("stream/{key}")]

        public async Task<IActionResult> GetFile([FromRoute] string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return BadRequest();
            }
            if (key.Contains("null") || key.Contains("undefined"))
            {
                return BadRequest();
            }
            MongoFileObject photo = _fileService.Get(key);

            if (photo != null)
            {
                var photoStream = await _fileService.DownloadFile(photo.PhotoObjectId);

                if (photoStream != null)
                {
                    return new FileStreamResult(photoStream, "image/jpeg") { FileDownloadName = photo.FileName };

                }
            }

            return BadRequest();
        }


        [HttpGet("save/{key}")]
        public async Task<IActionResult> SaveFile([FromRoute] string key)
        {
            try
            {
                string path = "d:\\Temp\\";
                await _fileService.SaveFile(key, path);
                return Ok();
            }
            catch { }


            return BadRequest();
        }


        [HttpGet("{id:length(24)}", Name = "GetPhoto")]
        public ActionResult<MongoFileObject> Get(string id)
        {
            var photo = _fileService.Get(id);

            if (photo == null)
            {
                return NotFound();
            }

            return photo;


        }


        [HttpPost]
        public ActionResult<MongoFileObject> Create(MongoFileObject photo)
        {

            _fileService.Create(photo);

            return CreatedAtRoute("GetPhoto", new { id = photo.Id.ToString() }, photo);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, MongoFileObject photoIn)
        {
            var photo = _fileService.Get(id);

            if (photo == null)
            {
                return NotFound();
            }

            _fileService.Update(id, photoIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var photo = _fileService.Get(id);

            if (photo == null)
            {
                return NotFound();
            }

            _fileService.Remove(photo.Id);

            return NoContent();
        }
    }
}
