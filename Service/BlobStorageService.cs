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

    public async Task<string> UploadSongAsync(string filePath, string fileName)
    {
        var blobClient = _blobContainerClient.GetBlobClient(fileName);
        using (var fileStream = File.OpenRead(filePath))
        {
            await blobClient.UploadAsync(fileStream, true);
        }

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
}
