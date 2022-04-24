import axios from 'axios';

export async function getFreeTimeSlots(city, virus, dateFrom, dateTo) {

    let response;
    try {
        response = await axios({
            method: 'post',
            url: 'https://systemszczepien.azurewebsites.net/patient/timeSlots/Filter',

            data: {
                city: "Warszawa",
                dateFrom: "01-01-2022 10:00",
                dateTo: "01-10-2022 10:00",
                virus: "Koronawirus"
            }
        });
        console.log({
            response,
        })
    } catch (error) {
        console.error(error.message);
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
