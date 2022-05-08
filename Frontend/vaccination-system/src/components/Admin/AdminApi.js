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
        });
        console.log('udało się pobrac dane')
        return [response.data, errCode];

    } catch (error) {
        console.error(error.message);
        return [response, error.response.status.toString()];
    }
}

export function getRandomPatientData() {

    return [createRandomRow()
        , createRandomRow()
        , createRandomRow()
        , createRandomRow()
        , createRandomRow()
        , createRandomRow()
        , createRandomRow()
        , createRandomRow()
        , createRandomRow()
        , createRandomRow()
        , createRandomRow()
        , createRandomRow()
        , createRandomRow()
        , createRandomRow()
        , createRandomRow()
        , createRandomRow()]
}

const createRandomRow = () => {
    return {
        id: randomId(),
        PESEL: randomInt(10000000000, 100000000000).toString(),
        firstName: randomTraderName().split(' ')[0],
        lastName: randomTraderName().split(' ')[1],
        email: randomEmail(),
        dateOfBirth: dateFormat(randomDate(new Date(50, 1), new Date("1/1/30")), "isoDate").toString(),
        phoneNumber: randomPhoneNumber(),
        active: randomBoolean()
    }
};