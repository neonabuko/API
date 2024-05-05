﻿namespace ScoreHubAPI.Entities.Dto;

public record SongDto(
    string Name,
    string Title,
    string? Author,
    TimeSpan Duration
);
