using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace SongManager.Data;

public class BlobStorageService
{
    private readonly BlobContainerClient _blobContainerClient;

    public BlobStorageService(BlobContainerConfiguration blobContainerConfig, IConfiguration configuration)
    {
        _blobContainerClient = blobContainerConfig.CreateBlobContainerClient(configuration);
        SetContainerAccessAsync().Wait();
    }

    private async Task SetContainerAccessAsync() => await _blobContainerClient.SetAccessPolicyAsync(PublicAccessType.Blob);

    public async Task<string> UploadSongAsync(IFormFile file, string fileName)
    {
        string filePath = Path.GetTempFileName();
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        var blobClient = _blobContainerClient.GetBlobClient(fileName);
        using (var fileStream = File.OpenRead(filePath))
        {
            await blobClient.UploadAsync(fileStream, true);
        }
        File.Delete(filePath);
        return blobClient.Uri.ToString();
    }

    public async Task<ICollection<SongDto>> GetAllSongsAsync()
    {
        var songs = new List<SongDto>();

        await foreach (var song in _blobContainerClient.GetBlobsAsync())
        {
            var songName = _blobContainerClient.GetBlobClient(song.Name).Name.ToString();
            var songUrl = _blobContainerClient.GetBlobClient(song.Name).Uri.ToString();

            SongDto dto = new(
                songName,
                songUrl
            );
            
            songs.Add(dto);
        }

        return songs;
    }

    public async Task DeleteSongAsync(string name) {
        var song = _blobContainerClient.GetBlobClient(name);
        await song.DeleteIfExistsAsync();
    }
}
