using System;

namespace Application.Dtos;

public class TarotReadingResponse
{
    public string CardName { get; set; } = null!;
    public bool IsReversed { get; set; }
    public string ImageUrl { get; set; } = null!;
}

public class TarotReadingRequest
{
    public int? NumberOfCards { get; set; } = null;
    public string? SpreadType { get; set; } = null;
    public string? Time { get; set; } = null;
    public string Language { get; set; } = "en";
    public string? Question { get; set; } = null;
}
