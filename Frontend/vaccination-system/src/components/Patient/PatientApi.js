import axios from 'axios';

export async function getFreeTimeSlots(city, dateFrom, dateTo, virus) {
    let response;
    try {
        response = await axios({
            method: 'get',
            url: 'https://systemszczepien.azurewebsites.net/patient/timeSlots/Filter',

            params: {
                /*city: city,
                dateFrom: dateFrom,
                dateTo: dateTo,
                virus: virus*/
                city: "Warszawa",
                dateFrom: "01-01-2022 10:00",
                dateTo: "01-10-2022 10:00",
                virus: "Koronawirus"
            }
        });
        console.log(
            "request succueeded"
        )
    } catch (error) {
        console.error(error.message);
    }
    return response.data;
}
