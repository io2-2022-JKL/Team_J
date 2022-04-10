import * as React from 'react';
import { render, screen } from "@testing-library/react"
import "@testing-library/jest-dom"
import PatientMainPage from "../Patient/PatientMainPage";
import { BrowserRouter, Routes } from 'react-router-dom';
import LoginPage from '../LoginPage';

test("renders buttons", async () => {
    const { getByText } = render(<BrowserRouter> <LoginPage /> </BrowserRouter>)
    expect(getByText('Nie masz konta pacjenckiego? Zarejestruj się.')).toBeInTheDocument()
})