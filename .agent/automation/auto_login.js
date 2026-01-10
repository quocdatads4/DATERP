const { Builder, By, Key, until } = require('selenium-webdriver');
const chrome = require('selenium-webdriver/chrome');

/**
 * Auto Login Automation Script
 * Updated for ABP Framework 10 Login page with Education theme
 * 
 * Selectors based on Login.cshtml:
 * - Username: asp-for="LoginInput.UserNameOrEmailAddress" -> id="LoginInput_UserNameOrEmailAddress"
 * - Password: id="passwordInput" (custom id)
 * - Login Button: abp-button with value="Login"
 */
(async function autoLogin() {
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
        console.error('FAILED to start Chrome driver. Make sure Chrome is installed and chromedriver matches your version.');
        console.error(err);
        return;
    }

    try {
        console.log('--- STARTING AUTO LOGIN ---');

        // Get role from command line arguments (admin|teacher|student)
        const roleArg = process.argv[2] || 'admin';
        const role = roleArg.toLowerCase();

        // Credentials mapping based on Memory Bank
        const credentials = {
            admin: { u: 'admin@datacademy.edu.vn', p: 'Admin@123' },
            teacher: { u: 'teacher@datacademy.edu.vn', p: 'Teacher@123' },
            student: { u: 'student@datacademy.edu.vn', p: 'Student@123' }
        };

        if (!credentials[role]) {
            console.error(`ERROR: Unknown role '${roleArg}'. Supported: admin, teacher, student.`);
            await driver.quit();
            return;
        }

        const { u, p } = credentials[role];
        console.log(`--- AUTO LOGIN FOR ROLE: ${role.toUpperCase()} ---`);

        // Login URL with culture=vi
        const loginUrl = 'http://localhost:5223/Account/Login?culture=vi';
        console.log(`Navigating to: ${loginUrl}`);
        await driver.get(loginUrl);

        // Wait for page load
        await driver.sleep(2000);

        console.log('Waiting for username field...');
        let usernameField = await driver.wait(
            until.elementLocated(By.css('input[name="LoginInput.UserNameOrEmailAddress"]')),
            15000
        );
        await driver.wait(until.elementIsVisible(usernameField), 5000);

        console.log(`Entering username: ${u}`);
        await usernameField.clear();
        await usernameField.sendKeys(u);

        console.log('Waiting for password field...');
        let passwordField = await driver.wait(
            until.elementLocated(By.id('passwordInput')),
            5000
        );
        await driver.wait(until.elementIsVisible(passwordField), 5000);

        console.log('Entering password...');
        await passwordField.sendKeys(Key.CONTROL, "a", Key.DELETE);
        await passwordField.sendKeys(p);

        console.log('Looking for login button...');
        // ABP button renders as <button> with name="Action" value="Login"
        // Using multiple selector strategies for robustness
        let loginButton;
        try {
            // Try by value attribute first (ABP standard)
            loginButton = await driver.wait(
                until.elementLocated(By.css('button[value="Login"]')),
                5000
            );
        } catch {
            // Fallback: try by type submit within form
            loginButton = await driver.wait(
                until.elementLocated(By.css('form button[type="submit"]')),
                5000
            );
        }
        await driver.wait(until.elementIsEnabled(loginButton), 5000);

        console.log('Clicking login button...');
        // Using JavaScript click for robustness
        await driver.executeScript("arguments[0].click();", loginButton);

        console.log('Waiting for redirect to any dashboard or home...');
        try {
            await driver.wait(until.urlMatches(/dashboard|\/$/i), 20000);
            console.log(`SUCCESS: Redirected to ${await driver.getCurrentUrl()}.`);
        } catch (waitErr) {
            console.error('TIMEOUT waiting for redirect. Checking current state...');
            let currentUrl = await driver.getCurrentUrl();
            console.log(`Current URL: ${currentUrl}`);

            // Check for validation errors (ABP uses text-danger class)
            let errorElements = await driver.findElements(By.css('.text-danger'));
            for (let el of errorElements) {
                let text = await el.getText();
                if (text.trim()) {
                    console.error('Validation Error: ' + text);
                }
            }

            // Check for alert messages
            let alerts = await driver.findElements(By.css('.alert'));
            for (let alert of alerts) {
                let text = await alert.getText();
                if (text.trim()) {
                    console.error('Alert Message: ' + text);
                }
            }

            throw waitErr;
        }

        console.log('Login process COMPLETED successfully.');

        // Final verification
        await driver.sleep(2000);
        let finalUrl = await driver.getCurrentUrl();
        console.log(`Final URL: ${finalUrl}`);

        const pageTitle = await driver.getTitle();
        console.log(`Page Title: ${pageTitle}`);

    } catch (e) {
        console.error('--- ERROR DURING AUTOMATION ---');
        console.error(e.message);
    } finally {
        console.log('Automation finished. Browser remains open for review.');
        // driver.quit() omitted to allow manual inspection
    }
})();
