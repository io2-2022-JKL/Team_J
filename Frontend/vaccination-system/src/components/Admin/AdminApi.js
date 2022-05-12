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

export async function editPatient(row) {
    let err = '200'
    try {
        await axios({
            method: 'post',
            url: SYSTEM_SZCZEPIEN_URL + '/admin/patients/editPatient',
            params: {
                id: row.id,
                pesel: row.pesel,
                firstName: row.firstName,
                lastName: row.lastName,
                mail: row.mail,
                dateOfBirth: row.dateOfBirth,
                phoneNumber: row.phoneNumber,
                active: row.active
            }
        });

        return err;
    } catch (error) {
        console.error(error.message);
        return error.response.status.toString();
    }

}