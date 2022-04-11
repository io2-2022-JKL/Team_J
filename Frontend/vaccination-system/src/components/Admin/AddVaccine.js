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

export default function AddNewVaccine() {
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
                        Wpisz dane nowej szczepionki
                    </Typography>
                    <Box component="form" noValidate sx={{ mt: 3 }}>
                        <Grid container spacing={2}>
                            <Grid item xs={12}>
                                <TextField
                                    name="company"
                                    required
                                    fullWidth
                                    id="company"
                                    label="Firma"
                                    autoFocus
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    required
                                    fullWidth
                                    id="name"
                                    label="Nazwa"
                                    name="name"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    required
                                    fullWidth
                                    id="numberOfDoses"
                                    label="Liczba dawek"
                                    name="numberOfDoses"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    required
                                    fullWidth
                                    name="minDaysBetweenDoses"
                                    label="Minimalna liczba dni pomiędzy dawkami"
                                    id="minDaysBetweenDoses"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    required
                                    fullWidth
                                    name="maxDaysBetweenDoses"
                                    label="Maksymalna liczba dni pomiędzy dawkami"
                                    id="maxDaysBetweenDoses"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    name="virus"
                                    required
                                    fullWidth
                                    id="virus"
                                    label="Nazwa wirusa"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    required
                                    fullWidth
                                    name="minPatientAge"
                                    label="Minimalny wiek pacjenta"
                                    id="minPatientAge"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    required
                                    fullWidth
                                    name="maxPatientAge"
                                    label="Maksymalny wiek pacjenta"
                                    id="maxPatientAge"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    name="active"
                                    required
                                    fullWidth
                                    id="active"
                                    label="Aktywna"
                                />
                            </Grid>
                        </Grid>
                    </Box>
                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        sx={{ mt: 3, mb: 2 }}
                        onClick={() => { navigate("/admin/vaccines") }}
                    >
                        Dodaj szczepionkę
                    </Button>
                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        sx={{ mt: 3, mb: 2 }}
                        onClick={() => { navigate("/admin/vaccines") }}
                    >
                        Powrót
                    </Button>
                </Box>
            </Container>
        </ThemeProvider>
    )
}