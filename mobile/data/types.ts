export interface CAMPItem {
  id: string;
  name: string;
  category: string;
  cost: number;
  quantity: number;
  isFavorite: boolean;
  isPlaced: boolean;
  notes?: string;
}

export interface ItemFilters {
  search: string;
  category: string;
  showFavoritesOnly: boolean;
  showPlacedOnly: boolean;
}

export interface SavedPlan {
  id: string;
  name: string;
  items: CAMPItem[];
  budget: number;
  createdAt: number;
  updatedAt: number;
}

/**
 * Portable cross-platform plan format.
 * Can be imported by both the mobile app and the desktop CAMP Planner.
 * schemaVersion must be incremented on breaking changes.
 */
export interface PortablePlan {
  schemaVersion: '1';
  schemaType: 'fo76camp-portable-plan';
  name: string;
  budget: number;
  exportedAt: string; // ISO 8601
  source: 'mobile' | 'desktop';
  items: PortablePlanItem[];
}

export interface PortablePlanItem {
  id: string;
  name: string;
  category: string;
  cost: number;
  quantity: number;
  notes?: string;
}

/** Convert a SavedPlan to the portable cross-platform format. */
export function toPortablePlan(plan: SavedPlan): PortablePlan {
  return {
    schemaVersion: '1',
    schemaType: 'fo76camp-portable-plan',
    name: plan.name,
    budget: plan.budget,
    exportedAt: new Date().toISOString(),
    source: 'mobile',
    items: plan.items
      .filter((i) => i.quantity > 0)
      .map((i) => ({
        id: i.id,
        name: i.name,
        category: i.category,
        cost: i.cost,
        quantity: i.quantity,
        notes: i.notes,
      })),
  };
}

/** Convert a portable plan back into a SavedPlan (for mobile import). */
export function fromPortablePlan(portable: PortablePlan): SavedPlan {
  return {
    id: Date.now().toString(),
    name: portable.name,
    budget: portable.budget,
    createdAt: Date.now(),
    updatedAt: Date.now(),
    items: portable.items.map((i) => ({
      id: i.id,
      name: i.name,
      category: i.category,
      cost: i.cost,
      quantity: i.quantity,
      isFavorite: false,
      isPlaced: false,
      notes: i.notes,
    })),
  };
}
