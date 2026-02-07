using System;

namespace Application.Dtos;

public class TarotReadingResponse
{
    public string CardName { get; set; } = null!;
    public bool IsReversed { get; set; }
    public string ImageUrl { get; set; } = null!;
}

public class GuidedTarotRequest
{
    public int NumberOfCards { get; set; } = 3;
    public string SpreadType { get; set; } = null!;
    public string Time { get; set; } = null!;
    public string Language { get; set; } = "en";
}

public class CustomTarotRequest
{
    public string Question { get; set; } = null!;
    public string Language { get; set; } = "en";
}
