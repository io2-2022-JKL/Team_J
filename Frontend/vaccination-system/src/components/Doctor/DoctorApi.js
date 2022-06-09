import { getRequest, postRequest, postRequestNoBody } from '../../api/Api';

export async function getIncomingAppointments(doctorId) {
    return getRequest('/doctor/incomingAppointments/' + doctorId)
}

export async function getFormerAppointments(doctorId) {
    return getRequest('/doctor/formerAppointments/' + doctorId)
}

export async function getDoctorInfo(doctorId) {
    return getRequest('/doctor/info/' + doctorId)
}

export async function getAppointmetInfo(doctorId, appointmentId) {
    return getRequest('/doctor/vaccinate/' + doctorId + '/' + appointmentId)
}

export async function createTimeSlots(doctorId, windowBegin, windowEnd, duration) {
    return postRequest('/doctor/timeSlots/create/' + doctorId, {
        windowBegin: windowBegin,
        windowEnd: windowEnd,
        timeSlotDurationInMinutes: duration
    })
}

export async function modifyTimeSlots(doctorId, timeSlotId, timeFrom, timeTo) {
    return postRequest('/doctor/timeSlots/modify/' + doctorId + '/' + timeSlotId, {
        timeFrom: timeFrom,
        timeTo: timeTo
    })
}

export async function deleteTimeSlots(doctorId, timeSlotsIds) {
    return postRequest('/doctor/timeSlots/delete/' + doctorId, timeSlotsIds.map(function (id) { return { id } }));
}

export async function getTimeSlots(doctorId) {
    return getRequest('/doctor/timeSlots/' + doctorId)
}

export async function vaccinationDidNotHappen(doctorId, appointmentId) {
    return postRequestNoBody('/doctor/vaccinate/vaccinationDidNotHappen/' + doctorId + '/' + appointmentId)
}

export async function confirmVaccination(doctorId, appointmentId, batchId) {
    return postRequestNoBody('/doctor/vaccinate/confirmVaccination/' + doctorId + '/' + appointmentId + '/' + batchId)
}

export async function certify(doctorId, appointmentId) {
    return postRequestNoBody('/doctor/vaccinate/certify/' + doctorId + '/' + appointmentId)
}