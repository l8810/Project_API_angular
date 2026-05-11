export interface Gift {
  id: number;
  name: string;
  description: string;
  donorId: number;  // שונה מ-donorName
  donorName?: string;  // רק להצגה
  price: number;
  categoryId: number;  // שונה מ-categoryName
  categoryName?: string;  // רק להצגה
  picture: string;
  winnerId?: number;
  winnerName?: string; 
}