import React from 'react';
import { View, Text, TouchableOpacity, ScrollView, StyleSheet } from 'react-native';
import { CAMPItem } from '../data/types';
import { Theme } from '../hooks/useTheme';

interface SummaryPanelProps {
  items: CAMPItem[];
  budget: number;
  used: number;
  remaining: number;
  placedCount: number;
  totalItems: number;
  onClose: () => void;
  theme: Theme;
}

export default function SummaryPanel({
  items,
  budget,
  used,
  remaining,
  placedCount,
  totalItems,
  onClose,
  theme,
}: SummaryPanelProps) {
  const byCategory = items.reduce(
    (acc, item) => {
      if (!acc[item.category]) acc[item.category] = { count: 0, cost: 0 };
      acc[item.category].count += item.quantity;
      acc[item.category].cost += item.cost * item.quantity;
      return acc;
    },
    {} as Record<string, { count: number; cost: number }>
  );

  const percentUsed = budget > 0 ? Math.round((used / budget) * 100) : 0;

  return (
    <View style={[styles.overlay, { backgroundColor: 'rgba(0,0,0,0.6)' }]}>
      <View style={[styles.panel, { backgroundColor: theme.surface }]}>
        <View style={styles.header}>
          <Text style={[styles.title, { color: theme.text }]}>Plan Summary</Text>
          <TouchableOpacity onPress={onClose} hitSlop={8}>
            <Text style={[styles.closeBtn, { color: theme.accent }]}>✕</Text>
          </TouchableOpacity>
        </View>

        <ScrollView showsVerticalScrollIndicator={false}>
          <View style={styles.statsGrid}>
            <StatBox label="Budget" value={budget} color={theme.text} theme={theme} />
            <StatBox label="Used" value={used} color={theme.text} theme={theme} />
            <StatBox label="Remaining" value={remaining} color={remaining < 0 ? theme.error : theme.success} theme={theme} />
            <StatBox label="% Used" value={`${percentUsed}%`} color={percentUsed > 90 ? theme.error : theme.text} theme={theme} />
            <StatBox label="Items" value={totalItems} color={theme.text} theme={theme} />
            <StatBox label="Placed" value={placedCount} color={theme.success} theme={theme} />
          </View>

          <Text style={[styles.sectionTitle, { color: theme.text, borderBottomColor: theme.border }]}>
            By Category
          </Text>

          {Object.entries(byCategory).sort((a, b) => b[1].cost - a[1].cost).map(([cat, data]) => (
            <View key={cat} style={[styles.catRow, { borderBottomColor: theme.border }]}>
              <Text style={[styles.catName, { color: theme.text }]}>{cat}</Text>
              <Text style={[styles.catData, { color: theme.textSecondary }]}>
                ×{data.count} · cost {data.cost}
              </Text>
            </View>
          ))}

          {Object.keys(byCategory).length === 0 && (
            <Text style={[styles.noItems, { color: theme.textSecondary }]}>No items in plan.</Text>
          )}
        </ScrollView>
      </View>
    </View>
  );
}

function StatBox({ label, value, color, theme }: { label: string; value: number | string; color: string; theme: Theme }) {
  return (
    <View style={[styles.statBox, { backgroundColor: theme.background }]}>
      <Text style={[styles.statLabel, { color: theme.textSecondary }]}>{label}</Text>
      <Text style={[styles.statValue, { color }]}>{value}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  overlay: { ...StyleSheet.absoluteFillObject, justifyContent: 'center', alignItems: 'center', zIndex: 100 },
  panel: { width: '92%', maxHeight: '82%', borderRadius: 14, padding: 18 },
  header: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 14 },
  title: { fontSize: 19, fontWeight: '700' },
  closeBtn: { fontSize: 22, fontWeight: '700' },
  statsGrid: { flexDirection: 'row', flexWrap: 'wrap', gap: 8, marginBottom: 16 },
  statBox: { flex: 1, minWidth: '28%', borderRadius: 8, padding: 10, alignItems: 'center' },
  statLabel: { fontSize: 11 },
  statValue: { fontSize: 18, fontWeight: '700', marginTop: 2 },
  sectionTitle: { fontSize: 14, fontWeight: '700', borderBottomWidth: 1, paddingBottom: 6, marginBottom: 6 },
  catRow: { flexDirection: 'row', justifyContent: 'space-between', paddingVertical: 8, borderBottomWidth: 1 },
  catName: { fontSize: 13, fontWeight: '600' },
  catData: { fontSize: 12 },
  noItems: { textAlign: 'center', padding: 16 },
});
