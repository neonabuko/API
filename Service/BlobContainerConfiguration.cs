using Azure.Storage.Blobs;

namespace SongManager.Data;

public class BlobContainerConfiguration
{
    public BlobContainerClient CreateBlobContainerClient(IConfiguration configuration) {
        string connectionString = configuration.GetConnectionString("AzureContext") ?? throw new Exception();
        string containerName = "songmanager";
        var blobServiceClient = new BlobServiceClient(connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        return blobContainerClient;
    }
}