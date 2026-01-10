/**
 * Login/Register Page JavaScript
 * Premium Input Interactions & Password Toggle
 */

(function () {
    'use strict';

    // ===== Password Toggle Functionality =====
    function initPasswordToggle() {
        const toggleButtons = document.querySelectorAll('.premium-input-action');

        toggleButtons.forEach(button => {
            button.addEventListener('click', function () {
                const inputGroup = this.closest('.premium-input-group');
                const passwordInput = inputGroup.querySelector('input[type="password"], input[type="text"]');
                const icon = this.querySelector('i');

                if (passwordInput.type === 'password') {
                    passwordInput.type = 'text';
                    icon.classList.remove('ti-eye-off');
                    icon.classList.add('ti-eye');
                } else {
                    passwordInput.type = 'password';
                    icon.classList.remove('ti-eye');
                    icon.classList.add('ti-eye-off');
                }
            });
        });
    }

    // ===== Input Focus Animation =====
    function initInputAnimations() {
        const inputs = document.querySelectorAll('.premium-input');

        inputs.forEach(input => {
            // Add filled class when input has value
            input.addEventListener('input', function () {
                if (this.value.trim() !== '') {
                    this.classList.add('has-value');
                } else {
                    this.classList.remove('has-value');
                }
            });

            // Check on page load for pre-filled values
            if (input.value.trim() !== '') {
                input.classList.add('has-value');
            }

            // Focus effect
            input.addEventListener('focus', function () {
                const group = this.closest('.premium-input-group');
                if (group) {
                    group.classList.add('is-focused');
                }
            });

            input.addEventListener('blur', function () {
                const group = this.closest('.premium-input-group');
                if (group) {
                    group.classList.remove('is-focused');
                }
            });
        });
    }

    // ===== Form Validation Enhancement =====
    function initFormValidation() {
        const forms = document.querySelectorAll('form');

        forms.forEach(form => {
            form.addEventListener('submit', function (e) {
                const inputs = this.querySelectorAll('.premium-input[required]');
                let isValid = true;

                inputs.forEach(input => {
                    const group = input.closest('.premium-input-group');

                    if (input.value.trim() === '') {
                        isValid = false;
                        if (group) {
                            group.classList.add('is-invalid');

                            // Shake animation
                            group.classList.add('shake');
                            setTimeout(() => group.classList.remove('shake'), 500);
                        }
                    } else {
                        if (group) {
                            group.classList.remove('is-invalid');
                        }
                    }
                });

                if (!isValid) {
                    e.preventDefault();
                }
            });

            // Remove invalid state on input
            form.querySelectorAll('.premium-input').forEach(input => {
                input.addEventListener('input', function () {
                    const group = this.closest('.premium-input-group');
                    if (group && this.value.trim() !== '') {
                        group.classList.remove('is-invalid');
                    }
                });
            });
        });
    }

    // ===== Button Loading State =====
    function initButtonLoading() {
        const submitButtons = document.querySelectorAll('.premium-btn[type="submit"]');

        submitButtons.forEach(button => {
            const form = button.closest('form');

            if (form) {
                form.addEventListener('submit', function () {
                    // Add loading state
                    button.classList.add('is-loading');
                    button.disabled = true;

                    const originalContent = button.innerHTML;
                    button.innerHTML = `
                        <span class="d-flex align-items-center justify-content-center gap-2">
                            <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                            Đang xử lý...
                        </span>
                    `;

                    // Reset after timeout (fallback)
                    setTimeout(() => {
                        button.classList.remove('is-loading');
                        button.disabled = false;
                        button.innerHTML = originalContent;
                    }, 10000);
                });
            }
        });
    }

    // ===== Caps Lock Warning =====
    function initCapsLockWarning() {
        const passwordInputs = document.querySelectorAll('input[type="password"]');

        passwordInputs.forEach(input => {
            const group = input.closest('.premium-input-group');

            input.addEventListener('keyup', function (e) {
                if (e.getModifierState && e.getModifierState('CapsLock')) {
                    if (!group.querySelector('.caps-lock-warning')) {
                        const warning = document.createElement('div');
                        warning.className = 'caps-lock-warning';
                        warning.innerHTML = '<i class="ti ti-alert-triangle"></i>';
                        warning.title = 'Caps Lock đang bật';
                        group.appendChild(warning);
                    }
                } else {
                    const warning = group.querySelector('.caps-lock-warning');
                    if (warning) {
                        warning.remove();
                    }
                }
            });
        });
    }

    // ===== Initialize All =====
    function init() {
        initPasswordToggle();
        initInputAnimations();
        initFormValidation();
        initButtonLoading();
        initCapsLockWarning();
    }

    // Run on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();
