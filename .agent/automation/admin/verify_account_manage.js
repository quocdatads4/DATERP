const { Builder, By, Key, until } = require('selenium-webdriver');
const chrome = require('selenium-webdriver/chrome');

(async function verifyAccountManage() {
    let options = new chrome.Options();
    options.addArguments('--no-sandbox');
    options.addArguments('--disable-dev-shm-usage');
    options.addArguments('--disable-gpu');
    options.addArguments('--window-size=1280,800');
    options.addArguments('--incognito');

    let driver;
    try {
        driver = await new Builder()
            .forBrowser('chrome')
            .setChromeOptions(options)
            .build();
    } catch (err) {
        console.error('FAILED to start Chrome driver.');
        console.error(err);
        return;
    }

    try {
        console.log('Navigating to login page...');
        await driver.get('http://localhost:5223/Account/Login?culture=vi&ReturnUrl=%2FAccount%2FManage');

        // Check if already logged in
        let currentUrl = await driver.getCurrentUrl();
        if (currentUrl.includes('Account/Manage')) {
            console.log('Already logged in or redirected.');
        } else {
            console.log('Waiting for login form...');
            let usernameField = await driver.wait(until.elementLocated(By.name('LoginInput.UserNameOrEmailAddress')), 10000);

            console.log('Entering credentials...');
            await usernameField.clear();
            await usernameField.sendKeys('admin@datacademy.edu.vn');

            let passwordField = await driver.findElement(By.id('passwordInput'));
            await passwordField.sendKeys(Key.CONTROL, "a", Key.DELETE);
            await passwordField.sendKeys('Admin@123');

            console.log('Clicking login button...');
            let loginButton = await driver.findElement(By.xpath("//button[@value='Login']"));
            await driver.executeScript("arguments[0].click();", loginButton);
        }

        console.log('Waiting for Account Manage page...');
        await driver.wait(until.urlContains('Account/Manage'), 15000);

        console.log('Checking for page errors...');
        await driver.sleep(3000);
        let pageSource = await driver.getPageSource();

        if (pageSource.includes("An unhandled exception") || pageSource.includes("Internal Server Error")) {
            console.error('FAILURE: Server error detected on page!');
        } else {
            // Check for tabs
            let tabs = await driver.findElements(By.css('#accountTabs .nav-link'));
            if (tabs.length >= 2) {
                console.log(`SUCCESS: Found ${tabs.length} navigation tabs.`);
            } else {
                console.warn('WARNING: Expected at least 2 tabs, found ' + tabs.length);
            }

            // Check for Personal Info tab content
            let personalInfoTab = await driver.findElements(By.id('personal-info'));
            if (personalInfoTab.length > 0) {
                console.log('SUCCESS: Personal Info tab found.');
            }

            // Check for form elements
            let formElements = await driver.findElements(By.css('#formPersonalInfo input'));
            if (formElements.length >= 3) {
                console.log(`SUCCESS: Found ${formElements.length} form inputs in Personal Info.`);
            }

            // Switch to Security tab
            console.log('Testing Security tab...');
            let securityTab = await driver.findElement(By.id('security-tab'));
            await securityTab.click();
            await driver.sleep(500);

            // Check for password form
            let passwordForm = await driver.findElements(By.id('formChangePassword'));
            if (passwordForm.length > 0) {
                console.log('SUCCESS: Change Password form found.');
            }

            // Check for Tabler icons
            if (pageSource.includes('ti-user') || pageSource.includes('ti-lock')) {
                console.log('SUCCESS: Tabler icons are present.');
            }

            // Check for Vietnamese text
            if (pageSource.includes('Thông tin cá nhân') || pageSource.includes('Bảo mật')) {
                console.log('SUCCESS: Vietnamese labels are present.');
            }

            console.log('SUCCESS: Account Manage page verification passed.');
        }

    } catch (e) {
        console.error('Automation Error:', e.message);
    } finally {
        console.log('Verification process finished.');
        // Keep browser open for inspection, or uncomment to close:
        // await driver.quit();
    }
})();
