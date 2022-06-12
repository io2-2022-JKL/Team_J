import { getRequest } from "./Api";

export default async function getCities() {
    return getRequest('/cities')
}

export async function downloadCities() {
    const [data, err] = await getCities()
    cities = data.map(city => ({ value: city.city, label: city.city }))
    citiesEmptyPossible = data.map(city => ({ value: city.city, label: city.city }))
    citiesEmptyPossible.push({ value: '', label: '' })
}

export var cities = []
export var citiesEmptyPossible = []