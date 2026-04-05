namespace Handy.wwwroot.js
// auth.js - скрипты для страниц входа и регистрации

document.addEventListener('DOMContentLoaded', function () {
    // Добавляем ripple эффект на кнопки
    addRippleEffect();

    // Валидация формы
    setupFormValidation();
});

function addRippleEffect() {
    const buttons = document.querySelectorAll('.btn-submit');

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

function setupFormValidation() {
    const form = document.querySelector('.auth-form');
    if (!form) return;

    form.addEventListener('submit', function (e) {
        // Проверка паролей для регистрации
        const password = form.querySelector('input[type="password"]');
        const confirmPassword = form.querySelectorAll('input[type="password"]')[1];

        if (password && confirmPassword && password.value !== confirmPassword.value) {
            e.preventDefault();
            showError(confirmPassword, 'Пароли не совпадают');
        }
    });
}

function showError(input, message) {
    input.classList.add('error');
    let errorSpan = input.parentElement.querySelector('.error-message');
    if (!errorSpan) {
        errorSpan = document.createElement('span');
        errorSpan.classList.add('error-message');
        input.parentElement.appendChild(errorSpan);
    }
    errorSpan.textContent = message;
}