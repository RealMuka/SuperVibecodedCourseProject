// Константы для определения текущего режима
let currentMode = 'login';

const authForm = document.getElementById('auth-form');
const usernameInput = document.getElementById('auth-username');
const passwordInput = document.getElementById('auth-password');
const errorContainer = document.getElementById('auth-error');
const submitBtn = document.getElementById('auth-submit-btn');

// Базовый URL для API
const API_BASE_URL = '/api/auth'; 

// Переключение вкладок
function switchTab(mode) {
    currentMode = mode;
    errorContainer.classList.add('hidden');
    
    const tabLogin = document.getElementById('tab-login');
    const tabRegister = document.getElementById('tab-register');

    if (mode === 'login') {
        tabLogin.classList.add('active');
        tabRegister.classList.remove('active');
        submitBtn.textContent = 'Войти';
    } else {
        tabLogin.classList.remove('active');
        tabRegister.classList.add('active');
        submitBtn.textContent = 'Зарегистрироваться';
    }
}

// Обработка отправки формы
authForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    errorContainer.classList.add('hidden');
    
    const username = usernameInput.value.trim();
    const password = passwordInput.value;

    const requestData = {
        name: username,
        password: password
    };

    const endpoint = currentMode === 'login' ? '/login' : '/register';

    try {
        submitBtn.disabled = true;
        submitBtn.textContent = 'Секундочку... ⏳';

        const response = await fetch(`${API_BASE_URL}${endpoint}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(requestData),
            credentials: 'include'
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.message || 'Что-то пошло не так. Попробуйте снова.');
        }

        if (currentMode === 'register') {
            alert('Регистрация успешна! Теперь войдите в аккаунт.');
            switchTab('login');
            passwordInput.value = '';
        } else {
            localStorage.setItem('userName', username);
            window.location.href = '/tours';
        }

    } catch (error) {
        errorContainer.textContent = error.message;
        errorContainer.classList.remove('hidden');
    } finally {
        submitBtn.disabled = false;
        submitBtn.textContent = currentMode === 'login' ? 'Войти' : 'Зарегистрироваться';
    }
});

// Защита страницы авторизации
document.addEventListener('DOMContentLoaded', async () => {
    try {
        const response = await fetch(`${API_BASE_URL}/me`, { credentials: 'include' });
        if (response.ok) {
            const user = await response.json();
            localStorage.setItem('userName', user.name);
            window.location.href = '/tours';
        }
    } catch (e) {
        // Пользователь не авторизован
    }
});