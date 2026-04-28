import React from 'react';
import { View, Text, TextInput, TouchableOpacity, StyleSheet } from 'react-native';
import { Theme } from '../hooks/useTheme';

interface BudgetBarProps {
  budget: number;
  used: number;
  remaining: number;
  percentUsed: number;
  onBudgetChange: (v: number) => void;
  theme: Theme;
}

export default function BudgetBar({
  budget,
  used,
  remaining,
  percentUsed,
  onBudgetChange,
  theme,
}: BudgetBarProps) {
  const [editing, setEditing] = React.useState(false);
  const [input, setInput] = React.useState(budget.toString());

  const submit = () => {
    const n = parseInt(input, 10);
    if (!isNaN(n) && n > 0) {
      onBudgetChange(n);
    } else {
      setInput(budget.toString());
    }
    setEditing(false);
  };

  const barColor =
    percentUsed > 90 ? theme.error : percentUsed > 70 ? theme.warning : theme.success;

  return (
    <View style={styles.container}>
      <View style={styles.row}>
        <Text style={[styles.label, { color: theme.textSecondary }]}>Budget:</Text>
        {editing ? (
          <TextInput
            style={[styles.input, { color: theme.text, borderColor: theme.border }]}
            value={input}
            onChangeText={setInput}
            onBlur={submit}
            onSubmitEditing={submit}
            keyboardType="numeric"
            autoFocus
          />
        ) : (
          <TouchableOpacity onPress={() => { setInput(budget.toString()); setEditing(true); }}>
            <Text style={[styles.value, { color: theme.accent }]}>{budget}</Text>
          </TouchableOpacity>
        )}
        <Text style={[styles.label, { color: theme.textSecondary }]}>Used: <Text style={{ color: theme.text }}>{used}</Text></Text>
        <Text style={[styles.label, { color: remaining < 0 ? theme.error : theme.success }]}>
          Left: {remaining}
        </Text>
      </View>
      <View style={[styles.track, { backgroundColor: theme.border }]}>
        <View style={[styles.fill, { width: `${percentUsed}%` as any, backgroundColor: barColor }]} />
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, paddingHorizontal: 12, paddingVertical: 10 },
  row: { flexDirection: 'row', alignItems: 'center', gap: 10, marginBottom: 6, flexWrap: 'wrap' },
  label: { fontSize: 12, fontWeight: '500' },
  value: { fontSize: 15, fontWeight: '700' },
  input: { fontSize: 15, fontWeight: '700', borderWidth: 1, borderRadius: 4, paddingHorizontal: 6, paddingVertical: 1, minWidth: 60 },
  track: { height: 5, borderRadius: 3, overflow: 'hidden' },
  fill: { height: '100%', borderRadius: 3 },
});
