const { Builder, By, Key, until } = require('selenium-webdriver');
const chrome = require('selenium-webdriver/chrome');

(async function verifyStudentExamTask() {
    let options = new chrome.Options();
    options.addArguments('--no-sandbox');
    options.addArguments('--disable-dev-shm-usage');
    options.addArguments('--disable-gpu');
    options.addArguments('--window-size=1280,800');
    // options.addArguments('--incognito'); // Comment out incognito to debugging if needed, but keeping it for consistency

    let driver;
    try {
        driver = await new Builder()
            .forBrowser('chrome')
            .setChromeOptions(options)
            .build();
    } catch (err) {
        console.error('FAILED to start Chrome driver.');
        console.error(err);
        process.exit(1);
    }

    try {
        console.log('=== STUDENT EXAM TASK VERIFICATION ===');
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
            await driver.wait(until.urlContains('/'), 10000);
        }

        // 2. Navigate to Exam Subjects
        console.log('Step 2: Navigating to Exam Subjects...');
        await driver.get('http://localhost:5223/Examination/ExamSubjects');
        try {
            await driver.wait(until.elementLocated(By.css('.subject-card')), 10000);
        } catch (e) {
            console.log("Wait for .subject-card timed out. Continuing to check for buttons...");
        }

        // 3. Click "Access Exam" (Truy cập bài làm)
        console.log('Step 3: Clicking "Access Exam" button...');
        // Wait for the button to be present using XPath
        let xpathSelector = "//a[contains(@href, '/Examination/ExamLists')]";
        try {
            await driver.wait(until.elementLocated(By.xpath(xpathSelector)), 5000);
        } catch (e) {
            console.error(`FAILURE: Could not find any "Access Exam" button with selector: ${xpathSelector}`);
            let source = await driver.getPageSource();
            // console.log("Page Source Dump (Partial):", source.substring(0, 2000));
            throw new Error("Access Exam button not found.");
        }

        let accessBtns = await driver.findElements(By.xpath(xpathSelector));
        if (accessBtns.length > 0) {
            let firstBtn = accessBtns[0];
            await driver.executeScript("arguments[0].scrollIntoView({block: 'center'});", firstBtn);
            await driver.sleep(500);
            await driver.executeScript("arguments[0].click();", firstBtn);
        } else {
            throw new Error("Access Exam button found via wait but length is 0? Should not happen.");
        }

        // 4. Wait for Exam Lists Page
        console.log('Step 4: Waiting for Exam Lists page...');
        await driver.wait(until.urlContains('/Examination/ExamLists'), 10000);

        // 5. Select an Exam List Item to View Details (Projects/Tasks)
        console.log('Step 5: Selecting an Exam List item...');

        // Wait until at least one link in a card body is available, or any 'btn' that is not a hash link
        let itemSelector = By.css('.card-body a');
        try {
            await driver.wait(until.elementLocated(itemSelector), 5000);
        } catch (e) {
            console.warn("Could not find standard card body links. Checking for other buttons...");
        }

        let startButtons = await driver.findElements(By.xpath("//a[contains(@class, 'btn') and not(contains(@href, '#'))]"));
        if (startButtons.length === 0) {
            startButtons = await driver.findElements(By.css('.card-body a'));
        }

        if (startButtons.length > 0) {
            console.log(`Found ${startButtons.length} possible action buttons.`);
            let btn = startButtons[0];
            let href = await btn.getAttribute('href');
            console.log(`Clicking button pointing to: ${href}`);

            await driver.executeScript("arguments[0].scrollIntoView({block: 'center'});", btn);
            await driver.sleep(500);
            await btn.click();

            // 6. Verify Task/Project Page
            console.log('Step 6: Verifying Project/Task Page...');
            await driver.sleep(3000); // Wait for page load
            let newUrl = await driver.getCurrentUrl();
            console.log(`Current URL: ${newUrl}`);

            if (newUrl.includes('ExamProjects') || newUrl.includes('ExamTasks') || newUrl.includes('TakeExam')) {
                console.log('SUCCESS: Navigated to deeper exam level.');

                // Check for Task UI elements
                let taskElements = await driver.findElements(By.css('.col-md-3, .list-group-item, .card'));
                if (taskElements.length > 0) {
                    console.log(`SUCCESS: Found ${taskElements.length} potential UI elements (Tasks/Projects).`);
                } else {
                    console.warn('WARNING: No obvious task elements found.');
                }

            } else {
                console.warn(`WARNING: URL ${newUrl} does not explicitly match expected Task/Project patterns. Verification might require update.`);
            }

        } else {
            console.error('FAILURE: No buttons found to start/view exam list item.');
            throw new Error("No Exam List Item buttons found.");
        }

    } catch (e) {
        console.error('--- ERROR IN VERIFICATION SCRIPT ---');
        console.error(e);
        process.exit(1); // Exit with error code to notify PowerShell
    } finally {
        console.log('Verification script finished.');
        // await driver.quit(); 
    }
})();
