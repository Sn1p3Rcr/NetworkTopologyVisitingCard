const AUTH_KEY = 'topology_client_auth';

export function getAuth() {
    const raw = localStorage.getItem(AUTH_KEY);
    return raw ? JSON.parse(raw) : null;
}

export function setAuth(data) {
    localStorage.setItem(AUTH_KEY, JSON.stringify(data));
}

export function clearAuth() {
    localStorage.removeItem(AUTH_KEY);
}

export function isAdmin() {
    const auth = getAuth();
    return auth?.roles?.includes('Admin') ?? false;
}

export function isAuthenticated() {
    return Boolean(getAuth()?.token);
}

export async function api(path, options = {}) {
    const auth = getAuth();
    const headers = {
        'Content-Type': 'application/json',
        ...(options.headers || {})
    };

    if (auth?.token) {
        headers['Authorization'] = `Bearer ${auth.token}`;
    }

    const response = await fetch(path, { ...options, headers });

    if (response.status === 401) {
        clearAuth();
        throw new Error('Сессия истекла. Войдите снова.');
    }

    const text = await response.text();
    let data = null;
    if (text) {
        try {
            data = JSON.parse(text);
        } catch {
            data = text;
        }
    }

    if (!response.ok) {
        let message = `Ошибка ${response.status}`;
        if (data?.message) message = data.message;
        else if (typeof data === 'string') message = data;
        else if (data?.errors) message = Object.values(data.errors).flat().join(', ');
        else if (data?.title) message = data.title;
        throw new Error(message);
    }

    return data;
}

export const authApi = {
    login: (email, password) =>
        api('/api/auth/login', { method: 'POST', body: JSON.stringify({ email, password }) }),
    register: (email, password, fullName) =>
        api('/api/auth/register', {
            method: 'POST',
            body: JSON.stringify({ email, password, fullName })
        })
};

export const projectsApi = {
    list: () => api('/api/projects'),
    get: (id) => api(`/api/projects/${id}`),
    create: (body) => api('/api/projects', { method: 'POST', body: JSON.stringify(body) }),
    update: (id, body) => api(`/api/projects/${id}`, { method: 'PUT', body: JSON.stringify(body) }),
    remove: (id) => api(`/api/projects/${id}`, { method: 'DELETE' })
};

export const reviewsApi = {
    list: () => api('/api/reviews'),
    byProject: (projectId) => api(`/api/reviews/by-project/${projectId}`),
    create: (body) => api('/api/reviews', { method: 'POST', body: JSON.stringify(body) }),
    update: (id, body) => api(`/api/reviews/${id}`, { method: 'PUT', body: JSON.stringify(body) }),
    remove: (id) => api(`/api/reviews/${id}`, { method: 'DELETE' })
};

export const technologiesApi = {
    list: () => api('/api/technologies'),
    create: (body) => api('/api/technologies', { method: 'POST', body: JSON.stringify(body) }),
    update: (id, body) => api(`/api/technologies/${id}`, { method: 'PUT', body: JSON.stringify(body) }),
    remove: (id) => api(`/api/technologies/${id}`, { method: 'DELETE' })
};

export const topologiesApi = {
    list: () => api('/api/networktopologies'),
    byProject: (projectId) => api(`/api/networktopologies/by-project/${projectId}`),
    create: (body) => api('/api/networktopologies', { method: 'POST', body: JSON.stringify(body) }),
    update: (id, body) => api(`/api/networktopologies/${id}`, { method: 'PUT', body: JSON.stringify(body) }),
    remove: (id) => api(`/api/networktopologies/${id}`, { method: 'DELETE' })
};
