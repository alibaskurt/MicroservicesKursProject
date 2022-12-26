using FreeCourse.PhotoStock.Dtos;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.PhotoStock.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : CustomControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PhotoSave(IFormFile photo, CancellationToken cancelToken)
        {
            if (photo != null && photo.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos/", photo.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await photo.CopyToAsync(stream, cancelToken);
                }
                var returnPath = "photos/" + photo.FileName;

                PhotoDto photoDto = new()
                {
                    Url = returnPath
                };

                var response = Response<PhotoDto>.Success(photoDto, 200);
                return CreateActionResultInstance(response);
            }

            return CreateActionResultInstance(Response<PhotoDto>.Fail("Photo is empty", 400));
        }

        [HttpGet]
        public IActionResult PhotoDelete(string photoUrl)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photoUrl);

            if (!System.IO.File.Exists(path))
            {
                return CreateActionResultInstance(Response<NoContent>.Fail("Photo not found", 404));
            }

            System.IO.File.Delete(photoUrl);

            return CreateActionResultInstance(Response<NoContent>.Success(204));
        }


    }
}
