const API_BASE = import.meta.env.VITE_API_URL || 'http://localhost:8000/api';

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options?.headers,
    },
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({ detail: res.statusText }));
    throw new Error(err.detail || 'Request failed');
  }
  return res.json();
}

export interface PropertyMedia {
  media_id: number;
  media_type: string;
  file_path: string;
  is_main: boolean;
}

export interface Property {
  property_id: number;
  landlord_id: number;
  title: string;
  description: string | null;
  address: string;
  city: string;
  district: string | null;
  property_type: string;
  area: string;
  rooms: number | null;
  floor: number | null;
  total_floors: number | null;
  price: string;
  is_active: boolean;
  created_date: string | null;
  status_id: number;
  media: PropertyMedia[];
}

export interface PropertyList {
  items: Property[];
  total: number;
  page: number;
  size: number;
}

export interface TokenResponse {
  access_token: string;
  token_type: string;
  user_id: number;
  email: string;
  roles: string[];
}

export interface UserInfo {
  user_id: number;
  email: string;
  phone: string | null;
  registration_date: string | null;
  is_active: boolean;
  roles: string[];
}

export const api = {
  async getProperties(params?: Record<string, string>): Promise<PropertyList> {
    const query = params ? '?' + new URLSearchParams(params).toString() : '';
    return request<PropertyList>(`/properties${query}`);
  },

  async getProperty(id: number): Promise<Property> {
    return request<Property>(`/properties/${id}`);
  },

  async login(email: string, password: string): Promise<TokenResponse> {
    return request<TokenResponse>('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    });
  },

  async register(data: {
    email: string;
    password: string;
    first_name?: string;
    last_name?: string;
    phone?: string;
    role?: string;
  }): Promise<TokenResponse> {
    return request<TokenResponse>('/auth/register', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },

  async getMe(token: string): Promise<UserInfo> {
    return request<UserInfo>(`/auth/me?token=${token}`);
  },
};
