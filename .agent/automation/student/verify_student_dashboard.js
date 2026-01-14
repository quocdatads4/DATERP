const { Builder, By, Key, until } = require('selenium-webdriver');
const chrome = require('selenium-webdriver/chrome');

/**
 * Verify Student Dashboard Automation Script
 * Đăng nhập với tài khoản Student và kiểm tra:
 * 1. Đăng nhập thành công
 * 2. Redirect đến Student Dashboard
 * 3. Kiểm tra menu Student hiển thị đúng
 * 4. Kiểm tra nội dung trang Dashboard
 */
(async function verifyStudentDashboard() {
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
        console.error('FAILED to start Chrome driver. Make sure Chrome is installed.');
        console.error(err);
        return;
    }

    try {
        console.log('=== STUDENT DASHBOARD VERIFICATION ===');
        console.log('');

        // Credentials for Student
        const username = 'student@datacademy.edu.vn';
        const password = 'Student@123';

        // 1. Navigate to login page
        console.log('Step 1: Navigating to login page...');
        await driver.get('http://localhost:5223/Account/Login?culture=vi&ReturnUrl=%2Fstudent%2Fdashboard');
        await driver.sleep(2000);

        // 2. Check if already logged in
        let currentUrl = await driver.getCurrentUrl();
        if (currentUrl.includes('/student/dashboard')) {
            console.log('Already logged in, redirected to dashboard.');
        } else {
            // 3. Enter credentials
            console.log('Step 2: Entering Student credentials...');
            let usernameField = await driver.wait(
                until.elementLocated(By.css('input[name="LoginInput.UserNameOrEmailAddress"]')),
                15000
            );
            await usernameField.clear();
            await usernameField.sendKeys(username);

            let passwordField = await driver.wait(
                until.elementLocated(By.id('passwordInput')),
                5000
            );
            await passwordField.sendKeys(Key.CONTROL, "a", Key.DELETE);
            await passwordField.sendKeys(password);

            // 4. Click login button
            console.log('Step 3: Clicking login button...');
            let loginButton = await driver.findElement(By.css('button[value="Login"]'));
            await driver.executeScript("arguments[0].click();", loginButton);

            // 5. Wait for redirect
            console.log('Step 4: Waiting for redirect to Student Dashboard...');
            await driver.wait(until.urlContains('/student/dashboard'), 20000);
        }

        console.log('SUCCESS: Logged in and redirected to Student Dashboard.');
        await driver.sleep(3000);

        // 6. Verify page content
        console.log('');
        console.log('Step 5: Verifying page content...');

        let pageSource = await driver.getPageSource();

        // Check for errors
        if (pageSource.includes("An unhandled exception") || pageSource.includes("Internal Server Error")) {
            console.error('FAILURE: Server error detected on page!');
        } else {
            console.log('SUCCESS: No server errors.');
        }

        // Check for Student Dashboard content
        if (pageSource.includes('DATMOS Learning Platform') || pageSource.includes('Student Dashboard')) {
            console.log('SUCCESS: Student Dashboard content found.');
        } else {
            console.warn('WARNING: Student Dashboard content not found.');
        }

        // Check for course cards
        if (pageSource.includes('Microsoft Word 2019') && pageSource.includes('Microsoft Excel 2019')) {
            console.log('SUCCESS: Course cards found (Word 2019, Excel 2019).');
        }

        // 7. Verify Student Menu
        console.log('');
        console.log('Step 6: Verifying Student Menu...');

        // Check for Student-specific menu items
        let menuItems = await driver.findElements(By.css('.navbar-nav .nav-item .nav-link'));
        let menuTexts = [];
        for (let item of menuItems) {
            let text = await item.getText();
            if (text.trim()) menuTexts.push(text.trim());
        }
        console.log('Menu items found: ' + menuTexts.join(', '));

        // Check for expected Student menu items
        const expectedStudentMenu = ['Trang chủ', 'Khóa học', 'Luyện thi'];
        const unexpectedAdminMenu = ['Bảng điều khiển', 'Quản lý hệ thống', 'Cấu hình'];

        let hasStudentMenu = expectedStudentMenu.some(item => menuTexts.includes(item));
        let hasAdminMenu = unexpectedAdminMenu.some(item => menuTexts.includes(item));

        if (hasStudentMenu && !hasAdminMenu) {
            console.log('SUCCESS: Student menu is displayed correctly.');
        } else if (hasAdminMenu) {
            console.error('FAILURE: Admin menu is showing instead of Student menu!');
            console.log('This indicates the logged-in user has Admin role, not Student role.');
        } else {
            console.warn('WARNING: Could not verify menu items.');
        }

        // 8. Verify user role in dropdown
        console.log('');
        console.log('Step 7: Verifying user role...');

        let userDropdown = await driver.findElement(By.css('.dropdown-user .nav-link'));
        await userDropdown.click();
        await driver.sleep(500);

        let dropdownContent = await driver.findElement(By.css('.dropdown-user .dropdown-menu'));
        let roleText = await dropdownContent.getText();

        if (roleText.includes('Student') || roleText.includes('Học viên')) {
            console.log('SUCCESS: User is logged in as Student.');
        } else if (roleText.includes('Admin')) {
            console.error('FAILURE: User is logged in as Admin, not Student!');
        } else {
            console.log('User dropdown content: ' + roleText.substring(0, 100));
        }

        console.log('');
        console.log('=== VERIFICATION COMPLETE ===');

    } catch (e) {
        console.error('--- ERROR DURING AUTOMATION ---');
        console.error(e.message);
    } finally {
        console.log('');
        console.log('Automation finished. Browser remains open for manual review.');
        // driver.quit() omitted to allow manual inspection
    }
})();
