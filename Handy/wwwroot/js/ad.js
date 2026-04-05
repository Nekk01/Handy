namespace Handy.wwwroot.js
// ad.js - скрипты для страницы создания объявления

document.addEventListener('DOMContentLoaded', function () {
    // Добавляем ripple эффект
    addRippleEffect();

    // Валидация цены
    setupPriceValidation();

    // Предпросмотр ссылки на изображение
    setupImagePreview();
});

function addRippleEffect() {
    const buttons = document.querySelectorAll('.btn-submit, .btn-back');

    buttons.forEach(button => {
        button.addEventListener('click', function (e) {
            const ripple = document.createElement('span');
            ripple.classList.add('ripple');
            this.style.position = 'relative';
            this.style.overflow = 'hidden';
            this.appendChild(ripple);

            const rect = this.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            ripple.style.left = `${x}px`;
            ripple.style.top = `${y}px`;
            ripple.style.position = 'absolute';

            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    });
}

function setupPriceValidation() {
    const priceInput = document.querySelector('input[type="number"]');
    if (!priceInput) return;

    priceInput.addEventListener('change', function () {
        if (this.value < 0) {
            this.value = 0;
        }
    });

    priceInput.addEventListener('input', function () {
        if (this.value === '' || this.value === null) {
            return;
        }
        const numValue = parseFloat(this.value);
        if (isNaN(numValue)) {
            this.value = 0;
        }
    });
}

function setupImagePreview() {
    const imageUrlInput = document.querySelector('input[asp-for="ADImageUrl"]');
    if (!imageUrlInput) return;

    let previewContainer = null;

    imageUrlInput.addEventListener('blur', function () {
        const url = this.value.trim();

        if (url && (url.startsWith('http://') || url.startsWith('https://'))) {
            if (!previewContainer) {
                previewContainer = document.createElement('div');
                previewContainer.className = 'image-preview';
                previewContainer.style.marginTop = '10px';
                previewContainer.style.borderRadius = '12px';
                previewContainer.style.overflow = 'hidden';
                previewContainer.style.background = 'rgba(255,255,255,0.05)';
                previewContainer.style.padding = '10px';
                previewContainer.style.textAlign = 'center';

                const img = document.createElement('img');
                img.style.maxWidth = '100%';
                img.style.maxHeight = '150px';
                img.style.borderRadius = '8px';
                previewContainer.appendChild(img);

                this.parentElement.appendChild(previewContainer);
            }

            const img = previewContainer.querySelector('img');
            img.src = url;
            img.onerror = function () {
                img.src = 'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" width="100" height="100" viewBox="0 0 24 24" fill="none" stroke="%238BA3C6" stroke-width="2"%3E%3Crect x="3" y="3" width="18" height="18" rx="2"%3E%3C/rect%3E%3Ccircle cx="8.5" cy="8.5" r="1.5"%3E%3C/circle%3E%3Cpolyline points="21 15 16 10 5 21"%3E%3C/polyline%3E%3C/svg%3E';
                img.style.opacity = '0.5';
            };
            previewContainer.style.display = 'block';
        } else if (previewContainer) {
            previewContainer.style.display = 'none';
        }
    });
}