
const assert = require('assert')
const { Builder, By, Key, until } = require('selenium-webdriver');

async function patientLoginTest() {
  try {
    let driver = new Builder().forBrowser('firefox').build();
    await driver.get('http://localhost:3000/signin');

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
    assert.equal(actualUrl, expectedUrl);
    console.log("Patient login test passed")
    await driver.quit();
  } catch (error) {
    console.log(error)
    console.log("Patient login test did not pass")
    await driver.quit();
  }
};

async function doctorLoginTest() {
  try {
    let driver = new Builder().forBrowser('firefox').build();
    await driver.get('http://localhost:3000/signin');

    driver.manage().setTimeouts({ implicit: 1000 })

    let emailForm = await driver.findElement(By.name('email'));
    let passwordForm = await driver.findElement(By.name('password'));
    let submitButton = await driver.findElement(By.name('submitButton'));


    await emailForm.sendKeys('sylwesterS@doktor.org.pl');
    await passwordForm.sendKeys('-EV92QbHF$!8keH=');
    await submitButton.click();

    let expectedUrl = "http://localhost:3000/doctor/redirection";
    await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony

    let actualUrl = await driver.getCurrentUrl();
    assert.equal(actualUrl, expectedUrl);
    console.log("Doctor login test passed")
    await driver.quit();
  } catch (error) {
    console.log(error)
    console.log("Doctor login test did not pass")
    await driver.quit();
  }
};

async function adminLoginTest() {
  try {
    let driver = new Builder().forBrowser('chrome').build();

    await driver.get('http://localhost:3000/signin');

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
    assert.equal(actualUrl, expectedUrl);
    console.log("Admin login test passed")
    await driver.quit();

  } catch (error) {
    console.log(error)
    console.log("Admin login test did not pass")
    await driver.quit();
  }
}

async function RegisterTest() {
  try {
    let driver = new Builder().forBrowser('chrome').build();

    await driver.get('http://localhost:3000/signin');

    driver.manage().setTimeouts({ implicit: 1000 })

    let link = await driver.findElement(By.name('link'));
    await link.click();

    let expectedUrl = "http://localhost:3000/register";
    await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony

    let actualUrl = await driver.getCurrentUrl();
    assert.equal(actualUrl, expectedUrl);

    let peselForm = await driver.findElement(By.name('PESEL'));
    let firstNameForm = await driver.findElement(By.name('firstName'));
    let lastNameForm = await driver.findElement(By.name('lastName'));
    let emailForm = await driver.findElement(By.name('mail'));
    let passwordForm = await driver.findElement(By.name('password'));
    let password2Form = await driver.findElement(By.name('password2'));
    let dateOfBirthForm = await driver.findElement(By.name('dateOfBirth'));
    let phoneNumberForm = await driver.findElement(By.name('phoneNumber'));
    let registerButton = await driver.findElement(By.name('registerButton'));

    await peselForm.sendKeys('99691208457');
    await firstNameForm.sendKeys('Grzegorz');
    await lastNameForm.sendKeys('Brzęczyszczykiewicz');
    await emailForm.sendKeys('GB3@onet.pl');
    await passwordForm.sendKeys('gb1234');
    await password2Form.sendKeys('gb1234');
    await dateOfBirthForm.sendKeys('01-01-1939');
    await phoneNumberForm.sendKeys('801802803');
    await registerButton.click();
    
    expectedUrl = "http://localhost:3000/signin";
    await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony
    
    let snackbar = await driver.findElement(By.name('snackbar'));
    let pom = await snackbar.isDisplayed();
    assert.equal(pom,true)

    console.log("Register test passed")
    await driver.quit();
  } catch (error) {
    console.log(error)
    console.log("Register test did not pass")
    await driver.quit();
  }
}
RegisterTest()
patientLoginTest();
doctorLoginTest();
adminLoginTest();