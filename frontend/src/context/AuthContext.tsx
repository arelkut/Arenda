import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import { api } from '../services/api';
import type { TokenResponse } from '../services/api';

interface AuthState {
  token: string | null;
  userId: number | null;
  email: string | null;
  roles: string[];
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (data: { email: string; password: string; first_name?: string; last_name?: string; phone?: string }) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthState | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(localStorage.getItem('token'));
  const [userId, setUserId] = useState<number | null>(null);
  const [email, setEmail] = useState<string | null>(null);
  const [roles, setRoles] = useState<string[]>([]);

  useEffect(() => {
    const stored = localStorage.getItem('token');
    const storedEmail = localStorage.getItem('email');
    const storedRoles = localStorage.getItem('roles');
    if (stored) {
      setToken(stored);
      setEmail(storedEmail);
      setRoles(storedRoles ? JSON.parse(storedRoles) : []);
    }
  }, []);

  const handleAuth = (data: TokenResponse) => {
    setToken(data.access_token);
    setUserId(data.user_id);
    setEmail(data.email);
    setRoles(data.roles);
    localStorage.setItem('token', data.access_token);
    localStorage.setItem('email', data.email);
    localStorage.setItem('roles', JSON.stringify(data.roles));
  };

  const login = async (email: string, password: string) => {
    const data = await api.login(email, password);
    handleAuth(data);
  };

  const register = async (regData: { email: string; password: string; first_name?: string; last_name?: string; phone?: string }) => {
    const data = await api.register(regData);
    handleAuth(data);
  };

  const logout = () => {
    setToken(null);
    setUserId(null);
    setEmail(null);
    setRoles([]);
    localStorage.removeItem('token');
    localStorage.removeItem('email');
    localStorage.removeItem('roles');
  };

  return (
    <AuthContext.Provider value={{ token, userId, email, roles, isAuthenticated: !!token, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be inside AuthProvider');
  return ctx;
}
