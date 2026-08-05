import { getVisitorMetadata } from '../utils/visitor';

const API_BASE = (import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000').replace(/\/$/, '');

function getAuthHeaders() {
  const token = localStorage.getItem('admin_token');
  return token ? { Authorization: `Bearer ${token}` } : {};
}

async function request(path, options = {}) {
  const url = `${API_BASE}${path}`;
  const response = await fetch(url, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...getAuthHeaders(),
      ...options.headers,
    },
  });

  if (!response.ok) {
    let message = `Request failed (${response.status})`;
    try {
      const body = await response.json();
      message = body.message || body.title || message;
    } catch {
      /* ignore parse errors */
    }
    throw new Error(message);
  }

  if (response.status === 204) return null;

  const contentType = response.headers.get('content-type') || '';
  if (contentType.includes('application/json')) {
    return response.json();
  }
  return response.text();
}

export async function submitContact(formData, deviceType) {
  return request('/api/contact', {
    method: 'POST',
    body: JSON.stringify({ ...formData, ...getVisitorMetadata(), deviceType }),
  });
}

export async function recordVisit(pagePath, deviceType) {
  return request('/api/analytics/visit', {
    method: 'POST',
    body: JSON.stringify({ pagePath, ...getVisitorMetadata(), deviceType }),
  });
}

export async function login(username, password) {
  return request('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify({ username, password }),
  });
}

export async function fetchDashboard(token) {
  return request('/api/analytics/dashboard', {
    headers: { Authorization: `Bearer ${token}` },
  });
}

export { API_BASE };
