export interface TarotReadingRequest {
  numberOfCards: number | null;
  spreadType: string | null;
  time: string | null;
  language: string;
  question: string | null;
}

export interface TarotReadingResponse {
  cardName: string;
  isReversed: boolean;
  imageUrl: string;
}
