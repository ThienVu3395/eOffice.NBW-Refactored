using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Clone_KySoDienTu.Helpers
{
    public static class FileHelper
    {
        public static HttpResponseMessage CreateFileResponse(
            byte[] bytes,
            string contentType,
            string fileName)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Không tạo được file.")
                };
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes)
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName,
                FileNameStar = fileName
            };

            return response;
        }
    }
}