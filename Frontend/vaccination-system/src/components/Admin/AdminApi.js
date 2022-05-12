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
            timeout: 2000
        });
        console.log('udało się pobrac dane')
        return [response.data, errCode];

    } catch (error) {
        console.error(error.message);
        if(response!=null)
            return [response, error.response.status.toString()];
        return [response,error.code];
    }
}

export async function getVaccinesData() {
    let response;
    let errCode = '200';
    try {
        response = await axios({
            method: 'get',
            url: SYSTEM_SZCZEPIEN_URL + '/admin/vaccines',
            timeout: 2000
        });
        console.log('udało się pobrac dane')
        const result = response.data.map(obj => {
            const newObj = {
                id: obj.vaccineId,
                company: obj.company,
                name: obj.name,
                numberOfDoses: obj.numberOfDoses,
                minDaysBetweenDoses: obj.minDaysBetweenDoses,
                maxDaysBetweenDoses: obj.maxDaysBetweenDoses,
                virus: obj.virus,
                minPatientAge: obj.minPatientAge,
                maxPatientAge: obj.maxPatientAge,
                active: obj.active
            }
            return newObj;
        })
        return [result, errCode];

    } catch (error) {
        console.error(error.message);
        if(response!=null)
            return [response, error.response.status.toString()];
        return [response,error.code];
    }
}
export async function addVaccine(company,name,numberOfDoses,minDaysBetweenDoses,maxDaysBetweenDoses,virus,minPatientAge,maxPatientAge,active)
{
    let response;
    let err = '200';
    try {
        response = await axios({
            method: 'post',
            url: SYSTEM_SZCZEPIEN_URL + '/admin/vaccines/addVaccine',
            data:
            {
                company: company,
                name: name,
                numberOfDoses: numberOfDoses,
                minDaysBetweenDoses: minDaysBetweenDoses,
                maxDaysBetweenDoses: maxDaysBetweenDoses,
                virus: virus,
                minPatientAge: minPatientAge,
                maxPatientAge: maxPatientAge,
                active: active === 'tak'?true:false
            },
            timeout: 2000
        });
        console.log(response)
        return err;

    } catch (error) {
        console.error(error.message);
        console.error(error.code);
        if(response==null)
            return error.code;
        return error.response.status.toString();
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