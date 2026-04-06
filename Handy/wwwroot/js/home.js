namespace Handy.wwwroot.js
// home.js - отдельный файл для скриптов

// Ripple эффект для кнопок
function addRippleEffect() {
    document.querySelectorAll('.btn-top').forEach(button => {
        button.addEventListener('click', function (e) {
            const ripple = document.createElement('span');
            ripple.classList.add('ripple');
            this.appendChild(ripple);

            const rect = this.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            ripple.style.left = `${x}px`;
            ripple.style.top = `${y}px`;

            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    });
}

// Эффект параллакса для карточки
function addParallaxEffect() {
    const card = document.querySelector('.home-card');
    if (card) {
        card.addEventListener('mousemove', function (e) {
            const rect = card.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;
            const centerX = rect.width / 2;
            const centerY = rect.height / 2;
            const rotateX = (y - centerY) / 30;
            const rotateY = (centerX - x) / 30;
            card.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) translateY(-5px)`;
        });

        card.addEventListener('mouseleave', function () {
            card.style.transform = 'perspective(1000px) rotateX(0deg) rotateY(0deg) translateY(0px)';
        });
    }
}

// Анимация появления элементов при скролле
function addScrollAnimation() {
    const elements = document.querySelectorAll('.home-title, .home-description, .top-buttons');
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, { threshold: 0.1 });

    elements.forEach(el => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(20px)';
        el.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(el);
    });
}

// Инициализация всех эффектов при загрузке страницы
document.addEventListener('DOMContentLoaded', function () {
    addRippleEffect();
    addParallaxEffect();
    // addScrollAnimation(); // Раскомментировать если нужна анимация при скролле

    console.log('Home page scripts initialized');
});