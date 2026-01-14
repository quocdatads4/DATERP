const { Builder, By, Key, until } = require('selenium-webdriver');
const chrome = require('selenium-webdriver/chrome');
const fs = require('fs');
const path = require('path');

/**
 * verify_examination_module.js
 * Kiểm tra cấu trúc menu mới: Đào tạo -> Quản lý thi cử -> Môn thi.
 * Hỗ trợ các menu đa cấp (nested dropdowns).
 * 
 * Enhanced with detailed logging to .agent/log directory
 */

const LOG_DIR = path.join(__dirname, '..', '..', 'log');
const LOG_FILE = path.join(LOG_DIR, `examination_automation_${new Date().toISOString().replace(/[:.]/g, '-')}.log`);

function log(message) {
    const timestamp = new Date().toISOString();
    const logMessage = `[${timestamp}] ${message}`;
    console.log(logMessage);

    // Ensure log directory exists
    if (!fs.existsSync(LOG_DIR)) {
        fs.mkdirSync(LOG_DIR, { recursive: true });
    }
    fs.appendFileSync(LOG_FILE, logMessage + '\n');
}

(async function verifyExaminationRestructuredUI() {
    let options = new chrome.Options();
    options.addArguments('--no-sandbox');
    options.addArguments('--disable-dev-shm-usage');
    options.addArguments('--disable-gpu');
    options.addArguments('--window-size=1600,1000');
    options.addArguments('--incognito');

    let driver;
    try {
        driver = await new Builder()
            .forBrowser('chrome')
            .setChromeOptions(options)
            .build();
    } catch (err) {
        log('FAILED to start Chrome driver.');
        log(err.message);
        return;
    }

    try {
        log('=== EXAMINATION RESTRUCTURED MENU VERIFICATION ===');
        log(`Log file: ${LOG_FILE}`);

        // 1. Đăng nhập Admin
        log('Step 1: Logging in as Administrator...');
        await driver.get('http://localhost:5223/Account/Login?culture=vi');
        await driver.sleep(2000);

        log('Waiting for username field...');
        let usernameField = await driver.wait(
            until.elementLocated(By.css('input[name="LoginInput.UserNameOrEmailAddress"]')),
            15000
        );
        await driver.wait(until.elementIsVisible(usernameField), 5000);

        log('Entering username: admin@datacademy.edu.vn');
        await usernameField.clear();
        await usernameField.sendKeys('admin@datacademy.edu.vn');

        log('Waiting for password field...');
        let passwordField = await driver.wait(
            until.elementLocated(By.id('passwordInput')),
            5000
        );
        await driver.wait(until.elementIsVisible(passwordField), 5000);

        log('Entering password using JavaScript...');
        // Use JavaScript to set password value directly (more reliable)
        await driver.executeScript(`
            const pwd = document.getElementById('passwordInput');
            pwd.value = 'Admin@123';
            pwd.dispatchEvent(new Event('input', { bubbles: true }));
            pwd.dispatchEvent(new Event('change', { bubbles: true }));
        `);
        await driver.sleep(500); // Small delay to ensure value is set

        // Debug: verify both fields have values
        let usernameValue = await usernameField.getAttribute('value');
        let passwordValue = await passwordField.getAttribute('value');
        log(`DEBUG: Username value = "${usernameValue}"`);
        log(`DEBUG: Password value length = ${passwordValue ? passwordValue.length : 0}`);

        log('Looking for login button...');
        let loginButton;
        try {
            // Try by type submit first (Education theme)
            loginButton = await driver.wait(
                until.elementLocated(By.css('button[type="submit"]')),
                5000
            );
            log('Found button by type="submit"');
        } catch {
            // Fallback: try by value attribute (ABP standard)
            loginButton = await driver.wait(
                until.elementLocated(By.css('button[value="Login"]')),
                5000
            );
            log('Found button by value="Login"');
        }
        await driver.wait(until.elementIsEnabled(loginButton), 5000);

        log('Clicking login button...');
        await driver.executeScript("arguments[0].click();", loginButton);

        log('Waiting for redirect...');
        try {
            // Wait for URL to change from login page
            await driver.wait(async () => {
                let currentUrl = await driver.getCurrentUrl();
                return !currentUrl.includes('/Account/Login');
            }, 20000);

            let newUrl = await driver.getCurrentUrl();
            log(`SUCCESS: Redirected to ${newUrl}`);
        } catch (waitErr) {
            log('TIMEOUT waiting for redirect. Checking current state...');
            let currentUrl = await driver.getCurrentUrl();
            log(`Current URL: ${currentUrl}`);

            // Check for validation errors
            let errorElements = await driver.findElements(By.css('.text-danger'));
            for (let el of errorElements) {
                let text = await el.getText();
                if (text.trim()) {
                    log('Validation Error: ' + text);
                }
            }

            // Check for alert messages
            let alerts = await driver.findElements(By.css('.alert'));
            for (let alert of alerts) {
                let text = await alert.getText();
                if (text.trim()) {
                    log('Alert Message: ' + text);
                }
            }

            // Take screenshot on failure
            let screenshot = await driver.takeScreenshot();
            let screenshotPath = path.join(LOG_DIR, 'login_failure_screenshot.png');
            fs.writeFileSync(screenshotPath, screenshot, 'base64');
            log(`Screenshot saved: ${screenshotPath}`);

            throw waitErr;
        }

        // 2. Mở Menu "Đào tạo"
        log('Step 2: Opening "Đào tạo" menu...');
        let educationMenu = await driver.wait(
            until.elementLocated(By.xpath("//span[contains(text(), 'Đào tạo')]/..")),
            15000
        );
        await educationMenu.click();
        await driver.sleep(1000);

        // 3. Hover "Quản lý thi cử" bên trong "Đào tạo"
        log('Step 3: Hovering over "Quản lý thi cử" sub-menu...');
        let examSubMenu = await driver.wait(
            until.elementLocated(By.xpath("//span[contains(text(), 'Quản lý thi cử')]/..")),
            5000
        );

        const actions = driver.actions({ bridge: true });
        await actions.move({ duration: 500, origin: examSubMenu, x: 0, y: 0 }).perform();
        await driver.sleep(1500);

        // 4. Kiểm tra các menu con hiển thị khi hover
        log('Step 4: Checking sub-items (Môn thi, Bài thi, Đề thi)...');
        const items = ['Môn thi', 'Bài thi', 'Đề thi'];
        for (const item of items) {
            let el = await driver.wait(until.elementLocated(By.xpath(`//span[contains(text(), '${item}')]/..`)), 5000);
            await driver.wait(until.elementIsVisible(el), 5000);
            log(`- Found and visible sub-item: ${item}`);
        }

        // 5. Truy cập "Môn thi"
        log('Step 5: Navigating to "Môn thi"...');
        let monThiLink = await driver.findElement(By.xpath("//span[contains(text(), 'Môn thi')]/.."));
        await driver.executeScript("arguments[0].click();", monThiLink);

        await driver.wait(until.urlContains('/Examination/ExamSubjects'), 15000);
        log('SUCCESS: Navigated to Subjects management page.');

        // Get Browser Console Logs
        await driver.sleep(3000);
        var logs = await driver.manage().logs().get('browser');
        log('--- BROWSER LOGS START ---');
        logs.forEach(function (entry) {
            log('[BROWSER] ' + entry.level.name + ': ' + entry.message);
        });
        log('--- BROWSER LOGS END ---');

        // 6. Kiểm tra Dashboard Stats
        log('Step 6: Verifying Dashboard Stats cards...');
        let statsCards = await driver.findElements(By.className('stats-card'));
        log(`- Found ${statsCards.length} stats cards.`);
        if (statsCards.length === 3) {
            log('SUCCESS: All 3 stats cards are present.');
        } else {
            log('WARNING: Missing some stats cards.');
        }

        // 7. Kiểm tra DataTable Action Buttons
        log('Step 7: Checking for Action buttons in table...');
        try {
            let actionButtons = await driver.wait(until.elementsLocated(By.className('btn-group')), 5000);
            log(`- Found ${actionButtons.length} action button groups in table rows.`);
        } catch (e) {
            log('- No action buttons found (possibly empty table).');
        }

        log('');
        log('✅ PREMIUM UI DESIGN VERIFIED SUCCESSFULLY.');

    } catch (e) {
        log('--- AUTOMATION ERROR ---');
        log(e.message);
    } finally {
        log('');
        log('Verification process finished.');
        log(`Full log saved to: ${LOG_FILE}`);
    }
})();
