import { getRequest } from "./Api";

export default async function getViruses() {
    return getRequest('/viruses')
}