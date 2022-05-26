const { Builder, Browser, By, Key, until } = require('selenium-webdriver');

(async function example() {
    let driver = new webdriver.Builder()
        .forBrowser(webdriver.Browser.FIREFOX)
        .usingServer('http://localhost:3000/signin')
        .build();
})();