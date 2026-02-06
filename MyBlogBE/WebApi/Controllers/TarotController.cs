using System;
using System.Text.Json;
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

    public TarotController(IOptions<BaseSettings> baseSettings, ITarotService service)
    {
        _baseSettings = baseSettings.Value;
        _httpClient = new HttpClient();
        _service = service;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetTarotAsync()
    {
        var cards = await _service.GetCardsAsync();
        return HandleResponse(Success(cards));
    }

    [HttpPost("")]
    public async Task<IActionResult> GetTarotInterpretationAsync(TarotReadingRequest request)
    {
        var apiKey = _baseSettings.Gemini.ApiKey;
        var model = _baseSettings.Gemini.Model;
        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

        var cards = new List<TarotReadingResponse>();

        var prompt =
            $@"You are a master Tarot reader with years of experience providing multilingual support. 
            Please decode this message deeply and provide advice.";

        if (request.SpreadType != null)
        {
            prompt += $@" Spread type: {request.SpreadType}.";
        }
        if (request.Time != null)
        {
            prompt += $@" Within the timeframe: {request.Time}.";
        }
        if (request.Language != null)
        {
            prompt += $@" In language: {request.Language}.";
        }
        if (request.NumberOfCards != null)
        {
            cards = await _service.DrawCardAsync(request.NumberOfCards.Value);
        }

        if (request.Question != null)
        {
            prompt += $@" The questioner's question is: {request.Question}.";
        }

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

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);
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
}
