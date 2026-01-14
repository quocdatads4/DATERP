const { Builder, By, Key, until } = require('selenium-webdriver');
const chrome = require('selenium-webdriver/chrome');

(async function verifyStudentExamPage() {
    let options = new chrome.Options();
    options.addArguments('--no-sandbox');
    options.addArguments('--disable-dev-shm-usage');
    options.addArguments('--disable-gpu');
    options.addArguments('--window-size=1280,800');
    // options.addArguments('--incognito'); // Optional

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
        console.log('=== STUDENT EXAM PAGE VERIFICATION ===');
        const username = 'student@datacademy.edu.vn';
        const password = 'Student@123';

        // 1. Login
        console.log('Step 1: Logging in...');
        await driver.get('http://localhost:5223/Account/Login');

        // Login if needed
        let currentUrl = await driver.getCurrentUrl();
        if (!currentUrl.includes('/student/dashboard') && !currentUrl.includes('/Examination/ExamSubjects')) {
            let usernameField = await driver.wait(until.elementLocated(By.css('input[name="LoginInput.UserNameOrEmailAddress"]')), 10000);
            await usernameField.sendKeys(username);
            let passwordField = await driver.findElement(By.id('passwordInput'));
            await passwordField.sendKeys(password);
            let loginButton = await driver.findElement(By.css('button[value="Login"]'));
            await loginButton.click();
            await driver.sleep(2000);
        }

        // 2. Click "Menuluyện thi"
        console.log('Step 2: Clicking "Luyện thi" menu...');
        // Wait for menu to be visible. The text is "Luyện thi" or icon "ti-test-pipe"
        // Using partial link text or finding by icon class parent
        let examMenuLink = await driver.wait(
            until.elementLocated(By.xpath("//a[contains(., 'Luyện thi')]")),
            10000
        );

        let linkHref = await examMenuLink.getAttribute('href');
        console.log(`Found menu link pointing to: ${linkHref}`);

        if (linkHref.includes('/Examination/ExamSubjects')) {
            console.log('SUCCESS: Menu link is correct.');
        } else {
            console.error(`FAILURE: Menu link is incorrect. Found: ${linkHref}`);
        }

        if (linkHref.includes('/Examination/ExamSubjects')) {
            console.log('SUCCESS: Menu link is correct.');
        } else {
            console.error(`FAILURE: Menu link is incorrect. Found: ${linkHref}`);
        }

        // Revert to standard Click to verify CSS fix
        await examMenuLink.click();

        // 3. Verify Redirection
        console.log('Step 3: Verifying redirection...');
        await driver.wait(until.urlContains('/Examination/ExamSubjects'), 10000);
        console.log('SUCCESS: Redirected to Exam Subjects page.');

        // 4. Verify Student UI Elements
        console.log('Step 4: Verifying Student UI elements...');
        // Hero Section
        let heroSection = await driver.findElements(By.css('.card.mb-5.border-0')); // Approximate class
        if (heroSection.length > 0) {
            console.log('SUCCESS: Hero section found.');
        }

        // Subject Cards
        let cards = await driver.findElements(By.css('.subject-card'));
        if (cards.length > 0) {
            console.log(`SUCCESS: Found ${cards.length} subject cards.`);
            // Verify content of first card
            let firstCardText = await cards[0].getText();
            console.log('First card content preview: ' + firstCardText.substring(0, 50) + '...');
        } else {
            console.error('FAILURE: No subject cards found! (Is data seeded?)');
            // Check for "No data" message
            let noData = await driver.findElements(By.xpath("//*[contains(text(), 'Chưa có dữ liệu')]"));
            if (noData.length > 0) {
                console.warn('WARNING: "No data" message displayed. UI is working, but no data.');
            }
        }

    } catch (e) {
        console.error('--- ERROR ---');
        console.error(e);
        // Take screenshot if possible (not implementing here for simplicity, but logging source)
    } finally {
        console.log('Verification finished.');
        // await driver.quit(); // Keep open for manual review if user watches
    }
})();
