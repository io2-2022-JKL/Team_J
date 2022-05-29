import { getRequest } from '../../api/Api';

export async function getIncomingAppointments(doctorId) {
    return getRequest('/doctor/incomingAppointments/' + doctorId)
}

export async function getDoctorInfo(doctorId) {
    return getRequest('/doctor/info/' + doctorId)
}

export async function getAppointmetInfo(doctorId, appointmentId) {
    return getRequest('/doctor/vaccinate/' + doctorId + '/' + appointmentId)
}