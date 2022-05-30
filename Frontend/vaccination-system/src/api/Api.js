import axios from "axios";

export const SYSTEM_SZCZEPIEN_URL = //'https://localhost:5001'
'https://systemszczepien.azurewebsites.net'
//'https://vaccinationsystemteaml.azurewebsites.net'
// j.nowak@mail.com password123()
//'https://vaccinationsystemapi.azurewebsites.net'
// kowalskij@example.com 123456

export async function getRequest(URI) {
    let response;
    let errCode = '200';
    try {
        response = await axios({
            method: 'get',
            url: SYSTEM_SZCZEPIEN_URL + URI,
            timeout: 5000
        });
        console.log('udało się pobrać dane')
        return [response.data, errCode];

    } catch (error) {
        console.error(error.message);
        if (error.response != null)
            return [response, error.response.status.toString()];
        return [response, error.code];
    }
}

export async function postRequest(URI, body) {
    let response;
    let err = '200';
    try {
        response = await axios({
            method: 'post',
            url: SYSTEM_SZCZEPIEN_URL + URI,
            data: body,
            timeout: 5000
        });
        return err;

    } catch (error) {
        console.error(error.message);
        console.error(error.code);
        if (error.response == null)
            return error.code;
        return error.response.status.toString();
    }
}

export async function postRequestNoBody(URI) {
    let response;
    let err = '200';
    try {
        response = await axios({
            method: 'post',
            url: SYSTEM_SZCZEPIEN_URL + URI,
            timeout: 5000
        });
        return err;

    } catch (error) {
        console.error(error.message);
        console.error(error.code);
        if (error.response == null)
            return error.code;
        return error.response.status.toString();
    }
}

export async function deleteRequest(URI) {
    let response;
    let err = '200';
    try {
        response = await axios({
            method: 'delete',
            url: SYSTEM_SZCZEPIEN_URL + URI,
            timeout: 5000
        });
        console.log(response)
        return err;

    } catch (error) {
        console.error(error.message);
        console.error(error.code);
        if (error.response == null)
            return error.code;
        return error.response.status.toString();
    }
}
