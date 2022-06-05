
const assert = require('assert')

const { Builder, By, Key, until } = require('selenium-webdriver');

async function login(driver, email, password, expectedUrl) {

  let emailForm = await driver.findElement(By.name('email'));
  let passwordForm = await driver.findElement(By.name('password'));
  let submitButton = await driver.findElement(By.name('submitButton'));


  await emailForm.sendKeys(email);
  await passwordForm.sendKeys(password);
  await submitButton.click();

  await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony

  let actualUrl = await driver.getCurrentUrl();
  assert.equal(actualUrl, expectedUrl);
}

async function register(driver, PESEL, firstName, lastName, email, password, dateOfBirth, phoneNumber) {
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

  await peselForm.sendKeys(PESEL);
  await firstNameForm.sendKeys(firstName);
  await lastNameForm.sendKeys(lastName);
  await emailForm.sendKeys(email);
  await passwordForm.sendKeys(password);
  await password2Form.sendKeys(password);
  await dateOfBirthForm.sendKeys(dateOfBirth);
  await phoneNumberForm.sendKeys(phoneNumber);
  await registerButton.click();

  expectedUrl = "http://localhost:3000/signin";
  await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony

  let snackbar = await driver.findElement(By.name('snackbar'));
  let pom = await snackbar.isDisplayed();
  assert.equal(pom, true)
}

async function patientLoginTest(email, password) {
  try {
    let driver = new Builder().forBrowser('firefox').build();
    await driver.get('http://localhost:3000/signin');
    driver.manage().setTimeouts({ implicit: 1000 })

    await login(driver, email, password, "http://localhost:3000/patient")

    console.log("Patient login test passed")
    await driver.quit();
  } catch (error) {
    console.log(error)
    console.log("Patient login test did not pass")
    await driver.quit();
  }
};

async function doctorLoginTest(email, password) {
  try {
    let driver = new Builder().forBrowser('firefox').build();
    await driver.get('http://localhost:3000/signin');
    driver.manage().setTimeouts({ implicit: 1000 })

    await login(driver, email, password, "http://localhost:3000/doctor/redirection")

    console.log("Doctor login test passed")
    await driver.quit();
  } catch (error) {
    console.log(error)
    console.log("Doctor login test did not pass")
    await driver.quit();
  }
};

async function adminLoginTest(email, password) {
  try {
    let driver = new Builder().forBrowser('chrome').build();
    await driver.get('http://localhost:3000/signin');
    driver.manage().setTimeouts({ implicit: 1000 })

    await login(driver, email, password, "http://localhost:3000/admin")

    console.log("Admin login test passed")
    await driver.quit();

  } catch (error) {
    console.log(error)
    console.log("Admin login test did not pass")
    await driver.quit();
  }
}

async function RegisterTest(PESEL, firstName, lastName, email, password, dateOfBirth, phoneNumber) {
  try {
    let driver = new Builder().forBrowser('chrome').build();

    await register(driver, PESEL, firstName, lastName, email, password, dateOfBirth, phoneNumber);

    console.log("Register test passed")
    await driver.quit();
  } catch (error) {
    console.log(error)
    console.log("Register test did not pass")
    await driver.quit();
  }
}

async function MakeNewDoctorTest(PESEL, firstName, lastName, email, password, dateOfBirth, phoneNumber,adminEmail,adminPassword) {
  try {
    let driver = new Builder().forBrowser('chrome').build();
    
    //await driver.get('http://localhost:3000/signin');
    //driver.manage().setTimeouts({ implicit: 1000 })
    await register(driver, PESEL, firstName, lastName, email, password, dateOfBirth, phoneNumber);
    console.log("Registered new patient");

    await login(driver, adminEmail, adminPassword, "http://localhost:3000/admin")
    console.log("Logged as admin");

    let manageDoctorsButton = await driver.findElement(By.name("manageDoctorsButton"));
    await manageDoctorsButton.click();

    let expectedUrl = "http://localhost:3000/admin/doctors";
    await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony

    let addDoctorButton= await driver.findElement(By.name("addDoctorButton"));
    await addDoctorButton.click();

    expectedUrl = "http://localhost:3000/admin/patients";
    await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony

    let filter = await driver.findElement(By.name("peselFilter"));
    await filter.sendKeys(PESEL);

    let filter2 = await driver.findElement(By.name("firstNameFilter"));
    await filter2.sendKeys(firstName);
    
    let row = await driver.findElement(By.xpath("//*[text()='"+firstName+"']"));
    console.log("Found patient " + firstName + ' ' + lastName);
    await row.click();

    let selectBox = await driver.findElement(By.name("centerSelection"));
    let choose = await driver.findElement(By.name("chooseButton"));

    await selectBox.click();

    let element = await driver.findElement(By.xpath("//*[@id='centerSelection']/option[2]"));
    let text = await element.getText();
    
    assert.equal(text,"Szpital Kliniczny im. Heliodora Święcickiego Uniwersytetu Medycznego, Poznań");
    console.log("Chose "+ text + " as VaccinationCenter");
    await element.click();
    await choose.click();
    
    await driver.findElement(By.xpath("//*[text()='Pomyślnie wykonano akcję: Dodano nowego lekarza']"));
    
    let back = await driver.findElement(By.name("backButton"));
    await back.click();
    
    expectedUrl = "http://localhost:3000/admin";
    await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony
    
    manageDoctorsButton = await driver.findElement(By.name("manageDoctorsButton"));
    await manageDoctorsButton.click();
    
    expectedUrl = "http://localhost:3000/admin/doctors";
    await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony
    
    filter = await driver.findElement(By.name("peselFilter"));
    await filter.sendKeys(PESEL);

    filter2 = await driver.findElement(By.name("firstNameFilter"));
    await filter2.sendKeys(firstName);
    
    await driver.findElement(By.xpath("//*[text()='"+PESEL+"']"));
    console.log("Found doctor: "+firstName+' '+lastName);

    console.log("Make new doctor test passed")
    await driver.quit();
  } catch (error) {
    console.log(error)
    console.log("Make new doctor test did not pass")
    await driver.quit();
  }
}
//RegisterTest('99691208457','Grzegorz','Brzęczyszczykiewicz','GB3@onet.pl','gb1234','01-01-1939','801802803')
patientLoginTest('adi222@wp.pl', 'haslohaslo');
doctorLoginTest('sylwesterS@doktor.org.pl', '-EV92QbHF$!8keH=');
adminLoginTest('admin@systemszczepien.org.pl', '1234');
MakeNewDoctorTest('11111111117','Jaga','Brzęczyszczykiewiczówna','JB2@onet.pl','jb21234','01-01-1946','801802809','admin@systemszczepien.org.pl', '1234');