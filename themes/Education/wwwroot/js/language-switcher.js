/**
 * Language Switcher for ABP Framework
 * Sets the .AspNetCore.Culture cookie and reloads the page
 */
(function () {
    'use strict';

    // Cookie name for ASP.NET Core culture (ABP Framework standard)
    const CULTURE_COOKIE_NAME = '.AspNetCore.Culture';

    /**
     * Get the current culture from cookie
     * @returns {string|null} The current culture code (e.g., 'en', 'vi')
     */
    function getCurrentCulture() {
        const cookies = document.cookie.split(';');
        for (let i = 0; i < cookies.length; i++) {
            const cookie = cookies[i].trim();
            if (cookie.startsWith(CULTURE_COOKIE_NAME + '=')) {
                const value = decodeURIComponent(cookie.substring(CULTURE_COOKIE_NAME.length + 1));
                // Parse format: c=en|uic=en or c=vi|uic=vi
                const match = value.match(/c=([^|]+)/);
                if (match) {
                    return match[1].split('-')[0]; // Return just language code (en, vi)
                }
            }
        }
        return null;
    }

    /**
     * Set the culture cookie and reload the page
     * @param {string} culture - The culture code (e.g., 'en', 'vi')
     */
    function setCulture(culture) {
        // ABP Framework expects format: c=<culture>|uic=<culture>
        const cultureValue = `c=${culture}|uic=${culture}`;

        // Set cookie with path=/ and expires in 1 year
        const expires = new Date();
        expires.setFullYear(expires.getFullYear() + 1);

        document.cookie = `${CULTURE_COOKIE_NAME}=${encodeURIComponent(cultureValue)};path=/;expires=${expires.toUTCString()};SameSite=Lax`;

        // Reload the page to apply the new culture
        window.location.reload();
    }

    /**
     * Update the active state of language dropdown items
     */
    function updateActiveLanguage() {
        const currentCulture = getCurrentCulture();
        const languageItems = document.querySelectorAll('.dropdown-language .dropdown-item[data-language]');

        languageItems.forEach(item => {
            const language = item.getAttribute('data-language');
            if (language === currentCulture) {
                item.classList.add('active');
            } else {
                item.classList.remove('active');
            }
        });
    }

    /**
     * Initialize the language switcher
     */
    function init() {
        // Update active language on page load
        updateActiveLanguage();

        // Add click event listeners to language dropdown items
        const languageItems = document.querySelectorAll('.dropdown-language .dropdown-item[data-language]');

        languageItems.forEach(item => {
            item.addEventListener('click', function (e) {
                e.preventDefault();
                const language = this.getAttribute('data-language');
                if (language) {
                    setCulture(language);
                }
            });
        });
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    // Expose functions globally for debugging
    window.languageSwitcher = {
        getCurrent: getCurrentCulture,
        setCulture: setCulture
    };
})();
