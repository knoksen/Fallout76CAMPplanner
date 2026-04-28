import React, { useState } from 'react';
import {
  View, Text, TextInput, TouchableOpacity, StyleSheet, Platform, StatusBar,
} from 'react-native';
import { useTheme } from '../hooks/useTheme';
import { useBudget } from '../hooks/useBudget';
import { useItems } from '../hooks/useItems';
import BudgetBar from './BudgetBar';
import ItemList from './ItemList';
import FilterPanel from './FilterPanel';
import SummaryPanel from './SummaryPanel';
import AddItemModal from './AddItemModal';
import LoadSavePanel from './LoadSavePanel';

export default function CAMPPlannerApp() {
  const { theme, isDark, toggleTheme } = useTheme();
  const {
    items, filteredItems, filters, setFilters,
    addItem, removeItem, toggleFavorite, togglePlaced,
    updateQuantity, clearAll, placedCount, totalItems,
  } = useItems();
  const { budget, setBudget, used, remaining, percentUsed } = useBudget(items);

  const [showAdd, setShowAdd] = useState(false);
  const [showFilters, setShowFilters] = useState(false);
  const [showSummary, setShowSummary] = useState(false);
  const [showPlans, setShowPlans] = useState(false);
  const [searchText, setSearchText] = useState('');

  const handleSearchChange = (text: string) => {
    setSearchText(text);
    setFilters({ ...filters, search: text });
  };

  return (
    <View
      style={[
        styles.root,
        { backgroundColor: theme.background },
        Platform.OS === 'android' && { paddingTop: StatusBar.currentHeight ?? 0 },
      ]}
    >
      <StatusBar barStyle={isDark ? 'light-content' : 'dark-content'} backgroundColor={theme.surface} />

      {/* Top bar */}
      <View style={[styles.topBar, { backgroundColor: theme.surface, borderBottomColor: theme.border }]}>
        <BudgetBar
          budget={budget}
          used={used}
          remaining={remaining}
          percentUsed={percentUsed}
          onBudgetChange={setBudget}
          theme={theme}
        />
        <TouchableOpacity onPress={toggleTheme} style={[styles.themeBtn, { borderColor: theme.border }]} hitSlop={6}>
          <Text style={styles.themeIcon}>{isDark ? '☀️' : '🌙'}</Text>
        </TouchableOpacity>
      </View>

      {/* Search bar */}
      <View style={[styles.searchBar, { backgroundColor: theme.surface, borderBottomColor: theme.border }]}>
        <TextInput
          style={[styles.searchInput, { backgroundColor: theme.background, color: theme.text, borderColor: theme.border }]}
          placeholder="Search your plan..."
          placeholderTextColor={theme.textSecondary}
          value={searchText}
          onChangeText={handleSearchChange}
        />
      </View>

      {/* Item list */}
      <ItemList
        items={filteredItems}
        onRemove={removeItem}
        onToggleFavorite={toggleFavorite}
        onTogglePlaced={togglePlaced}
        onUpdateQuantity={updateQuantity}
        theme={theme}
      />

      {/* Bottom action bar */}
      <View style={[styles.actionBar, { backgroundColor: theme.surface, borderTopColor: theme.border }]}>
        <ActionBtn label="＋ Add" color={theme.accent} onPress={() => setShowAdd(true)} />
        <ActionBtn label="⚙ Filter" color={theme.textSecondary} onPress={() => setShowFilters(true)} />
        <ActionBtn label="📊 Summary" color={theme.textSecondary} onPress={() => setShowSummary(true)} />
        <ActionBtn label="💾 Plans" color={theme.textSecondary} onPress={() => setShowPlans(true)} />
        {items.length > 0 && (
          <ActionBtn label="🗑 Clear" color={theme.error} onPress={clearAll} />
        )}
      </View>

      {/* Overlays */}
      {showFilters && (
        <FilterPanel filters={filters} onFiltersChange={setFilters} onClose={() => setShowFilters(false)} theme={theme} />
      )}
      {showSummary && (
        <SummaryPanel
          items={items} budget={budget} used={used} remaining={remaining}
          placedCount={placedCount} totalItems={totalItems}
          onClose={() => setShowSummary(false)} theme={theme}
        />
      )}
      {showPlans && (
        <LoadSavePanel
          items={items}
          budget={budget}
          onLoad={(loadedItems, loadedBudget) => {
            clearAll();
            loadedItems.forEach(addItem);
            setBudget(loadedBudget);
            setShowPlans(false);
          }}
          onClose={() => setShowPlans(false)}
          theme={theme}
        />
      )}
      {showAdd && (
        <AddItemModal onAdd={addItem} onClose={() => setShowAdd(false)} theme={theme} />
      )}
    </View>
  );
}

function ActionBtn({ label, color, onPress }: { label: string; color: string; onPress: () => void }) {
  return (
    <TouchableOpacity onPress={onPress} style={styles.actionBtn}>
      <Text style={[styles.actionBtnText, { color }]}>{label}</Text>
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  root: { flex: 1 },
  topBar: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingRight: 8,
    borderBottomWidth: 1,
    elevation: 3,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.15,
    shadowRadius: 3,
  },
  themeBtn: { borderWidth: 1, borderRadius: 20, padding: 6 },
  themeIcon: { fontSize: 18 },
  searchBar: { paddingHorizontal: 12, paddingVertical: 8, borderBottomWidth: 1 },
  searchInput: { borderRadius: 8, borderWidth: 1, paddingHorizontal: 12, paddingVertical: 7, fontSize: 13 },
  actionBar: {
    flexDirection: 'row',
    justifyContent: 'space-around',
    borderTopWidth: 1,
    paddingVertical: 8,
    paddingHorizontal: 4,
  },
  actionBtn: { paddingHorizontal: 8, paddingVertical: 6 },
  actionBtnText: { fontSize: 12, fontWeight: '700' },
});
