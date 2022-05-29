import { getRequest } from "./Api";

export default async function getViruses() {
    return getRequest('/viruses')
}

export async function downloadViruses() {
    const [data, err] = await getViruses()
    viruses = data.map(virus => ({ value: virus.virus, label: virus.virus }))
    virusesEmptyPossible = data.map(virus => ({ value: virus.virus, label: virus.virus }))
    virusesEmptyPossible.push({ value: '', label: '' })
}

export var viruses = []
export var virusesEmptyPossible = []