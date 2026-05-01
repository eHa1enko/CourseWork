namespace CourseWork.API.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadAudioAsync(Stream stream, string fileName);
        Task<string> UploadImageAsync(Stream stream, string fileName, string folder = "covers");
        Task DeleteAudioAsync(string url);
        Task DeleteImageAsync(string url);
    }
}
