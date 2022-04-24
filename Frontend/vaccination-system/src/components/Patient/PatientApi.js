import axios from 'axios';
import Moment from 'moment';

export async function getFreeTimeSlots(city, dateFrom, dateTo, virus) {

    //console.log("dateTo", Moment(dateTo).format('DD-MM-YYYY hh:mm'))
    //console.log("dateFrom", Moment(dateFrom).format('DD-MM-YYYY hh:mm'))

    let response;
    try {
        response = await axios({
            method: 'get',
            url: 'https://systemszczepien.azurewebsites.net/patient/timeSlots/Filter',

            params: {
                city: city,
                dateFrom: Moment(dateFrom).format('DD-MM-YYYY hh:mm'),
                dateTo: Moment(dateTo).format('DD-MM-YYYY hh:mm'),
                virus: virus
                /*city: "Warszawa",
                dateFrom: "01-01-2022 10:00",
                dateTo: "01-10-2022 10:00",
                virus: "Koronawirus"*/
            }
        });
        console.log(
            "request succueeded"
        )
    } catch (error) {
        console.error(error.message);
        return "fail"
    }

    return response;
}

export async function bookTimeSlot(timeSlot, vaccine) {

    let patientId = localStorage.getItem('userID')
    let timeSlotId = timeSlot.timeSlotId
    let vaccineId = vaccine.vaccineId

    console.log(patientId, timeSlotId, vaccineId)


    try {
        let response = await axios({
            method: 'post',
            url: 'https://systemszczepien.azurewebsites.net/patient/timeSlots/Book/' + patientId + '/' + timeSlotId + '/' + vaccineId,
        });
        console.log(
            "request succueeded"
        )
        return "success"

    } catch (error) {
        console.error(error.message);
        return "fail"
    }
}

export async function getFormerAppointments(patientId) {

    let response;
    try {
        response = await axios({
            method: 'get',
            url: 'https://systemszczepien.azurewebsites.net/patient/appointments/formerAppointments/'+patientId,
        });
        /*
        console.log({
            data: response.data,
        })
        */
        return response.data;
    } catch (error) {
        console.error(error.message);
    }
}

export async function getIncomingAppointments(patientId) {

    let response;
    try {
        response = await axios({
            method: 'get',
            url: 'https://systemszczepien.azurewebsites.net/patient/appointments/incomingAppointments/'+patientId,
        });
        /*
        console.log({
            data: response.data,
        })
        */
        return response.data;
    } catch (error) {
        console.error(error.message);
    }
}

export async function cancelAppointment(patientId,appointmentId) {
    let response;
    try {
        response = await axios({
            method: 'delete',
            url: 'https://systemszczepien.azurewebsites.net/patient/appointments/incomingAppointments/cancelAppointment/'+patientId+'/'+appointmentId,
        });
        
        console.log({
            response,
        })
        
    } catch (error) {
        console.error(error.message);
    }
    
}

export async function getCertificates(patientId) {

    let response;
    try {
        response = await axios({
            method: 'get',
            url: 'https://systemszczepien.azurewebsites.net/patient/certificates/'+patientId,
        });
        /*
        console.log({
            data: response.data,
        })
        */
        return response.data;

    } catch (error) {
        console.error(error.message);
    }
}
