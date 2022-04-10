import * as React from 'react';
import { render, screen } from "@testing-library/react"
import "@testing-library/jest-dom"
import AdminMainPage from "../Admin/AdminMainPage.js"
import { BrowserRouter, Routes } from 'react-router-dom';

test("renders buttons", async () => {
    const { getByText } = render(<BrowserRouter> <AdminMainPage /> </BrowserRouter>)
    expect(getByText('Zarządzaj pacjentami')).toBeInTheDocument()
    expect(getByText('Zarządzaj lekarzami')).toBeInTheDocument()
    expect(getByText('Zarządzaj Centrami Szczepień')).toBeInTheDocument()
    expect(getByText('Zarządzaj szczepionkami')).toBeInTheDocument()
    expect(getByText('Wyloguj się')).toBeInTheDocument()
})