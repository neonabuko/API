using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;

namespace SongManager.Data;

public class BlobStorageService(string connectionString, string containerName)
{
    private readonly string _connectionString = connectionString;
    private readonly string _containerName = containerName;

    public async Task<string> UploadSongAsync(string filePath, string fileName)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        var blobClient = blobContainerClient.GetBlobClient(fileName);
        using (var fileStream = File.OpenRead(filePath))
        {
            await blobClient.UploadAsync(fileStream, true);
        }

        // Return the URL of the uploaded blob
        return blobClient.Uri.ToString();
    }
}
