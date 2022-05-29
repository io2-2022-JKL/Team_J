import axios from 'axios';
import { SYSTEM_SZCZEPIEN_URL } from '../../api/Api';

export async function getIncomingAppointments(doctorId) {
    let response;
    let errCode = '200';
    try {
        response = await axios({
            method: 'get',
            url: SYSTEM_SZCZEPIEN_URL + '/doctor/incomingAppointments/' + doctorId,
            timeout: 4000
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

export async function getDoctorInfo(doctorId) {
    let response;
    let errCode = '200';
    try {
        response = await axios({
            method: 'get',
            url: SYSTEM_SZCZEPIEN_URL + '/doctor/info/' + doctorId,
        });
        return [response.data, errCode];
    } catch (error) {
        console.error(error.message);
        if (error.response != null)
            return [response, error.response.status.toString()];
        return [response, error.code];
    }
}

export async function getTimeSlots(doctorId) {
    let response;
    let errCode = '200';
    try {
        response = await axios({
            method: 'get',
            url: SYSTEM_SZCZEPIEN_URL + '/doctor/timeSlots/' + doctorId,
            timeout: 4000
        });
        return [response.data, errCode];
    } catch (error) {
        console.error(error.message);
        if (error.response != null)
            return [response, error.response.status.toString()];
        return [response, error.code];
    }
}

export async function modifyTimeSlots(doctorId,timeSlotId,timeFrom,timeTo) {
    let response;
    let errCode = '200';
    try {
        response = await axios({
            method: 'post',
            url: SYSTEM_SZCZEPIEN_URL + '/doctor/timeSlots/modify/' + doctorId + '/' + timeSlotId,
            timeout: 4000,
            data:
            {
                timeFrom: timeFrom,
                timeTo: timeTo 
            }
        });
        return errCode;
    } catch (error) {
        console.error(error.message);
        if (error.response != null)
            return error.response.status.toString();
        return error.code;
    }
}