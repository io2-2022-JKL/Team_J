import Moment from 'moment';
import { deleteRequest, getRequest, postRequestNoBody } from '../../api/Api';

export async function getFreeTimeSlots(city, dateFrom, dateTo, virus) {
    let from = Moment(dateFrom).format('DD-MM-YYYY')
    let to = Moment(dateTo).format('DD-MM-YYYY')

    return getRequest('/patient/timeSlots/filter' +
        '?city=' + city +
        '&dateFrom=' + from +
        '&dateTo=' + to +
        '&virus=' + virus)
}

export async function bookTimeSlot(timeSlotId, vaccineId) {
    let patientID = localStorage.getItem('patientID')
    return postRequestNoBody('/patient/timeSlots/Book/' + patientID + '/' + timeSlotId + '/' + vaccineId)
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
