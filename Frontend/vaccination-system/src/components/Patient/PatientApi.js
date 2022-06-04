import Moment from 'moment';
import { deleteRequest, getRequest, postRequest, postRequestNoBody } from '../../api/Api';

export async function getFreeTimeSlots(city, dateFrom, dateTo, virus) {
    console.log({
        city: city,
        dateFrom: Moment(dateFrom).format('DD-MM-YYYY'),
        dateTo: Moment(dateTo).format('DD-MM-YYYY'),
        virus: virus
    })
    return postRequest('/patient/timeSlots/filter', {
        city: city,
        dateFrom: Moment(dateFrom).format('DD-MM-YYYY'),
        dateTo: Moment(dateTo).format('DD-MM-YYYY'),
        virus: virus
    })
}

export async function bookTimeSlot(timeSlot, vaccine) {
    let patientId = localStorage.getItem('userID')
    let timeSlotId = timeSlot.timeSlotId
    let vaccineId = vaccine.vaccineId

    return postRequestNoBody('/patient/timeSlots/Book/' + patientId + '/' + timeSlotId + '/' + vaccineId)
}

export async function getFormerAppointments(patientId) {
    return getRequest('/patient/appointments/formerAppointments/' + patientId)
}

export async function getIncomingAppointments(patientId) {
    return getRequest('/patient/appointments/incomingAppointments/' + patientId)
}

export async function cancelAppointment(patientId, appointmentId) {
    return deleteRequest('/patient/appointments/incomingAppointments/cancelAppointments/' + patientId + '/' + appointmentId)
}

export async function getCertificates(patientId) {
    return getRequest('/patient/certificates/' + patientId)
}

export async function getPatientInfo(patientId) {
    return getRequest('/patient/info/' + patientId)
}
