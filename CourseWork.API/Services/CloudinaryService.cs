using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace CourseWork.API.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
        }

        // аудіо завантажується як video resource — так вимагає cloudinary api
        public async Task<string> UploadAudioAsync(Stream stream, string fileName)
        {
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = "songs"
            };
            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString();
        }

        public async Task<string> UploadImageAsync(Stream stream, string fileName, string folder = "covers")
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = folder
            };
            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString();
        }

        public async Task DeleteAudioAsync(string url)
        {
            var publicId = ExtractPublicId(url);
            if (publicId is null) return;
            await _cloudinary.DestroyAsync(new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Video
            });
        }

        public async Task DeleteImageAsync(string url)
        {
            var publicId = ExtractPublicId(url);
            if (publicId is null) return;
            await _cloudinary.DestroyAsync(new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image
            });
        }

        // витягує public_id з url, пропускає версійний сегмент (v123...)
        private static string? ExtractPublicId(string url)
        {
            try
            {
                var uri = new Uri(url);
                var parts = uri.AbsolutePath.Split('/');
                var uploadIdx = Array.IndexOf(parts, "upload");
                if (uploadIdx < 0) return null;

                var start = uploadIdx + 1;
                if (start < parts.Length && parts[start].StartsWith('v') && long.TryParse(parts[start][1..], out _))
                    start++;

                var withExt = string.Join("/", parts[start..]);
                return Path.ChangeExtension(withExt, null);
            }
            catch { return null; }
        }
    }
}
