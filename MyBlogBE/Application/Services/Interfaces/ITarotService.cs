using Application.Dtos;

namespace Application.Services.Interfaces;

public interface ITarotService
{
    Task<List<TarotReadingResponse>> GetCardsAsync();
    Task<List<TarotReadingResponse>> DrawCardAsync(int numberOfCards);
}
