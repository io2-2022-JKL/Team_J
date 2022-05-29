import { getRequest } from "./Api";

export default async function getCities() {
    return getRequest('/cities')
}