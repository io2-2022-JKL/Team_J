//SeleniumTests

const { Builder } = require('selenium-webdriver');
const firefox = require('selenium-webdriver/firefox');

(async function openFirefoxTest() {
    try {
        let options = new firefox.Options();
        let driver = await new Builder()
            .setChromeOptions(options)
            .forBrowser('firefox')
            .build();
        await driver.get('https://www.google.com');
        let promise = driver.getTitle();
        promise.then(function (title) {
            console.log(title)
        })
        await driver.quit();
    } catch (error) {
        console.log(error)
    }
})();