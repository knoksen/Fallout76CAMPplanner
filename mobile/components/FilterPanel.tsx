import React from 'react';
import { View, Text, TouchableOpacity, ScrollView, StyleSheet, Switch } from 'react-native';
import { ItemFilters } from '../data/types';
import { Theme } from '../hooks/useTheme';
import { CATEGORIES } from '../data/campItems';

interface FilterPanelProps {
  filters: ItemFilters;
  onFiltersChange: (f: ItemFilters) => void;
  onClose: () => void;
  theme: Theme;
}

export default function FilterPanel({ filters, onFiltersChange, onClose, theme }: FilterPanelProps) {
  return (
    <View style={[styles.overlay, { backgroundColor: 'rgba(0,0,0,0.6)' }]}>
      <View style={[styles.panel, { backgroundColor: theme.surface }]}>
        <View style={styles.header}>
          <Text style={[styles.title, { color: theme.text }]}>Filters</Text>
          <TouchableOpacity onPress={onClose} hitSlop={8}>
            <Text style={[styles.closeBtn, { color: theme.accent }]}>✕</Text>
          </TouchableOpacity>
        </View>

        <Text style={[styles.sectionLabel, { color: theme.textSecondary }]}>Category</Text>
        <ScrollView horizontal showsHorizontalScrollIndicator={false} style={styles.catScroll}>
          {CATEGORIES.map((cat) => (
            <TouchableOpacity
              key={cat}
              onPress={() => onFiltersChange({ ...filters, category: cat })}
              style={[
                styles.catBtn,
                { backgroundColor: filters.category === cat ? theme.accent : theme.border },
              ]}
            >
              <Text style={[styles.catText, { color: filters.category === cat ? '#fff' : theme.textSecondary }]}>
                {cat}
              </Text>
            </TouchableOpacity>
          ))}
        </ScrollView>

        <View style={[styles.switchRow, { borderTopColor: theme.border }]}>
          <Text style={[styles.switchLabel, { color: theme.text }]}>Favorites only</Text>
          <Switch
            value={filters.showFavoritesOnly}
            onValueChange={(v) => onFiltersChange({ ...filters, showFavoritesOnly: v })}
            trackColor={{ false: theme.border, true: theme.accent }}
            thumbColor="#fff"
          />
        </View>

        <View style={[styles.switchRow, { borderTopColor: theme.border }]}>
          <Text style={[styles.switchLabel, { color: theme.text }]}>Placed only</Text>
          <Switch
            value={filters.showPlacedOnly}
            onValueChange={(v) => onFiltersChange({ ...filters, showPlacedOnly: v })}
            trackColor={{ false: theme.border, true: theme.accent }}
            thumbColor="#fff"
          />
        </View>

        <TouchableOpacity
          onPress={() => onFiltersChange({ search: '', category: 'all', showFavoritesOnly: false, showPlacedOnly: false })}
          style={[styles.resetBtn, { borderColor: theme.border }]}
        >
          <Text style={[styles.resetText, { color: theme.textSecondary }]}>Reset filters</Text>
        </TouchableOpacity>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  overlay: { ...StyleSheet.absoluteFillObject, justifyContent: 'center', alignItems: 'center', zIndex: 100 },
  panel: { width: '90%', borderRadius: 14, padding: 18 },
  header: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 14 },
  title: { fontSize: 19, fontWeight: '700' },
  closeBtn: { fontSize: 22, fontWeight: '700' },
  sectionLabel: { fontSize: 12, marginBottom: 8 },
  catScroll: { maxHeight: 44, marginBottom: 14 },
  catBtn: { paddingHorizontal: 14, paddingVertical: 6, borderRadius: 20, marginRight: 6 },
  catText: { fontSize: 12, fontWeight: '600' },
  switchRow: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', paddingVertical: 12, borderTopWidth: 1 },
  switchLabel: { fontSize: 14 },
  resetBtn: { marginTop: 12, borderWidth: 1, borderRadius: 8, padding: 10, alignItems: 'center' },
  resetText: { fontSize: 13 },
});
