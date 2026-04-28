import React from 'react';
import { FlatList, View, Text, StyleSheet } from 'react-native';
import { CAMPItem } from '../data/types';
import { Theme } from '../hooks/useTheme';
import ItemCard from './ItemCard';

interface ItemListProps {
  items: CAMPItem[];
  onRemove: (id: string) => void;
  onToggleFavorite: (id: string) => void;
  onTogglePlaced: (id: string) => void;
  onUpdateQuantity: (id: string, quantity: number) => void;
  theme: Theme;
}

export default function ItemList({
  items,
  onRemove,
  onToggleFavorite,
  onTogglePlaced,
  onUpdateQuantity,
  theme,
}: ItemListProps) {
  if (items.length === 0) {
    return (
      <View style={styles.empty}>
        <Text style={[styles.emptyTitle, { color: theme.textSecondary }]}>No items yet</Text>
        <Text style={[styles.emptyHint, { color: theme.textSecondary }]}>
          Tap ＋ Add to start building your plan
        </Text>
      </View>
    );
  }

  return (
    <FlatList
      data={items}
      keyExtractor={(item) => item.id}
      renderItem={({ item }) => (
        <ItemCard
          item={item}
          onRemove={onRemove}
          onToggleFavorite={onToggleFavorite}
          onTogglePlaced={onTogglePlaced}
          onUpdateQuantity={onUpdateQuantity}
          theme={theme}
        />
      )}
      contentContainerStyle={styles.list}
    />
  );
}

const styles = StyleSheet.create({
  empty: { flex: 1, justifyContent: 'center', alignItems: 'center', padding: 40 },
  emptyTitle: { fontSize: 17, fontWeight: '600', marginBottom: 6 },
  emptyHint: { fontSize: 13, textAlign: 'center' },
  list: { paddingVertical: 8 },
});
