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

                // Verify Create Modal
                console.log('Verifying Create Modal...');
                let createBtn = await driver.findElement(By.css('button[name="CreateUser"]'));
                await createBtn.click();

                await driver.wait(until.elementLocated(By.className('modal-dialog')), 5000);
                await driver.sleep(1000); // Wait for animation

                let modalContent = await driver.findElement(By.className('modal-body')).getAttribute('innerHTML');
                // Check for Tabler icon class as primary indicator of custom content
                if (modalContent.includes('ti-lock') || modalContent.includes('Thông tin')) {
                    console.log('SUCCESS: Custom Create Modal loaded with Tabler icons.');
                } else {
                    console.error('FAILURE: Create Modal did NOT show expected custom content.');
                }

                // Close modal
                let cancelBtn = await driver.findElement(By.css('.modal-footer .btn-secondary'));
                await cancelBtn.click();
                await driver.sleep(500);

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
