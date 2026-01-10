const { Builder, By, Key, until, logging } = require('selenium-webdriver');
const chrome = require('selenium-webdriver/chrome');

(async function example() {
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
        await driver.get('http://localhost:5223/Account/Login?culture=vi&ReturnUrl=%2FIdentity%2FUsers');

        // Check if already logged in
        let currentUrl = await driver.getCurrentUrl();
        if (currentUrl.includes('Identity/Users')) {
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

        console.log('Waiting for identity management page...');
        await driver.wait(until.urlContains('Identity/Users'), 15000);

        console.log('Checking for page errors...');
        await driver.sleep(3000);
        let pageSource = await driver.getPageSource();

        if (pageSource.includes("An unhandled exception") || pageSource.includes("Internal Server Error")) {
            console.error('FAILURE: Server error detected on page!');
        } else {
            let table = await driver.findElements(By.css('table'));
            if (table.length > 0) {
                console.log(`SUCCESS: Found ${table.length} table(s). UI load verify OK.`);
            } else {
                console.warn('WARNING: No tables found on Identity/Users page.');
            }

            if (pageSource.includes("Users") || pageSource.includes("người dùng")) {
                console.log('SUCCESS: Page content verification passed.');
            }
        }

    } catch (e) {
        console.error('Automation Error:', e.message);
    } finally {
        console.log('Verification process finished.');
    }
})();
