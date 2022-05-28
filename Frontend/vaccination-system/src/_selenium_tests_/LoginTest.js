
const assert = require('assert')
const { Builder, By, Key, until } = require('selenium-webdriver');
//const driver = new Builder().forBrowser('firefox').build();

async function patientLoginTest() {
  try {
    let driver = new Builder().forBrowser('firefox').build();
    await driver.get('http://localhost:3000/signin');

    //console.log(await driver.getTitle());

    driver.manage().setTimeouts({ implicit: 1000 })

    let emailForm = await driver.findElement(By.name('email'));
    let passwordForm = await driver.findElement(By.name('password'));
    let submitButton = await driver.findElement(By.name('submitButton'));


    await emailForm.sendKeys('adi222@wp.pl');
    await passwordForm.sendKeys('haslohaslo');
    await submitButton.click();

    let expectedUrl = "http://localhost:3000/patient";
    await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony
    
    let actualUrl = await driver.getCurrentUrl();
    //console.log(await driver.getTitle())
    assert.equal(actualUrl, expectedUrl);
    console.log("Patient login test passed")
    await driver.quit();
  } catch (error) {
    console.log(error)
    console.log("Patient login test did not pass")
  }
};

async function adminLoginTest() {
  try {
    let driver = new Builder().forBrowser('chrome').build();
    
    await driver.get('http://localhost:3000/signin');

    //console.log(await driver.getTitle());

    driver.manage().setTimeouts({ implicit: 1000 })

    let emailForm = await driver.findElement(By.name('email'));
    let passwordForm = await driver.findElement(By.name('password'));
    let submitButton = await driver.findElement(By.name('submitButton'));


    await emailForm.sendKeys('admin@systemszczepien.org.pl');
    await passwordForm.sendKeys('1234');
    await submitButton.click();

    let expectedUrl = "http://localhost:3000/admin";
    await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony
    
    let actualUrl = await driver.getCurrentUrl();
    //console.log(await driver.getTitle())
    assert.equal(actualUrl, expectedUrl);
    console.log("Admin login test passed")
    await driver.quit();
  } catch (error) {
    console.log(error)
    console.log("Admin login test did not pass")
  }
}
patientLoginTest();
adminLoginTest();
