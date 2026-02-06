using System.Text.Json;
using Application.Dtos;
using Application.Services.Interfaces;
using Application.Settings;
using DataAccess;
using DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementations;

public class TarotService : ITarotService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly Random _random = new Random();

    public TarotService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TarotReadingResponse>> GetCardsAsync()
    {
        var cards = await _unitOfWork.TarotCards.ReadOnly().ToListAsync();
        var response = new List<TarotReadingResponse>();

        foreach (var card in cards)
        {
            response.Add(
                new TarotReadingResponse
                {
                    CardName = card.Name,
                    IsReversed = false,
                    ImageUrl = card.ImageUrl,
                }
            );
        }

        return response;
    }

    public async Task<List<TarotReadingResponse>> DrawCardAsync(int numberOfCards)
    {
        var cardIds = await _unitOfWork.TarotCards.ReadOnly().ToListAsync();
        var selectedCards = new List<TarotReadingResponse>();
        var usedIndices = new HashSet<int>();

        for (int i = 0; i < numberOfCards && usedIndices.Count < cardIds.Count; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = _random.Next(cardIds.Count);
            } while (usedIndices.Contains(randomIndex));

            usedIndices.Add(randomIndex);
            var randomId = cardIds[randomIndex];
            var card = await _unitOfWork.TarotCards.ReadOnly().FirstAsync(c => c.Id == randomId.Id);
            bool isReversed = _random.Next(2) == 0;

            selectedCards.Add(
                new TarotReadingResponse
                {
                    CardName = card.Name,
                    IsReversed = isReversed,
                    ImageUrl = card.ImageUrl,
                }
            );
        }

        return selectedCards;
    }
}
