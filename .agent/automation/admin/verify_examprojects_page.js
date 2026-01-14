const { Builder, By, until } = require('selenium-webdriver');
const chrome = require('selenium-webdriver/chrome');
const fs = require('fs');
const path = require('path');

/**
 * verify_examprojects_page.js
 * Kiểm tra trang Đề thi (ExamProjects)
 * - Đăng nhập Admin
 * - Truy cập trang ExamProjects
 * - Kiểm tra bảng hiển thị đúng
 */

const LOG_DIR = path.join(__dirname, '..', '..', 'log');
const LOG_FILE = path.join(LOG_DIR, `examprojects_automation_${new Date().toISOString().replace(/[:.]/g, '-')}.log`);

function log(message) {
    const timestamp = new Date().toISOString();
    const logMessage = `[${timestamp}] ${message}`;
    console.log(logMessage);

    if (!fs.existsSync(LOG_DIR)) {
        fs.mkdirSync(LOG_DIR, { recursive: true });
    }
    fs.appendFileSync(LOG_FILE, logMessage + '\n');
}

(async function verifyExamProjectsPage() {
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
        log('=== EXAM PROJECTS PAGE VERIFICATION ===');
        log(`Log file: ${LOG_FILE}`);

        // 1. Đăng nhập Admin
        log('Step 1: Logging in as Administrator...');
        await driver.get('http://localhost:5223/Account/Login?culture=vi');
        await driver.sleep(2000);

        let usernameField = await driver.wait(
            until.elementLocated(By.css('input[name="LoginInput.UserNameOrEmailAddress"]')),
            15000
        );
        await driver.wait(until.elementIsVisible(usernameField), 5000);
        await usernameField.clear();
        await usernameField.sendKeys('admin@datacademy.edu.vn');

        let passwordField = await driver.wait(
            until.elementLocated(By.id('passwordInput')),
            5000
        );
        await driver.wait(until.elementIsVisible(passwordField), 5000);

        await driver.executeScript(`
            const pwd = document.getElementById('passwordInput');
            pwd.value = 'Admin@123';
            pwd.dispatchEvent(new Event('input', { bubbles: true }));
            pwd.dispatchEvent(new Event('change', { bubbles: true }));
        `);
        await driver.sleep(500);

        let loginButton = await driver.wait(
            until.elementLocated(By.css('button[type="submit"]')),
            5000
        );
        await driver.wait(until.elementIsEnabled(loginButton), 5000);
        await driver.executeScript("arguments[0].click();", loginButton);

        await driver.wait(async () => {
            let currentUrl = await driver.getCurrentUrl();
            return !currentUrl.includes('/Account/Login');
        }, 20000);
        log('SUCCESS: Login completed.');

        // 2. Truy cập trực tiếp trang ExamProjects
        log('Step 2: Navigating to ExamProjects page...');
        await driver.get('http://localhost:5223/Examination/ExamProjects');
        await driver.sleep(3000);

        let currentUrl = await driver.getCurrentUrl();
        if (currentUrl.includes('/Examination/ExamProjects')) {
            log('SUCCESS: Navigated to ExamProjects page.');
        } else {
            log('WARNING: URL does not match expected. Current: ' + currentUrl);
        }

        // 3. Kiểm tra tiêu đề trang
        log('Step 3: Checking page title...');
        try {
            let pageTitle = await driver.wait(
                until.elementLocated(By.css('h1, .page-title, .card-title')),
                5000
            );
            let titleText = await pageTitle.getText();
            log(`Page title found: "${titleText}"`);
        } catch (e) {
            log('Could not find page title element.');
        }

        // 4. Kiểm tra bảng DataTable
        log('Step 4: Checking DataTable...');
        try {
            let table = await driver.wait(
                until.elementLocated(By.id('ExamProjectsTable')),
                10000
            );
            log('SUCCESS: ExamProjectsTable found.');

            // Kiểm tra các cột
            let headers = await driver.findElements(By.css('#ExamProjectsTable thead th'));
            log(`Found ${headers.length} table columns.`);
            for (let header of headers) {
                let text = await header.getText();
                if (text.trim()) {
                    log(`  - Column: ${text}`);
                }
            }
        } catch (e) {
            log('WARNING: ExamProjectsTable not found or not loaded yet.');
        }

        // 5. Kiểm tra nút tạo mới
        log('Step 5: Checking Create button...');
        try {
            let createBtn = await driver.findElement(By.id('NewExamProjectButton'));
            let btnText = await createBtn.getText();
            log(`SUCCESS: Create button found with text: "${btnText}"`);
        } catch (e) {
            log('WARNING: New ExamProject button not found.');
        }

        // 6. Kiểm tra stats cards
        log('Step 6: Checking stats cards...');
        let statsCards = await driver.findElements(By.css('.stat-card, .stats-card'));
        log(`Found ${statsCards.length} stats cards.`);

        // Get Browser Console Logs
        log('Step 7: Getting browser console logs...');
        await driver.sleep(2000);
        try {
            var logs = await driver.manage().logs().get('browser');
            if (logs.length > 0) {
                log('--- BROWSER LOGS ---');
                logs.forEach(function (entry) {
                    if (entry.level.name === 'SEVERE' || entry.level.name === 'WARNING') {
                        log('[BROWSER] ' + entry.level.name + ': ' + entry.message);
                    }
                });
            }
        } catch (e) {
            log('Could not retrieve browser logs.');
        }

        log('');
        log('✅ EXAM PROJECTS PAGE VERIFICATION COMPLETED.');

    } catch (e) {
        log('--- AUTOMATION ERROR ---');
        log(e.message);

        // Take screenshot on error
        try {
            let screenshot = await driver.takeScreenshot();
            let screenshotPath = path.join(LOG_DIR, 'examprojects_error_screenshot.png');
            fs.writeFileSync(screenshotPath, screenshot, 'base64');
            log(`Screenshot saved: ${screenshotPath}`);
        } catch (ssErr) {
            log('Could not take screenshot.');
        }
    } finally {
        log('');
        log('Verification process finished.');
        log(`Full log saved to: ${LOG_FILE}`);
        // Keep browser open for manual inspection
        // await driver.quit();
    }
})();
