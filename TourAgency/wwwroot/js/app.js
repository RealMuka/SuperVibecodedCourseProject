const API_BASE = '/api';

async function get(url) {
    try {
        const response = await fetch(`${API_BASE}${url}`, {
            credentials: 'include'
        });
        if (!response.ok) throw new Error(`Ошибка ${response.status}`);
        return await response.json();
    } catch (error) {
        showGlobalError(error.message);
        throw error;
    }
}

async function post(url, data) {
    try {
        const response = await fetch(`${API_BASE}${url}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data),
            credentials: 'include'
        });
        if (!response.ok) throw new Error(`Ошибка ${response.status}`);
        return await response.json();
    } catch (error) {
        showGlobalError(error.message);
        throw error;
    }
}

function getUrlParam(param) {
    return new URLSearchParams(window.location.search).get(param);
}

function formatPrice(value) {
    return new Intl.NumberFormat('ru-RU', {
        style: 'currency',
        currency: 'RUB',
        maximumFractionDigits: 0
    }).format(value);
}

function showGlobalError(message) {
    const container = document.getElementById('error-container');
    if (container) {
        container.textContent = `⚠️ Ошибка: ${message}`;
        container.classList.remove('hidden');
    }
}
