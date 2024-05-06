using ScoreHubAPI.Repositories;
using ScoreHubAPI.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ScoreHubAPI.Service;

public class SongService(IMusicRepository<Song> songRepository, string _songsPath)
: MusicService<Song>(songRepository, _songsPath)
{
    public FileStreamResult Stream(HttpRequest request, HttpResponse response, string name)
    {
        var fileStream = GetFileByNameAsync(name);

        if (request.Headers.TryGetValue("Range", out var rangeValues))
        {
            var rangeHeader = rangeValues.FirstOrDefault();
            if (rangeHeader != null && rangeHeader.StartsWith("bytes="))
            {
                return StreamRange(response, fileStream, rangeHeader);
            }
        }

        return new FileStreamResult(fileStream, "audio/mpeg")
        {
            EnableRangeProcessing = true
        };
    }

    private static FileStreamResult StreamRange(HttpResponse response, FileStream fileStream, string rangeHeader)
    {
        long fileSize = fileStream.Length;
        var rangeParts = rangeHeader[6..].Split('-');
        long startByte = long.Parse(rangeParts[0]);
        long endByte = (rangeParts.Length > 1 && !string.IsNullOrWhiteSpace(rangeParts[1]))
                        ? long.Parse(rangeParts[1])
                        : fileSize - 1;

        var responseLength = endByte - startByte + 1;

        fileStream.Seek(startByte, SeekOrigin.Begin);

        var responseStream = new FileStreamResult(fileStream, "audio/mpeg")
        {
            EnableRangeProcessing = true
        };


        response.StatusCode = 206;
        response.Headers.ContentRange = $"bytes {startByte}-{endByte}/{fileSize}";
        response.Headers["Content-Length"] = responseLength.ToString();

        return responseStream;
    }
}


