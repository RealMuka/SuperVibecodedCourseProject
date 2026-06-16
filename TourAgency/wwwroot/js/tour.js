document.addEventListener('DOMContentLoaded', () => {
    const tourId = getUrlParam('id');
    const reviewForm = document.getElementById('review-form');

    if (!tourId) {
        showGlobalError('ID тура не указан');
        return;
    }

    loadTourDetails();

    async function loadTourDetails() {
        try {
            const tour = await get(`/tours/${tourId}`);

            document.getElementById('tour-loading').classList.add('hidden');
            document.getElementById('tour-details').classList.remove('hidden');

            document.title = `${tour.name} — Поехали!`;
            document.getElementById('tour-title').textContent = tour.name;
            document.getElementById('tour-image').src = tour.imageUrl;
            document.getElementById('tour-category').textContent = tour.category;
            document.getElementById('tour-rating').textContent = `⭐ ${tour.averageRating.toFixed(1)}`;
            document.getElementById('tour-price').textContent = formatPrice(tour.price);
            document.getElementById('tour-description').textContent = tour.description;

            renderReviews(tour.reviews || []);
        } catch (err) {
            document.getElementById('tour-loading').textContent = '❌ Ошибка загрузки тура';
        }
    }

    function renderReviews(reviews) {
        const container = document.getElementById('reviews-container');
        container.innerHTML = '';

        if (reviews.length === 0) {
            container.innerHTML = '<div class="loading">Отзывов пока нет. Будьте первыми! 🚀</div>';
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

    reviewForm.addEventListener('submit', async (e) => {
        e.preventDefault();

        const data = {
            userName: document.getElementById('review-username').value,
            rating: parseInt(document.getElementById('review-rating').value),
            text: document.getElementById('review-text').value
        };

        try {
            const updatedTour = await post(`/tours/${tourId}/reviews`, data);
            reviewForm.reset();
            renderReviews(updatedTour.reviews || []);
        } catch (err) {
            alert('Не удалось отправить отзыв');
        }
    });
});
