import * as React from 'react';
import { render, screen } from "@testing-library/react"
import "@testing-library/jest-dom"
import DoctorMainPage from "../Doctor/DoctorMainPage.js"
import { BrowserRouter, Routes } from 'react-router-dom';

test("renders buttons", async () => {
    const { getByText } = render(<BrowserRouter> <DoctorMainPage /> </BrowserRouter>)
    expect(getByText('Twoje okna godzinowe')).toBeInTheDocument()
    expect(getByText('Przeglądaj pacjentów')).toBeInTheDocument()
    expect(getByText('Aktualna wizyta')).toBeInTheDocument()
    expect(getByText('Wystaw certyfikat szczepienia')).toBeInTheDocument()
    expect(getByText('Wyloguj się')).toBeInTheDocument()
})