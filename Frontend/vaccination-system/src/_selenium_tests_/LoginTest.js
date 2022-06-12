
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

async function register(driver, PESEL, firstName, lastName, email, password, dateOfBirth, phoneNumber,url) {
  await driver.get(url);

  driver.manage().setTimeouts({ implicit: 1000 })

  let link = await driver.findElement(By.name('link'));
  await link.click();

  let expectedUrl = url+"/register";
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

  expectedUrl = url+"/signin";
  await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony

  let snackbar = await driver.findElement(By.name('snackbar'));
  let pom = await snackbar.isDisplayed();
  assert.equal(pom, true)
}

async function patientLoginTest(email, password,url) {
  try {
    let driver = new Builder().forBrowser('firefox').build();
    await driver.get(url);
    driver.manage().setTimeouts({ implicit: 1000 })

    await login(driver, email, password, url+"/patient")

    console.log("Patient login test passed")
    await driver.quit();
  } catch (error) {
    console.log(error)
    console.log("Patient login test did not pass")
    await driver.quit();
  }
};

async function doctorLoginTest(email, password,url) {
  try {
    let driver = new Builder().forBrowser('firefox').build();
    await driver.get(url);
    driver.manage().setTimeouts({ implicit: 1000 })

    await login(driver, email, password, url+"/doctor/redirection")

    console.log("Doctor login test passed")
    await driver.quit();
  } catch (error) {
    console.log(error)
    console.log("Doctor login test did not pass")
    await driver.quit();
  }
};

async function adminLoginTest(email, password,url) {
  try {
    let driver = new Builder().forBrowser('chrome').build();
    await driver.get(url);
    driver.manage().setTimeouts({ implicit: 1000 })

    await login(driver, email, password, url+"/admin")

    console.log("Admin login test passed")
    await driver.quit();

  } catch (error) {
    console.log(error)
    console.log("Admin login test did not pass")
    await driver.quit();
  }
}

async function RegisterTest(PESEL, firstName, lastName, email, password, dateOfBirth, phoneNumber,url) {
  try {
    let driver = new Builder().forBrowser('chrome').build();

    await register(driver, PESEL, firstName, lastName, email, password, dateOfBirth, phoneNumber,url);

    console.log("Register test passed")
    await driver.quit();
  } catch (error) {
    console.log(error)
    console.log("Register test did not pass")
    await driver.quit();
  }
}

async function MakeNewDoctorTest(PESEL, firstName, lastName, email, password, dateOfBirth, phoneNumber,adminEmail,adminPassword,url) {
  try {
    let driver = new Builder().forBrowser('chrome').build();
    
    //await driver.get('http://localhost:3000/signin');
    driver.manage().setTimeouts({ implicit: 3000 });
    await register(driver, PESEL, firstName, lastName, email, password, dateOfBirth, phoneNumber,url);
    console.log("Registered new patient");

    await login(driver, adminEmail, adminPassword, url+"/admin")
    console.log("Logged as admin");

    let manageDoctorsButton = await driver.findElement(By.name("manageDoctorsButton"));
    await manageDoctorsButton.click();

    let expectedUrl = url+"/admin/doctors";
    await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony

    let addDoctorButton= await driver.findElement(By.name("addDoctorButton"));
    await addDoctorButton.click();

    expectedUrl = url+"/admin/patients";
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
    
    expectedUrl = url+"/admin";
    await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony
    
    manageDoctorsButton = await driver.findElement(By.name("manageDoctorsButton"));
    await manageDoctorsButton.click();
    
    expectedUrl = url+"/admin/doctors";
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
async function RegisterForVaccinationTest(PESEL, firstName, lastName, email, password, dateOfBirth, phoneNumber,docotrEmail,doctorPassword,batchNumber,url) {
  try {
    let driver = new Builder().forBrowser('chrome').build();
    //await driver.get(url);
    driver.manage().setTimeouts({ implicit: 3000 });
    await register(driver, PESEL, firstName, lastName, email, password, dateOfBirth, phoneNumber,url);
    console.log("Registered new patient");

    await login(driver, email, password, url+"/patient");
    console.log("Logged as patient");

    let registerForVaccinationButton = await driver.findElement(By.name("registerForVaccinationButton"));
    await registerForVaccinationButton.click();

    let expectedUrl = url+"/patient/timeSlots";
    await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony

    let virusSelectionBox = await driver.findElement(By.id("idVirus"))
    await virusSelectionBox.click();
    
    let element = await driver.findElement(By.xpath("//*[@id='idVirus']/option[1]"));
    let text = await element.getText();
    
    assert.equal(text,"Koronawirus");
    console.log("Chose "+ text + " as virus");
    await element.click();

    let citySelectionBox = await driver.findElement(By.id("cityFilter"));
    await citySelectionBox.click();

    element = await driver.findElement(By.xpath("//*[@id='cityFilter']/option[4]"));
    text = await element.getText();
    
    assert.equal(text,"Warszawa");
    console.log("Chose "+ text + " as city");
    await element.click();

    let dateFrom = await driver.findElement(By.id("dateFrom"));
    await dateFrom.sendKeys(Key.BACK_SPACE);
    await dateFrom.sendKeys("1");

    let dateTo= await driver.findElement(By.id("dateTo"));
    await dateTo.sendKeys(Key.BACK_SPACE);
    await dateTo.sendKeys("3");

    let chooseButton= await driver.findElement(By.name("chooseButton"));
    await chooseButton.click();

    let place = await driver.findElement(By.xpath("//*[contains(text(),'Szpital przy Banacha, Stefana Banacha 1a')]"));
    text = await place.getText();
    
    //assert.equal(text,"Szpital przy Banacha, Stefana Banacha 1a, Poniedziałek, 02-05-2022");
    console.log("Chose "+ text + " as vaccination place and termin");

    await place.click();

    let doctor = await driver.findElement(By.xpath("//*[contains(text(),'Lekarz Aaeesh')]"));
    text = await doctor.getText();
    
    //assert.equal(text,"Lekarz Aaeesh'a Abd Al-Rashid, 02-05-2022, 08:00 - 09:00");
    console.log("Chose "+ text + " as doctor and termin");
    await doctor.click();

    let vaccines = await driver.findElement(By.id('vaccines'));
    await vaccines.click();
    
    element = await driver.findElement(By.xpath("//*[@id='vaccines']/option[2]"));
    text = await element.getText();
    
    assert.equal(text,"AstraZeneca, firma: Oxford");
    console.log("Chose "+ text + " as vaccine");
    await element.click();

    let confirmButton = await driver.findElement(By.name("chooseVaccineButton"));
    await confirmButton.click();

    confirmButton = await driver.findElement(By.name("confirmButton"));
    await confirmButton.click();

    await driver.findElement(By.xpath("//*[text()='Pomyślnie zapisano na szczepienie']"));
    console.log("Pacjent "+firstName+" "+lastName+" pomyślnie zapisał się na szczepienie");
    
    let backButton = await driver.findElement(By.name("backButton"));
    await backButton.click();

    backButton = await driver.findElement(By.name("backButton2"));
    await backButton.click();

    expectedUrl = url+"/patient";
    await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony

    let button = await driver.findElement(By.name("logOutButton"));
    await button.click();

    expectedUrl = url+"/signin";
    await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony
    console.log("Logged out patient");
    
    await login(driver, docotrEmail, doctorPassword, url+"/doctor/redirection");
    console.log("Logged as doctor");

    button = await driver.findElement(By.name("doctorIncomingAppointmentsButton"));
    await button.click();

    expectedUrl = url+"/doctor/incomingAppointments";
    await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony

    button = await driver.findElement(By.name(firstName+lastName));
    await button.click();
    
    expectedUrl = url+"/doctor/vaccinate";
    await driver.wait(until.urlIs(expectedUrl)); //czekam na przejście do następnej strony
    
    button = await driver.findElement(By.name("vaccinateButton"));
    await button.click();

    let batchNumberForm = await driver.findElement(By.name("batchNumber"));
    await batchNumberForm.sendKeys(batchNumber);

    button = await driver.findElement(By.name("confirmButton"));
    await button.click();
    
    await driver.findElement(By.xpath("//*[text()='Pomyślnie wykonano akcję']"));
    console.log("Vaccinated "+firstName+" "+lastName);

    console.log("Register for vaccination and vaccinate test passed");

    await driver.quit();
  } catch (error) {
    console.log(error)
    console.log("Register for vaccination and vaccinate test did not pass");
    await driver.quit();
  }
}
//RegisterTest('99691208457','Grzegorz','Brzęczyszczykiewicz','GB3@onet.pl','gb1234','01-01-1939','801802803',"http://localhost:3000")
//patientLoginTest('adi222@wp.pl', 'haslohaslo',"http://localhost:3000");
//doctorLoginTest('sylwesterS@doktor.org.pl', '-EV92QbHF$!8keH=',"http://localhost:3000");
//adminLoginTest('admin@systemszczepien.org.pl', '1234',"http://localhost:3000");
//MakeNewDoctorTest('11111111104','Mania','Brzęczyszczykiewiczówna','MB7@onet.pl','1234','01-01-1946','801802809','admin@vs.org.pl', '1234',"https://systemszczepien.surge.sh");
RegisterForVaccinationTest('10101010100','Pen','Brzęczyszczykiewiczówna','PB2@onet.pl','1234','01-01-1946','801802809',"lekarz1@vs.org.pl","1234","1111","http://localhost:3000");///"https://systemszczepien.surge.sh"