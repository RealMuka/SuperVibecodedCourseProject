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

async function logout() {
    try {
        await fetch(`${API_BASE}/auth/logout`, {
            method: 'POST',
            credentials: 'include'
        });
    } catch (e) {
    }
    localStorage.removeItem('userName');
    window.location.href = '/auth';
}

async function initUserMenu(hideProfileLink = false) {
    const userMenuBtn = document.getElementById('user-menu-btn');
    const userMenuDropdown = document.getElementById('user-menu-dropdown');
    const userNameHeader = document.getElementById('user-name-header');
    const userAvatarSmall = document.getElementById('user-avatar-small');
    const logoutBtnDropdown = document.getElementById('logout-btn-dropdown');
    const profileMenuItem = document.getElementById('profile-menu-item');

    if (!userMenuBtn) return;

    try {
        const user = await get('/auth/me');
        userNameHeader.textContent = user.name;
        userAvatarSmall.textContent = user.name.charAt(0).toUpperCase();
    } catch (e) {
        // User not authenticated
    }

    if (hideProfileLink && profileMenuItem) {
        profileMenuItem.classList.add('hidden');
    }

    userMenuBtn.addEventListener('click', (e) => {
        e.stopPropagation();
        userMenuDropdown.classList.toggle('hidden');
        userMenuBtn.classList.toggle('active');
    });

    if (logoutBtnDropdown) {
        logoutBtnDropdown.addEventListener('click', logout);
    }

    userMenuDropdown?.addEventListener('click', (e) => {
        e.stopPropagation();
    });

    document.addEventListener('click', () => {
        if (userMenuDropdown && !userMenuDropdown.classList.contains('hidden')) {
            userMenuDropdown.classList.add('hidden');
            userMenuBtn.classList.remove('active');
        }
    });
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
