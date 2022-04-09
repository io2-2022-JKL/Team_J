import * as React from 'react';
import { render, screen } from "@testing-library/react"
import "@testing-library/jest-dom"
import PatientMainPage from "../Patient/PatientMainPage";
import { BrowserRouter, Routes } from 'react-router-dom';

test("renders buttons", async () => {
    const { getByText } = render(<BrowserRouter> <PatientMainPage /> </BrowserRouter>)
    expect(getByText('Wyloguj się')).toBeInTheDocument()
    expect(getByText('Twoje szczepienia')).toBeInTheDocument()
})