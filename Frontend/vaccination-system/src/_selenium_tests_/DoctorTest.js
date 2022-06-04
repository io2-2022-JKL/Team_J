
const assert = require('assert')
const { Builder, By, Key, until } = require('selenium-webdriver');

async function doctorTest() {
    try {
        let driver = new Builder().forBrowser('chrome').build();
        await driver.get('http://localhost:3000/signin');

        driver.manage().setTimeouts({ implicit: 1000 })

        let emailForm = await driver.findElement(By.name('email'));
        let passwordForm = await driver.findElement(By.name('password'));
        let submitButton = await driver.findElement(By.name('submitButton'));

        await emailForm.sendKeys('sylwesterS@doktor.org.pl');
        await passwordForm.sendKeys('-EV92QbHF$!8keH=');
        await submitButton.click();

        await driver.wait(until.urlIs("http://localhost:3000/doctor/redirection"));

        let doctorTab = await driver.findElement(By.name('doctorTab'));
        const actions = driver.actions({ async: true });
        await actions.move({ origin: doctorTab }).press().perform();
        //await doctorTab.click();

        let certificatesButton = await driver.findElement(By.name('certificatesButton'));
        await certificatesButton.click();

        let actualUrl = await driver.getCurrentUrl();
        let expectedUrl = "http://localhost:3000/doctor/incomingAppointments";
        assert.equal(actualUrl, expectedUrl);
        console.log("Doctor test passed")
        await driver.quit();
    } catch (error) {
        console.log(error)
        console.log("Doctor test did not pass")
    }
};
doctorTest()