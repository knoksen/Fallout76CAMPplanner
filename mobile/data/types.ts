export interface CAMPItem {
  id: string;
  name: string;
  category: string;
  cost: number;
  quantity: number;
  isFavorite: boolean;
  isPlaced: boolean;
  notes?: string;
}

export interface ItemFilters {
  search: string;
  category: string;
  showFavoritesOnly: boolean;
  showPlacedOnly: boolean;
}

export interface SavedPlan {
  id: string;
  name: string;
  items: CAMPItem[];
  budget: number;
  createdAt: number;
  updatedAt: number;
}
