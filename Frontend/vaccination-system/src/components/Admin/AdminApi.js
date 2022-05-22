import { randomDate, randomEmail, randomId, randomPhoneNumber, randomTraderName, randomBoolean, randomInt } from '@mui/x-data-grid-generator';
import dateFormat from 'dateformat';
import axios from 'axios';
import { SYSTEM_SZCZEPIEN_URL } from '../../api/Api';

export async function getPatientsData() {
    let response;
    let errCode = '200';
    try {
        response = await axios({
            method: 'get',
            url: SYSTEM_SZCZEPIEN_URL + '/admin/patients',
            timeout: 2000
        });
        console.log('udało się pobrac dane')
        return [response.data, errCode];

    } catch (error) {
        console.error(error.message);
        if (error.response != null)
            return [response, error.response.status.toString()];
        return [response, error.code];
    }
}

export async function getVaccinesData() {
    let response;
    let errCode = '200';
    try {
        response = await axios({
            method: 'get',
            url: SYSTEM_SZCZEPIEN_URL + '/admin/vaccines',
            timeout: 2000,
        });
        console.log('udało się pobrac dane')
        const result = response.data.map(obj => {
            const newObj = {
                id: obj.vaccineId,
                company: obj.company,
                name: obj.name,
                numberOfDoses: obj.numberOfDoses,
                minDaysBetweenDoses: obj.minDaysBetweenDoses,
                maxDaysBetweenDoses: obj.maxDaysBetweenDoses,
                virus: obj.virus,
                minPatientAge: obj.minPatientAge,
                maxPatientAge: obj.maxPatientAge,
                active: obj.active
            }
            return newObj;
        })
        return [result, errCode];

    } catch (error) {
        console.error(error.message);
        if (error.response != null)
            return [response, error.response.status.toString()];
        return [response, error.code];
    }
}

export async function getDoctorsData() {
    let response;
    let errCode = '200';
    try {
        response = await axios({
            method: 'get',
            url: SYSTEM_SZCZEPIEN_URL + '/admin/doctors',
            timeout: 2000
        });
        console.log('udało się pobrać dane')
        return [response.data, errCode];

    } catch (error) {
        console.error(error.message);
        if (error.response != null)
            return [response, error.response.status.toString()];
        return [response, error.code];
    }
}

export async function addVaccine(company, name, numberOfDoses, minDaysBetweenDoses, maxDaysBetweenDoses, virus, minPatientAge, maxPatientAge, active) {
    let response;
    let err = '200';
    try {
        response = await axios({
            method: 'post',
            url: SYSTEM_SZCZEPIEN_URL + '/admin/vaccines/addVaccine',
            data:
            {
                company: company,
                name: name,
                numberOfDoses: numberOfDoses,
                minDaysBetweenDoses: minDaysBetweenDoses,
                maxDaysBetweenDoses: maxDaysBetweenDoses,
                virus: virus,
                minPatientAge: minPatientAge,
                maxPatientAge: maxPatientAge,
                active: active === 'aktywny' ? true : false
            },
            timeout: 2000
        });
        //console.log(response)
        return err;

    } catch (error) {
        console.error(error.message);
        console.error(error.code);
        if (error.response == null)
            return error.code;
        return error.response.status.toString();
    }
}

export async function editVaccine(id, company, name, numberOfDoses, minDaysBetweenDoses, maxDaysBetweenDoses, virus, minPatientAge, maxPatientAge, active) {
    let response;
    let err = '200';
    try {
        response = await axios({
            method: 'post',
            url: SYSTEM_SZCZEPIEN_URL + '/admin/vaccines/editVaccine',
            data:
            {
                vaccineId: id,
                company: company,
                name: name,
                numberOfDoses: numberOfDoses,
                minDaysBetweenDoses: minDaysBetweenDoses,
                maxDaysBetweenDoses: maxDaysBetweenDoses,
                virus: virus,
                minPatientAge: minPatientAge,
                maxPatientAge: maxPatientAge,
                active: active === 'aktywny' ? true : false
            },
            timeout: 2000
        });
        console.log(response)
        return err;

    } catch (error) {
        console.error(error.message);
        console.error(error.code);
        if (error.response == null)
            return error.code;
        return error.response.status.toString();
    }
}

export async function editPatient(id, pesel, firstName, lastName, mail, dateOfBirth, phoneNumber, active) {
    let response;
    let err = '200';

    try {
        response = await axios({
            method: 'post',
            url: SYSTEM_SZCZEPIEN_URL + '/admin/patients/editPatient',
            data:
            {
                id: id,
                PESEL: pesel,
                firstName: firstName,
                lastName: lastName,
                mail: mail,
                dateOfBirth: dateOfBirth,
                phoneNumber: phoneNumber,
                active: active === 'aktywny' ? true : false
            },
            timeout: 2000
        });
        console.log(response)
        return err;

    } catch (error) {
        console.error(error.message);
        console.error(error.code);
        if (error.response == null)
            return error.code;
        return error.response.status.toString();
    }
}

export async function deletePatient(patientId) {
    let response;
    let err = '200';
    try {
        response = await axios({
            method: 'delete',
            url: SYSTEM_SZCZEPIEN_URL + '/admin/doctors/deleteDoctor/' + patientId,
            timeout: 2000
        });
        console.log(response)
        return err;

    } catch (error) {
        console.error(error.message);
        console.error(error.code);
        if (error.response == null)
            return error.code;
        return error.response.status.toString();
    }
}

export async function addDoctor(patientId, vaccinationCenterId) {
    console.log(patientId, vaccinationCenterId);
    return;

    let response;
    let err = '200';

    try {
        response = await axios({
            method: 'post',
            url: SYSTEM_SZCZEPIEN_URL + '/admin/doctors/addDoctor',
            data:
            {
                patientId: patientId,
                vaccinationCenterId: vaccinationCenterId
            },
            timeout: 2000
        });
        console.log(response)
        return err;

    } catch (error) {
        console.error(error.message);
        console.error(error.code);
        if (error.response == null)
            return error.code;
        return error.response.status.toString();
    }
}

export async function editDoctor(doctorId, pesel, firstName, lastName, mail, dateOfBirth, phoneNumber, vaccinationCenterId) {
    let response;
    let err = '200';

    try {
        response = await axios({
            method: 'post',
            url: SYSTEM_SZCZEPIEN_URL + '/admin/doctors/editDoctor',
            data:
            {
                doctorId: doctorId,
                PESEL: pesel,
                firstName: firstName,
                lastName: lastName,
                mail: mail,
                dateOfBirth: dateOfBirth,
                phoneNumber: phoneNumber,
                vaccinationCenterId: vaccinationCenterId
            },
            timeout: 2000
        });
        console.log(response)
        return err;

    } catch (error) {
        console.error(error.message);
        console.error(error.code);
        if (error.response == null)
            return error.code;
        return error.response.status.toString();
    }
}

export async function deleteDoctor(doctorId) {
    let response;
    let err = '200';
    try {
        response = await axios({
            method: 'delete',
            url: SYSTEM_SZCZEPIEN_URL + '/admin/doctors/deleteDoctor/' + doctorId,
            timeout: 2000
        });
        console.log(response)
        return err;

    } catch (error) {
        console.error(error.message);
        console.error(error.code);
        if (error.response == null)
            return error.code;
        return error.response.status.toString();
    }
}

export async function getVaccinationCentersData() {
    let response;
    let errCode = '200';
    try {
        response = await axios({
            method: 'get',
            url: SYSTEM_SZCZEPIEN_URL + '/admin/vaccinationCenters',
            timeout: 2000
        });
        console.log('udało się pobrac dane')
        return [response.data, errCode];

    } catch (error) {
        console.error(error.message);
        if (error.response != null)
            return [response, error.response.status.toString()];
        return [response, error.code];
    }
}

export async function addVaccinationCenter(name, city, street, vaccineIds, openingHoursDays, active) {
    let response;
    let err = '200';

    try {
        response = await axios({
            method: 'post',
            url: SYSTEM_SZCZEPIEN_URL + '/admin/vaccinationCenters/addVaccinationCenter',
            data:
            {
                name: name,
                city: city,
                street: street,
                vaccineIds: vaccineIds,
                openingHoursDays: openingHoursDays,
                active: active
            },
            timeout: 2000
        });
        console.log(response)
        return err;

    } catch (error) {
        console.error(error.message);
        console.error(error.code);
        if (error.response == null)
            return error.code;
        return error.response.status.toString();
    }
}

export async function editVaccinationCenter(id, name, city, street, vaccineIds, openingHoursDays, active) {
    let response;
    let err = '200';

    try {
        response = await axios({
            method: 'post',
            url: SYSTEM_SZCZEPIEN_URL + '/admin/vaccinationCenters/editVaccinationCenter',
            data:
            {
                id: id,
                name: name,
                city: city,
                street: street,
                vaccineIds: vaccineIds,
                openingHoursDays: openingHoursDays,
                active: active
            },
            timeout: 2000
        });
        console.log(response)
        return err;

    } catch (error) {
        console.error(error.message);
        console.error(error.code);
        if (error.response == null)
            return error.code;
        return error.response.status.toString();
    }
}