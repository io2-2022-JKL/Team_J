import * as React from 'react';
import { render } from "@testing-library/react"
import "@testing-library/jest-dom"
import PatientMainPage from "./PatientMainPage";
import { BrowserRouter, Routes } from 'react-router-dom';

test("renders component", async () => {
    const { getByText } = render(<BrowserRouter> <PatientMainPage /> </BrowserRouter>)
    expect(getByText('Wyloguj się')).toBeInTheDocument()
})