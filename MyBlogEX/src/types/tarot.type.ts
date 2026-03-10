export interface GuidedTarotRequest {
  numberOfCards: number;
  spreadType: string;
  time: string;
  language: string;
}

export interface CustomTarotRequest {
  question: string;
  language: string;
}

export interface TarotReadingResponse {
  cardName: string;
  isReversed: boolean;
  imageUrl: string;
}
