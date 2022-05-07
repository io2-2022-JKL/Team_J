export default async function getCities() {
    let response;
    let errCode = '200';
    try {
        response = await axios({
            method: 'get',
            url: SYSTEM_SZCZEPIEN_URL + '/cities',
        });
        return [response.data, errCode];

    } catch (error) {
        console.error(error.message);
        return [response, error.response.status.toString()];
    }
}