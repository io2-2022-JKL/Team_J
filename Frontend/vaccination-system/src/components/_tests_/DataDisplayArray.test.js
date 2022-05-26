import * as React from 'react';
import { render, screen } from "@testing-library/react"
import "@testing-library/jest-dom"
import PatientMainPage from "../Patient/PatientMainPage";
import { BrowserRouter, Routes } from 'react-router-dom';
import { getRandomPatientData } from '../Admin/AdminApi';
import DataDisplayArray from '../DataDisplayArray';

const mockColumns = [
    {
        field: 'id',
        flex: 2
    },
    {
        field: 'PESEL',
        minWidth: 110,
        flex: 0.5,
        editable: true
    },
    {
        field: 'firstName',
        headerName: 'Imię',
        flex: 0.5,
        editable: true
    },
    {
        field: 'lastName',
        headerName: 'Nazwisko',
        flex: 0.5,
        editable: true
    },
    {
        field: 'email',
        headerName: 'E-Mail',
        minWidth: 125,
        flex: 0.5,
        editable: true
    },
    {
        field: 'dateOfBirth',
        headerName: 'Data urodzenia',
        flex: 0.75,
        editable: true
    },
    {
        field: 'phoneNumber',
        headerName: 'Numer telefonu',
        minWidth: 125,
        flex: 0.5,
        editable: true
    },
    {
        field: 'active',
        headerName: 'Aktywny',
        type: 'boolean',
        editable: true,
        flex: 0.5,
        cellClassName: (params) => {
            if (params.value == null) {
                return '';
            }
            return clsx('active', {
                negative: params.value === false,
                positive: params.value === true,
            });
        },
    },
];

test("renders buttons", async () => {

})

//const mockData = getRandomPatientData();

/*test("renders buttons", async () => {
    const { getByText } = render(
        <DataDisplayArray
            loading={false}
            editCell={null}
            columns={mockColumns}
            filteredRows={mockData}
        />
    )
    expect(getByText('Imię')).toBeInTheDocument()
})*/