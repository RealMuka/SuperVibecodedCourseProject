document.addEventListener('DOMContentLoaded', () => {
    const userId = getUrlParam('userId') || '1';

    loadProfile();

    async function loadProfile() {
        try {
            const user = await get(`/users/${userId}`);

            document.getElementById('profile-loading').classList.add('hidden');
            document.getElementById('profile-content').classList.remove('hidden');

            document.getElementById('user-name').textContent = user.name;
            document.getElementById('user-bio').textContent = user.description;
            document.getElementById('user-avatar').textContent = user.name.charAt(0).toUpperCase();
            document.getElementById('stat-tours-count').textContent = user.toursRequested;

            renderUserReviews(user.reviews || []);
        } catch (err) {
            document.getElementById('profile-loading').textContent = '❌ Ошибка загрузки профиля';
        }
    }

    function renderUserReviews(reviews) {
        const container = document.getElementById('user-reviews-container');
        container.innerHTML = '';

        if (reviews.length === 0) {
            container.innerHTML = '<div class="loading">Вы еще не оставляли отзывов 😊</div>';
            return;
        }

        reviews.forEach(review => {
            const dateStr = new Date(review.createdAt).toLocaleDateString('ru-RU');
            const item = document.createElement('div');
            item.className = 'review-item';
            item.innerHTML = `
                <div class="review-header">
                    <span class="review-author">${review.userName}</span>
                    <div>
                        <span class="review-rating">${'⭐'.repeat(review.rating)}</span>
                        <span class="review-date">${dateStr}</span>
                    </div>
                </div>
                <p class="review-text">${review.text}</p>
            `;
            container.appendChild(item);
        });
    }
});
