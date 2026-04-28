import React, { useState } from 'react';
import {
  Modal,
  View,
  Text,
  TextInput,
  TouchableOpacity,
  FlatList,
  StyleSheet,
} from 'react-native';
import { CAMPItem } from '../data/types';
import { Theme } from '../hooks/useTheme';
import { CAMP_ITEMS, CATEGORIES } from '../data/campItems';

interface AddItemModalProps {
  onAdd: (item: CAMPItem) => void;
  onClose: () => void;
  theme: Theme;
}

export default function AddItemModal({ onAdd, onClose, theme }: AddItemModalProps) {
  const [search, setSearch] = useState('');
  const [category, setCategory] = useState('all');

  const filtered = CAMP_ITEMS.filter((item) => {
    if (category !== 'all' && item.category !== category) return false;
    if (search) {
      const q = search.toLowerCase();
      return item.name.toLowerCase().includes(q) || item.category.toLowerCase().includes(q);
    }
    return true;
  });

  return (
    <Modal visible animationType="slide" onRequestClose={onClose}>
      <View style={[styles.container, { backgroundColor: theme.background }]}>
        <View style={[styles.header, { backgroundColor: theme.surface, borderBottomColor: theme.border }]}>
          <Text style={[styles.title, { color: theme.text }]}>Add Item</Text>
          <TouchableOpacity onPress={onClose} hitSlop={8}>
            <Text style={[styles.closeBtn, { color: theme.accent }]}>✕</Text>
          </TouchableOpacity>
        </View>

        <TextInput
          style={[styles.searchInput, { backgroundColor: theme.surface, color: theme.text, borderColor: theme.border }]}
          placeholder="Search items..."
          placeholderTextColor={theme.textSecondary}
          value={search}
          onChangeText={setSearch}
        />

        <FlatList
          data={CATEGORIES}
          keyExtractor={(c) => c}
          horizontal
          showsHorizontalScrollIndicator={false}
          style={styles.catList}
          renderItem={({ item: cat }) => (
            <TouchableOpacity
              onPress={() => setCategory(cat)}
              style={[
                styles.catBtn,
                { backgroundColor: category === cat ? theme.accent : theme.surface, borderColor: theme.border },
              ]}
            >
              <Text style={[styles.catBtnText, { color: category === cat ? '#fff' : theme.textSecondary }]}>
                {cat}
              </Text>
            </TouchableOpacity>
          )}
        />

        <FlatList
          data={filtered}
          keyExtractor={(item) => item.id}
          renderItem={({ item }) => (
            <TouchableOpacity
              style={[styles.itemRow, { borderBottomColor: theme.border }]}
              onPress={() => { onAdd(item); onClose(); }}
            >
              <View style={styles.itemInfo}>
                <Text style={[styles.itemName, { color: theme.text }]}>{item.name}</Text>
                <Text style={[styles.itemMeta, { color: theme.textSecondary }]}>
                  {item.category} · Budget cost: {item.cost}
                </Text>
              </View>
              <Text style={[styles.addBtn, { color: theme.accent }]}>＋</Text>
            </TouchableOpacity>
          )}
          ListEmptyComponent={
            <Text style={[styles.noResults, { color: theme.textSecondary }]}>No items match.</Text>
          }
        />
      </View>
    </Modal>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1 },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: 16,
    borderBottomWidth: 1,
  },
  title: { fontSize: 19, fontWeight: '700' },
  closeBtn: { fontSize: 22, fontWeight: '700' },
  searchInput: {
    margin: 12,
    padding: 10,
    borderRadius: 8,
    borderWidth: 1,
    fontSize: 14,
  },
  catList: { maxHeight: 44, paddingHorizontal: 10, marginBottom: 4 },
  catBtn: {
    paddingHorizontal: 14,
    paddingVertical: 6,
    borderRadius: 20,
    marginRight: 6,
    borderWidth: 1,
  },
  catBtnText: { fontSize: 12, fontWeight: '600' },
  itemRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingHorizontal: 16,
    paddingVertical: 14,
    borderBottomWidth: 1,
  },
  itemInfo: { flex: 1 },
  itemName: { fontSize: 14, fontWeight: '600' },
  itemMeta: { fontSize: 11, marginTop: 2 },
  addBtn: { fontSize: 24, fontWeight: '700' },
  noResults: { textAlign: 'center', padding: 24 },
});
