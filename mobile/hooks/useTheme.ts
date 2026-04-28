import { useState } from 'react';

export interface Theme {
  background: string;
  surface: string;
  text: string;
  textSecondary: string;
  border: string;
  accent: string;
  error: string;
  warning: string;
  success: string;
}

const lightTheme: Theme = {
  background: '#f5f5f5',
  surface: '#ffffff',
  text: '#1a1a1a',
  textSecondary: '#666666',
  border: '#e0e0e0',
  accent: '#c8a951',
  error: '#d32f2f',
  warning: '#f57c00',
  success: '#388e3c',
};

const darkTheme: Theme = {
  background: '#12161c',
  surface: '#1b2129',
  text: '#ebeff4',
  textSecondary: '#aab4c0',
  border: '#3a4452',
  accent: '#f6bc39',
  error: '#ff6b6b',
  warning: '#ffb74d',
  success: '#66bb6a',
};

export function useTheme() {
  const [isDark, setIsDark] = useState(true);
  const toggleTheme = () => setIsDark((prev) => !prev);
  return { theme: isDark ? darkTheme : lightTheme, isDark, toggleTheme };
}
