import * as React from 'react';
import { useState } from 'react';
import validator from 'validator';
import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import TextField from '@mui/material/TextField';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Link, useNavigate } from "react-router-dom";

const theme = createTheme();

export default function AddNewDoctor() {
    const navigate = useNavigate();
    return (
        <ThemeProvider theme={theme}>
            <Container component="main" maxWidth="xs">
                <CssBaseline />
                <Box
                    sx={{
                        marginTop: 8,
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                    }}
                >
                    <Typography component="h1" variant="h5">
                        Wpisz dane nowego lekarza
                    </Typography>
                    <Box component="form" noValidate sx={{ mt: 3 }}>
                        <Grid container spacing={2}>
                            <Grid item xs={12}>
                                <TextField
                                    autoComplete="given-name"
                                    name="firstName"
                                    required
                                    fullWidth
                                    id="firstName"
                                    label="Imię"
                                    autoFocus
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    required
                                    fullWidth
                                    id="lastName"
                                    label="Nazwisko"
                                    name="lastName"
                                    autoComplete="family-name"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    required
                                    fullWidth
                                    id="email"
                                    label="Adres Email"
                                    name="email"
                                    autoComplete="email"
                                    type="email"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    required
                                    fullWidth
                                    name="password"
                                    label="Hasło"
                                    type="password"
                                    id="password"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    required
                                    fullWidth
                                    name="dateOfBirth"
                                    label="Data urodzenia"
                                    id="dateOfBirth"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    name="phoneNumber"
                                    required
                                    fullWidth
                                    id="phoneNumber"
                                    label="Numer telefonu"
                                    type="tel"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    name="vaccinationCenter"
                                    required
                                    fullWidth
                                    id="vaccinationCenter"
                                    label="Centrum szczepień"
                                    type="vaccinationCenter"
                                />
                            </Grid>
                        </Grid>
                    </Box>
                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        sx={{ mt: 3, mb: 2 }}
                        onClick={() => { navigate("/admin/doctors") }}
                    >
                        Dodaj lekarza
                    </Button>
                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        sx={{ mt: 3, mb: 2 }}
                        onClick={() => { navigate("/admin/doctors") }}
                    >
                        Powrót
                    </Button>
                </Box>
            </Container>
        </ThemeProvider>
    )
}