import { useState, useMemo } from 'react';
import { CAMPItem, ItemFilters } from '../data/types';

export function useItems() {
  const [items, setItems] = useState<CAMPItem[]>([]);
  const [filters, setFilters] = useState<ItemFilters>({
    search: '',
    category: 'all',
    showFavoritesOnly: false,
    showPlacedOnly: false,
  });

  const filteredItems = useMemo(() => {
    return items.filter((item) => {
      if (filters.showFavoritesOnly && !item.isFavorite) return false;
      if (filters.showPlacedOnly && !item.isPlaced) return false;
      if (filters.category !== 'all' && item.category !== filters.category) return false;
      if (filters.search) {
        const q = filters.search.toLowerCase();
        if (!item.name.toLowerCase().includes(q) && !item.category.toLowerCase().includes(q)) {
          return false;
        }
      }
      return true;
    });
  }, [items, filters]);

  const addItem = (item: CAMPItem) => {
    setItems((prev) => [...prev, { ...item, id: `${Date.now()}-${Math.random()}` }]);
  };

  const removeItem = (id: string) => setItems((prev) => prev.filter((i) => i.id !== id));

  const toggleFavorite = (id: string) =>
    setItems((prev) =>
      prev.map((i) => (i.id === id ? { ...i, isFavorite: !i.isFavorite } : i))
    );

  const togglePlaced = (id: string) =>
    setItems((prev) =>
      prev.map((i) => (i.id === id ? { ...i, isPlaced: !i.isPlaced } : i))
    );

  const updateQuantity = (id: string, quantity: number) =>
    setItems((prev) => prev.map((i) => (i.id === id ? { ...i, quantity } : i)));

  const updateNotes = (id: string, notes: string) =>
    setItems((prev) => prev.map((i) => (i.id === id ? { ...i, notes } : i)));

  const clearAll = () => setItems([]);

  const placedCount = useMemo(() => items.filter((i) => i.isPlaced).length, [items]);

  return {
    items,
    filteredItems,
    filters,
    setFilters,
    addItem,
    removeItem,
    toggleFavorite,
    togglePlaced,
    updateQuantity,
    updateNotes,
    clearAll,
    placedCount,
    totalItems: items.length,
  };
}
