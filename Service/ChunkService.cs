namespace Service;

public class ChunkService
{
    private readonly string _tempDirectory = "/app/songs/tmp";
    private readonly string _songDir = "/app/songs";

    public async Task StoreChunkAsync(string fileIdentifier, int chunkNumber, IFormFile chunkData)
    {
        string chunkFilePath = GetChunkFilePath(fileIdentifier, chunkNumber);
        using var fileStream = new FileStream(chunkFilePath, FileMode.Create, FileAccess.Write);
        await chunkData.CopyToAsync(fileStream);
    }

    public async Task<bool> IsFileCompleteAsync(string fileIdentifier, int totalChunks)
    {
        var chunkFileNames = Enumerable.Range(1, totalChunks)
                                        .Select(chunkNumber => GetChunkFilePath(fileIdentifier, chunkNumber));
        return await Task.Run(() => chunkFileNames.All(File.Exists));
    }

    public async Task ReconstructFileAsync(string fileIdentifier, int totalChunks)
    {
        var chunkFileNames = Enumerable.Range(1, totalChunks)
                                        .Select(chunkNumber => GetChunkFilePath(fileIdentifier, chunkNumber))
                                        .OrderBy(chunkFilePath => chunkFilePath);

        var outputFilePath = Path.Combine(_songDir + $"/{fileIdentifier}");
        using var outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);
        foreach (var chunkFilePath in chunkFileNames)
        {
            using var chunkFileStream = new FileStream(chunkFilePath, FileMode.Open, FileAccess.Read);
            await chunkFileStream.CopyToAsync(outputFileStream);
        }
    }

    public async Task DeleteTempChunksAsync(string fileIdentifier, int totalChunks)
    {
        var chunkFileNames = Enumerable.Range(1, totalChunks)
                                        .Select(chunkNumber => GetChunkFilePath(fileIdentifier, chunkNumber))
                                        .OrderBy(chunkFilePath => chunkFilePath);

        foreach (var chunkFileName in chunkFileNames)
        {
            if (File.Exists(chunkFileName))
            {
                await Task.Run(() => File.Delete(chunkFileName));
            }
        }
    }

    private string GetChunkFilePath(string fileIdentifier, int chunkNumber)
    {
        return Path.Combine(_tempDirectory, $"{fileIdentifier}_chunk_{chunkNumber}.tmp");
    }
}