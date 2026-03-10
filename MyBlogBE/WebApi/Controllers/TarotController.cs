using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using Application.Dtos;
using Application.Services.Interfaces;
using Application.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WebApi.Controllers;

[Route("api/tarot")]
[ApiController]
public class TarotController : BaseController
{
    private readonly BaseSettings _baseSettings;
    private readonly HttpClient _httpClient;
    private readonly ITarotService _service;

    private readonly string _apiKey;
    private readonly string _model;
    private readonly string _url;

    public TarotController(IOptions<BaseSettings> baseSettings, ITarotService service)
    {
        _baseSettings = baseSettings.Value;
        _httpClient = new HttpClient();
        _service = service;
        _model = _baseSettings.Gemini.Model;
        _apiKey = _baseSettings.Gemini.ApiKey;
        _url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
    }

    [HttpGet("")]
    public async Task<IActionResult> GetTarotAsync()
    {
        var cards = await _service.GetCardsAsync();
        return HandleResponse(Success(cards));
    }

    [HttpPost("guided")]
    public async Task<IActionResult> GetGuidedTarot(GuidedTarotRequest request)
    {
        var prompt =
            $@"You are a master Tarot reader with years of experience providing multilingual support. 
            Please decode this message deeply and provide advice.";

        prompt += $@" Spread type: {request.SpreadType}.";

        prompt += $@" Within the timeframe: {request.Time}.";

        prompt += $@" In language: {request.Language}.";

        var cards = await _service.DrawCardAsync(request.NumberOfCards);

        foreach (var card in cards)
        {
            prompt += $@" Card: {card.CardName} {(card.IsReversed ? "(Reversed)" : "(Upright)")}";
        }

        prompt +=
            @" Please provide a detailed explanation of the meaning of the drawn cards, 
            connect them together and give specific advice to the questioner. 
            Return the result formatted using HTML tags (such as <p>, <br/>, <strong>, <h3>). Do not use Markdown.";

        var requestBody = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
        };

        var response = await _httpClient.PostAsJsonAsync(_url, requestBody);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        var result =
            json.GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString()
            ?? throw new Exception("No response from Gemini API");

        return HandleResponse(Success(result));
    }

    [HttpPost("custom")]
    public async Task<IActionResult> GetCustomTarot(CustomTarotRequest request)
    {
        int cardCount = await PredictCardCount(request.Question);

        var prompt =
            $@"You are a master Tarot reader with years of experience providing multilingual support. 
            Please decode this message deeply and provide advice.";

        prompt += $@" In language: {request.Language}.";

        prompt += $@" Question from the user: {request.Question}.";

        var cards = await _service.DrawCardAsync(cardCount);
        foreach (var card in cards)
        {
            prompt += $@" Card: {card.CardName} {(card.IsReversed ? "(Reversed)" : "(Upright)")}";
        }

        prompt +=
            @" Please provide a detailed explanation of the meaning of the drawn cards, 
            connect them together and give specific advice to the questioner. Return the result formatted using HTML tags (such as <p>, <br/>, <strong>, <h3>). Do not use Markdown.";

        var requestBody = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
        };

        var response = await _httpClient.PostAsJsonAsync(_url, requestBody);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        var result =
            json.GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString()
            ?? throw new Exception("No response from Gemini API");

        return HandleResponse(Success(result));
    }

    private async Task<int> PredictCardCount(string question)
    {
        var predictPrompt =
            $@"
        Analyze the user's tarot question: '{question}'
        Decide how many cards to draw:
        - 1 card: Simple questions, Yes/No, or daily energy.
        - 3 cards: Relationships, career paths, or past-present-future.
        - 5 or more cards: Complex problems, deep spiritual guidance, or major life shifts.
        If the question already contains a card count, use that.
        If the question already contains the card name, use 0.
        Return ONLY the number (1, 3, 5, 6, 7, 11...). No text, no explanation.";

        var requestBody = new
        {
            contents = new[] { new { parts = new[] { new { text = predictPrompt } } } },
            generationConfig = new
            {
                temperature = 0.1, // Low temperature for focused responses
                maxOutputTokens = 2, // Only return a number, so limit tokens
            },
        };

        var response = await _httpClient.PostAsJsonAsync(_url, requestBody);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var textResult = json.GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        var match = Regex.Match(textResult ?? "3", @"\d");
        return match.Success ? int.Parse(match.Value) : 3;
    }
}
