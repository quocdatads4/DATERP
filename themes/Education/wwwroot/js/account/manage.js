/**
 * Account Manage Page JavaScript
 */
(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        initAvatarUpload();
        initPasswordToggle();
        initFormValidation();
    });

    /**
     * Initialize avatar upload functionality
     */
    function initAvatarUpload() {
        const avatarUpload = document.getElementById('avatarUpload');
        const uploadedAvatar = document.getElementById('uploadedAvatar');
        const resetButton = document.getElementById('resetAvatar');

        // Store original avatar src
        const originalAvatarSrc = uploadedAvatar ? uploadedAvatar.src : '';

        if (avatarUpload && uploadedAvatar) {
            avatarUpload.addEventListener('change', function (e) {
                const file = e.target.files[0];
                if (file) {
                    // Validate file size (800KB max)
                    if (file.size > 800 * 1024) {
                        abp.message.error('Kích thước file không được vượt quá 800KB', 'Lỗi');
                        this.value = '';
                        return;
                    }

                    // Validate file type
                    if (!['image/jpeg', 'image/png'].includes(file.type)) {
                        abp.message.error('Chỉ chấp nhận file JPG hoặc PNG', 'Lỗi');
                        this.value = '';
                        return;
                    }

                    // Preview image
                    const reader = new FileReader();
                    reader.onload = function (event) {
                        uploadedAvatar.src = event.target.result;
                    };
                    reader.readAsDataURL(file);
                }
            });
        }

        if (resetButton && uploadedAvatar) {
            resetButton.addEventListener('click', function () {
                uploadedAvatar.src = originalAvatarSrc;
                if (avatarUpload) {
                    avatarUpload.value = '';
                }
            });
        }
    }

    /**
     * Initialize password visibility toggle
     */
    function initPasswordToggle() {
        const toggleButtons = document.querySelectorAll('.toggle-password');

        toggleButtons.forEach(function (button) {
            button.addEventListener('click', function () {
                const input = this.parentElement.querySelector('input');
                const icon = this.querySelector('i');

                if (input.type === 'password') {
                    input.type = 'text';
                    icon.classList.remove('ti-eye-off');
                    icon.classList.add('ti-eye');
                } else {
                    input.type = 'password';
                    icon.classList.remove('ti-eye');
                    icon.classList.add('ti-eye-off');
                }
            });
        });
    }

    /**
     * Initialize form validation
     */
    function initFormValidation() {
        const changePasswordForm = document.getElementById('formChangePassword');

        if (changePasswordForm) {
            changePasswordForm.addEventListener('submit', function (e) {
                const newPassword = document.getElementById('newPassword');
                const confirmPassword = document.getElementById('confirmPassword');

                if (newPassword && confirmPassword) {
                    if (newPassword.value !== confirmPassword.value) {
                        e.preventDefault();
                        abp.message.error('Mật khẩu xác nhận không khớp với mật khẩu mới', 'Lỗi');
                        confirmPassword.focus();
                        return false;
                    }

                    if (newPassword.value.length < 6) {
                        e.preventDefault();
                        abp.message.error('Mật khẩu phải có ít nhất 6 ký tự', 'Lỗi');
                        newPassword.focus();
                        return false;
                    }
                }
            });
        }

        // Personal info form
        const personalInfoForm = document.getElementById('formPersonalInfo');
        if (personalInfoForm) {
            personalInfoForm.addEventListener('submit', function (e) {
                // Add any custom validation here if needed
            });
        }
    }

})();
