import React, { useState, useEffect } from 'react';
import {
  View, Text, TextInput, TouchableOpacity, FlatList,
  StyleSheet, Alert, ActivityIndicator, Share,
} from 'react-native';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { CAMPItem, SavedPlan, toPortablePlan, fromPortablePlan, PortablePlan } from '../data/types';
import { Theme } from '../hooks/useTheme';

interface LoadSavePanelProps {
  items: CAMPItem[];
  budget: number;
  onLoad: (items: CAMPItem[], budget: number) => void;
  onClose: () => void;
  theme: Theme;
}

const STORAGE_KEY = 'fo76_camp_plans_v1';

export default function LoadSavePanel({ items, budget, onLoad, onClose, theme }: LoadSavePanelProps) {
  const [plans, setPlans] = useState<SavedPlan[]>([]);
  const [planName, setPlanName] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    (async () => {
      try {
        const raw = await AsyncStorage.getItem(STORAGE_KEY);
        setPlans(raw ? (JSON.parse(raw) as SavedPlan[]) : []);
      } catch {
        Alert.alert('Error', 'Could not load saved plans.');
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  const savePlan = async () => {
    const name = planName.trim();
    if (!name) {
      Alert.alert('Name required', 'Enter a name for your plan.');
      return;
    }
    const plan: SavedPlan = {
      id: Date.now().toString(),
      name,
      items,
      budget,
      createdAt: Date.now(),
      updatedAt: Date.now(),
    };
    const updated = [...plans, plan];
    try {
      await AsyncStorage.setItem(STORAGE_KEY, JSON.stringify(updated));
      setPlans(updated);
      setPlanName('');
      Alert.alert('Saved', `"${name}" saved.`);
    } catch {
      Alert.alert('Error', 'Could not save plan.');
    }
  };

  const deletePlan = (id: string) => {
    Alert.alert('Delete plan?', 'This cannot be undone.', [
      { text: 'Cancel', style: 'cancel' },
      {
        text: 'Delete',
        style: 'destructive',
        onPress: async () => {
          const updated = plans.filter((p) => p.id !== id);
          try {
            await AsyncStorage.setItem(STORAGE_KEY, JSON.stringify(updated));
            setPlans(updated);
          } catch {
            Alert.alert('Error', 'Could not delete plan.');
          }
        },
      },
    ]);
  };

  /** Share a plan as portable JSON so the desktop app (or another device) can import it. */
  const sharePlan = async (plan: SavedPlan) => {
    try {
      const portable = toPortablePlan(plan);
      await Share.share({
        title: `FO76 CAMP Plan – ${plan.name}`,
        message: JSON.stringify(portable, null, 2),
      });
    } catch {
      Alert.alert('Error', 'Could not share plan.');
    }
  };

  /** Paste a portable JSON string (from clipboard or another source) and save it as a plan. */
  const importPortable = () => {
    Alert.prompt(
      'Import portable plan',
      'Paste the portable JSON exported from the desktop CAMP Planner or another device:',
      async (text) => {
        if (!text?.trim()) return;
        try {
          const portable = JSON.parse(text.trim()) as PortablePlan;
          if (portable.schemaType !== 'fo76camp-portable-plan') {
            Alert.alert('Invalid format', 'This does not look like a FO76 CAMP portable plan.');
            return;
          }
          const imported = fromPortablePlan(portable);
          const updated = [...plans, imported];
          await AsyncStorage.setItem(STORAGE_KEY, JSON.stringify(updated));
          setPlans(updated);
          Alert.alert('Imported', `"${imported.name}" has been added to your plans.`);
        } catch {
          Alert.alert('Error', 'Could not parse the JSON. Make sure you copied the full export.');
        }
      },
      'plain-text',
    );
  };

  return (
    <View style={[styles.overlay, { backgroundColor: 'rgba(0,0,0,0.6)' }]}>
      <View style={[styles.panel, { backgroundColor: theme.surface }]}>
        <View style={styles.header}>
          <Text style={[styles.title, { color: theme.text }]}>Load / Save</Text>
          <View style={styles.headerRight}>
            <TouchableOpacity onPress={importPortable} style={[styles.importBtn, { borderColor: theme.accent }]} hitSlop={8}>
              <Text style={[styles.importBtnText, { color: theme.accent }]}>↓ Import</Text>
            </TouchableOpacity>
            <TouchableOpacity onPress={onClose} hitSlop={8}>
              <Text style={[styles.closeBtn, { color: theme.accent }]}>✕</Text>
            </TouchableOpacity>
          </View>
        </View>

        <View style={styles.saveRow}>
          <TextInput
            style={[styles.nameInput, { backgroundColor: theme.background, color: theme.text, borderColor: theme.border }]}
            placeholder="Plan name..."
            placeholderTextColor={theme.textSecondary}
            value={planName}
            onChangeText={setPlanName}
          />
          <TouchableOpacity onPress={savePlan} style={[styles.saveBtn, { backgroundColor: theme.accent }]}>
            <Text style={styles.saveBtnText}>Save</Text>
          </TouchableOpacity>
        </View>

        {loading ? (
          <ActivityIndicator color={theme.accent} style={styles.loader} />
        ) : (
          <FlatList
            data={plans}
            keyExtractor={(p) => p.id}
            renderItem={({ item: plan }) => (
              <View style={[styles.planRow, { borderBottomColor: theme.border }]}>
                <View style={styles.planInfo}>
                  <Text style={[styles.planName, { color: theme.text }]}>{plan.name}</Text>
                  <Text style={[styles.planMeta, { color: theme.textSecondary }]}>
                    {plan.items.length} items · Budget {plan.budget}
                  </Text>
                </View>
                <TouchableOpacity
                  onPress={() => onLoad(plan.items, plan.budget)}
                  style={[styles.actionBtn, { borderColor: theme.accent }]}
                >
                  <Text style={[styles.actionBtnText, { color: theme.accent }]}>Load</Text>
                </TouchableOpacity>
                <TouchableOpacity
                  onPress={() => sharePlan(plan)}
                  style={[styles.actionBtn, { borderColor: theme.accent }]}
                >
                  <Text style={[styles.actionBtnText, { color: theme.accent }]}>Share</Text>
                </TouchableOpacity>
                <TouchableOpacity
                  onPress={() => deletePlan(plan.id)}
                  style={[styles.actionBtn, { borderColor: theme.error }]}
                >
                  <Text style={[styles.actionBtnText, { color: theme.error }]}>Del</Text>
                </TouchableOpacity>
              </View>
            )}
            ListEmptyComponent={
              <Text style={[styles.empty, { color: theme.textSecondary }]}>No saved plans.</Text>
            }
          />
        )}
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  overlay: { ...StyleSheet.absoluteFillObject, justifyContent: 'center', alignItems: 'center', zIndex: 100 },
  panel: { width: '92%', maxHeight: '80%', borderRadius: 14, padding: 18 },
  header: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 14 },
  headerRight: { flexDirection: 'row', alignItems: 'center', gap: 12 },
  title: { fontSize: 19, fontWeight: '700' },
  closeBtn: { fontSize: 22, fontWeight: '700' },
  importBtn: { borderWidth: 1, borderRadius: 6, paddingHorizontal: 10, paddingVertical: 4 },
  importBtnText: { fontSize: 12, fontWeight: '600' },
  saveRow: { flexDirection: 'row', gap: 8, marginBottom: 12 },
  nameInput: { flex: 1, padding: 9, borderRadius: 8, borderWidth: 1, fontSize: 13 },
  saveBtn: { paddingHorizontal: 16, paddingVertical: 9, borderRadius: 8, justifyContent: 'center' },
  saveBtnText: { color: '#fff', fontWeight: '700', fontSize: 13 },
  loader: { margin: 20 },
  planRow: { flexDirection: 'row', alignItems: 'center', paddingVertical: 10, borderBottomWidth: 1, gap: 8 },
  planInfo: { flex: 1 },
  planName: { fontSize: 13, fontWeight: '600' },
  planMeta: { fontSize: 11, marginTop: 1 },
  actionBtn: { borderWidth: 1, borderRadius: 6, paddingHorizontal: 10, paddingVertical: 4 },
  actionBtnText: { fontSize: 12, fontWeight: '600' },
  empty: { textAlign: 'center', padding: 18 },
});
