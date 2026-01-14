const { Builder, By, Key, until } = require('selenium-webdriver');
const chrome = require('selenium-webdriver/chrome');

(async function verifyStudentExamList() {
    let options = new chrome.Options();
    options.addArguments('--no-sandbox');
    options.addArguments('--disable-dev-shm-usage');
    options.addArguments('--disable-gpu');
    options.addArguments('--window-size=1280,800');
    options.addArguments('--incognito'); // Private browsing mode

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
        console.log('=== STUDENT EXAM LIST VERIFICATION ===');
        const username = 'student@datacademy.edu.vn';
        const password = 'Student@123';

        // 1. Login
        console.log('Step 1: Logging in...');
        await driver.get('http://localhost:5223/Account/Login');

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

        // 2. Navigate to Exam Subjects
        console.log('Step 2: Navigating to Exam Subjects...');
        await driver.get('http://localhost:5223/Examination/ExamSubjects');
        await driver.wait(until.elementLocated(By.css('.subject-card')), 10000);

        // 3. Click "Start Practice" for Word 2019
        console.log('Step 3: Clicking "Start Practice" button for Word 2019...');
        // Find the button with href containing subjectCode=WORD2019
        let accessBtns = await driver.findElements(By.xpath("//a[contains(@href, 'subjectCode=WORD2019')]"));

        if (accessBtns.length > 0) {
            console.log(`Found ${accessBtns.length} Word 2019 access buttons.`);
            let firstBtn = accessBtns[0];
            let href = await firstBtn.getAttribute('href');
            console.log(`Clicking button linking to: ${href}`);
            // Scroll into view
            await driver.executeScript("arguments[0].scrollIntoView({block: 'center'});", firstBtn);
            await driver.sleep(500);
            // Use JS click for reliability
            await driver.executeScript("arguments[0].click();", firstBtn);
        } else {
            console.error('FAILURE: No "Start Practice" button found for Word 2019.');
            return;
        }

        // 4. Verify Redirection and Exam Lists UI
        console.log('Step 4: Verifying Exam Lists Page...');
        await driver.wait(until.urlContains('/Examination/ExamLists'), 10000);
        console.log('SUCCESS: Redirected to Exam Lists page.');

        // Verify Subject Hero Section
        let heroTitle = await driver.findElement(By.css('.subject-header-card h2'));
        console.log('Found Subject Title: ' + await heroTitle.getText());

        // Statistics panel has been removed - skip Start Learning button check

        // Verify Exam List Items
        let listItems = await driver.findElements(By.css('.col-12 .card.border'));
        if (listItems.length > 0) {
            console.log(`SUCCESS: Found ${listItems.length} exam list items.`);
            let firstItemTitle = await listItems[0].findElement(By.css('h5.card-title'));
            console.log('First Exam Title: ' + await firstItemTitle.getText());
        } else {
            console.warn('WARNING: No exam list items found on the page (Subject might be empty).');
            // Check for "No Exams" message
            let noExams = await driver.findElements(By.xpath("//*[contains(text(), 'Chưa có bài thi') or contains(text(), 'No Exams Found')]"));
            if (noExams.length > 0) console.log('SUCCESS: "No Exams" message displayed correctly.');
        }

    } catch (e) {
        console.error('--- ERROR ---');
        console.error(e);
    } finally {
        console.log('Verification finished.');
    }
})();
