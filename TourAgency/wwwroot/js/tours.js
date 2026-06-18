document.addEventListener('DOMContentLoaded', () => {
    let currentCategory = 'Все';
    let currentMaxPrice = 300000;
    let currentMinRating = 0;

    initUserMenu();
    loadCategories();
    loadTours();

    // Загрузка категорий
    async function loadCategories() {
        try {
            const categories = await get('/categories');
            const container = document.getElementById('categories-container');
            container.innerHTML = '';

            categories.forEach(cat => {
                const chip = document.createElement('button');
                chip.className = `chip ${cat === 'Все' ? 'active' : ''}`;
                chip.textContent = cat;
                chip.addEventListener('click', () => {
                    document.querySelectorAll('.chip').forEach(c => c.classList.remove('active'));
                    chip.classList.add('active');
                    currentCategory = cat;
                    loadTours();
                });
                container.appendChild(chip);
            });
        } catch (err) {
            console.error('Ошибка загрузки категорий:', err);
        }
    }

    // Загрузка туров
    async function loadTours() {
        try {
            const category = currentCategory === 'Все' ? null : currentCategory;
            const url = `/tours?category=${category || ''}&maxPrice=${currentMaxPrice}&minRating=${currentMinRating}`;
            const tours = await get(url);

            const container = document.getElementById('tours-container');
            container.innerHTML = '';

            if (tours.length === 0) {
                container.innerHTML = '<div class="loading">😕 Туры не найдены. Попробуй изменить фильтры!</div>';
                return;
            }

            tours.forEach((tour, index) => {
                const card = document.createElement('div');
                card.className = 'tour-card';
                card.innerHTML = `
                    <img src="${tour.imageUrl}" alt="${tour.name}" class="tour-card-image">
                    <div class="tour-card-body">
                        <div class="tour-card-header">
                            <span class="tour-badge">${tour.category}</span>
                            <span class="tour-rating">⭐ ${tour.averageRating.toFixed(1)}</span>
                        </div>
                        <h2>${tour.name}</h2>
                        <p class="tour-card-desc">${tour.description}</p>
                        <div class="tour-card-footer">
                            <span class="tour-price">${formatPrice(tour.price)}</span>
                            <button class="tour-btn" onclick="window.location.href='/tour?id=${tour.id}'">Подробнее →</button>
                        </div>
                    </div>
                `;
                container.appendChild(card);
            });
        } catch (err) {
            console.error('Ошибка загрузки туров:', err);
        }
    }

    // Фильтр по цене
    document.getElementById('price-filter').addEventListener('input', (e) => {
        currentMaxPrice = parseInt(e.target.value);
        document.getElementById('price-max-val').textContent = formatPrice(currentMaxPrice);
        loadTours();
    });

    // Фильтр по рейтингу
    document.getElementById('rating-filter').addEventListener('change', (e) => {
        currentMinRating = parseFloat(e.target.value);
        loadTours();
    });
});
