import { useState, useMemo } from 'react';
import { CAMPItem } from '../data/types';

const DEFAULT_BUDGET = 400;

export function useBudget(items: CAMPItem[]) {
  const [budget, setBudget] = useState(DEFAULT_BUDGET);

  const used = useMemo(
    () => items.reduce((sum, item) => sum + item.cost * item.quantity, 0),
    [items]
  );

  const remaining = useMemo(() => budget - used, [budget, used]);

  const percentUsed = useMemo(
    () => (budget > 0 ? Math.min((used / budget) * 100, 100) : 0),
    [used, budget]
  );

  return { budget, setBudget, used, remaining, percentUsed };
}
