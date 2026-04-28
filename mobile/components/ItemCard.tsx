import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';
import { CAMPItem } from '../data/types';
import { Theme } from '../hooks/useTheme';

interface ItemCardProps {
  item: CAMPItem;
  onRemove: (id: string) => void;
  onToggleFavorite: (id: string) => void;
  onTogglePlaced: (id: string) => void;
  onUpdateQuantity: (id: string, quantity: number) => void;
  theme: Theme;
}

export default function ItemCard({
  item,
  onRemove,
  onToggleFavorite,
  onTogglePlaced,
  onUpdateQuantity,
  theme,
}: ItemCardProps) {
  const totalCost = item.cost * item.quantity;

  return (
    <View
      style={[
        styles.card,
        {
          backgroundColor: theme.surface,
          borderColor: item.isFavorite ? theme.accent : theme.border,
        },
      ]}
    >
      <View style={styles.row}>
        <TouchableOpacity onPress={() => onToggleFavorite(item.id)} hitSlop={8}>
          <Text style={[styles.star, { color: item.isFavorite ? theme.accent : theme.border }]}>
            {item.isFavorite ? '★' : '☆'}
          </Text>
        </TouchableOpacity>

        <View style={styles.info}>
          <Text style={[styles.name, { color: theme.text }]}>{item.name}</Text>
          <Text style={[styles.meta, { color: theme.textSecondary }]}>
            {item.category} · {item.cost}/ea · Total: {totalCost}
          </Text>
        </View>

        <TouchableOpacity
          onPress={() => onTogglePlaced(item.id)}
          style={[styles.placedBadge, { borderColor: item.isPlaced ? theme.success : theme.border }]}
          hitSlop={8}
        >
          <Text style={{ color: item.isPlaced ? theme.success : theme.textSecondary, fontSize: 11, fontWeight: '600' }}>
            {item.isPlaced ? 'PLACED' : 'PLAN'}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity onPress={() => onRemove(item.id)} hitSlop={8}>
          <Text style={[styles.remove, { color: theme.error }]}>✕</Text>
        </TouchableOpacity>
      </View>

      <View style={styles.qtyRow}>
        <TouchableOpacity
          onPress={() => onUpdateQuantity(item.id, Math.max(1, item.quantity - 1))}
          style={[styles.qtyBtn, { borderColor: theme.border }]}
        >
          <Text style={[styles.qtyBtnText, { color: theme.text }]}>−</Text>
        </TouchableOpacity>
        <Text style={[styles.qty, { color: theme.text }]}>{item.quantity}</Text>
        <TouchableOpacity
          onPress={() => onUpdateQuantity(item.id, item.quantity + 1)}
          style={[styles.qtyBtn, { borderColor: theme.border }]}
        >
          <Text style={[styles.qtyBtnText, { color: theme.text }]}>+</Text>
        </TouchableOpacity>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  card: { borderRadius: 10, borderWidth: 1.5, marginHorizontal: 8, marginVertical: 4, padding: 12 },
  row: { flexDirection: 'row', alignItems: 'center', gap: 8 },
  info: { flex: 1 },
  name: { fontSize: 14, fontWeight: '600' },
  meta: { fontSize: 11, marginTop: 2 },
  star: { fontSize: 20 },
  remove: { fontSize: 17, fontWeight: '700' },
  placedBadge: { borderWidth: 1, borderRadius: 4, paddingHorizontal: 6, paddingVertical: 2 },
  qtyRow: { flexDirection: 'row', alignItems: 'center', marginTop: 8, gap: 8 },
  qtyBtn: { borderWidth: 1, borderRadius: 4, paddingHorizontal: 10, paddingVertical: 2 },
  qtyBtnText: { fontSize: 16, fontWeight: '700' },
  qty: { fontSize: 14, fontWeight: '600', minWidth: 24, textAlign: 'center' },
});
