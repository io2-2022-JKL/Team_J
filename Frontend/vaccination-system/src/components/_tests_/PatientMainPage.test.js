import * as React from 'react';
import { render, screen } from "@testing-library/react"
import "@testing-library/jest-dom"
import PatientMainPage from "../Patient/PatientMainPage";
import { BrowserRouter, Routes } from 'react-router-dom';

test("renders buttons", async () => {
    const { getByText } = render(<BrowserRouter> <PatientMainPage /> </BrowserRouter>)
    expect(getByText('Twoje szczepienia')).toBeInTheDocument()
    expect(getByText('Zapisz się na szczepienie')).toBeInTheDocument()
    expect(getByText('Historia szczepień')).toBeInTheDocument()
    expect(getByText('Twoje certyfikaty szczepień')).toBeInTheDocument()
    expect(getByText('Wyloguj się')).toBeInTheDocument()
})