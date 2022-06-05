import { deleteRequest, getRequest, postRequest } from '../../api/Api';

export async function getPatientsData() {
    return getRequest('/admin/patients')
}

export async function getVaccinesData() {
    return getRequest('/admin/vaccines')
}

export async function getDoctorsData() {
    return getRequest('/admin/doctors')
}

export async function addVaccine(company, name, numberOfDoses, minDaysBetweenDoses, maxDaysBetweenDoses, virus, minPatientAge, maxPatientAge, active) {
    return postRequest('/admin/vaccines/addVaccine', {
        company: company,
        name: name,
        numberOfDoses: numberOfDoses,
        minDaysBetweenDoses: minDaysBetweenDoses,
        maxDaysBetweenDoses: maxDaysBetweenDoses,
        virus: virus,
        minPatientAge: minPatientAge,
        maxPatientAge: maxPatientAge,
        active: active === 'aktywny' ? true : false
    })
}

export async function editVaccine(id, company, name, numberOfDoses, minDaysBetweenDoses, maxDaysBetweenDoses, virus, minPatientAge, maxPatientAge, active) {
    return postRequest('/admin/vaccines/editVaccine', {
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
    })
}

export async function editPatient(id, pesel, firstName, lastName, mail, dateOfBirth, phoneNumber, active) {
    return postRequest('/admin/patients/editPatient', {
        id: id,
        PESEL: pesel,
        firstName: firstName,
        lastName: lastName,
        mail: mail,
        dateOfBirth: dateOfBirth,
        phoneNumber: phoneNumber,
        active: active === 'aktywny' ? true : false
    })
}

export async function deletePatient(patientId) {
    return deleteRequest('/admin/doctors/deletePatient/' + patientId);
}

export async function addDoctor(patientId, vaccinationCenterId) {
    console.log({
        patientId: patientId,
        vaccinationCenterId: vaccinationCenterId
    })
    return postRequest('/admin/doctors/addDoctor', {
        patientId: patientId,
        vaccinationCenterId: vaccinationCenterId
    })
}

export async function editDoctor(doctorId, pesel, firstName, lastName, mail, dateOfBirth, phoneNumber, vaccinationCenterId, active) {
    return postRequest('/admin/doctors/editDoctor', {
        doctorId: doctorId,
        PESEL: pesel,
        firstName: firstName,
        lastName: lastName,
        mail: mail,
        dateOfBirth: dateOfBirth,
        phoneNumber: phoneNumber,
        vaccinationCenterId: vaccinationCenterId,
        active: active === "aktywny" ? true : false
    })
}

export async function deleteDoctor(doctorId) {
    return deleteRequest('/admin/doctors/deleteDoctor/' + doctorId)
}

export async function getVaccinationCentersData() {
    return getRequest('/admin/vaccinationCenters')
}

export async function addVaccinationCenter(name, city, street, vaccineIds, openingHoursDays, active) {
    return postRequest('/admin/vaccinationCenters/addVaccinationCenter', {
        name: name,
        city: city,
        street: street,
        vaccineIds: vaccineIds,
        openingHoursDays: openingHoursDays,
        active: active
    })
}

export async function editVaccinationCenter(id, name, city, street, vaccineIds, openingHoursDays, active) {
    return postRequest('/admin/vaccinationCenters/editVaccinationCenter', {
        id: id,
        name: name,
        city: city,
        street: street,
        vaccineIds: vaccineIds,
        openingHoursDays: openingHoursDays,
        active: active
    })
}

export async function deleteVaccinationCenter(vaccinationCenterId) {
    return deleteRequest('/admin/vaccinationCenters/deleteVaccinationCenter/' + vaccinationCenterId)
}

export async function deleteVaccine(vaccineId) {
    return deleteRequest('/admin/vaccines/deleteVaccine/' + vaccineId)
}