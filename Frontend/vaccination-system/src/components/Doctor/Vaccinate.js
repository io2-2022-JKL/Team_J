import { Button, Container, CssBaseline, Grid, Typography } from '@mui/material';
import { Box, createTheme } from '@mui/system';
import * as React from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { ThemeProvider } from 'styled-components';
import { vaccinationDidNotHappen } from './DoctorApi';

const theme = createTheme();

export default function Vaccinate() {
    const location = useLocation();
    const navigate = useNavigate();

    const [data, setData] = React.useState([]);
    const [loading, setLoading] = React.useState(false);
    const [error, setError] = React.useState('');
    const [errorState, setErrorState] = React.useState(false);


    return (
        <ThemeProvider theme={theme}>
            <Container component="main" maxWidth="lg">
                <CssBaseline />
                <Box
                    sx={{
                        margin: 8,
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                    }}
                >
                    <Typography component="h1" variant="h5" alignSelf={'center'}>
                        Szczepienie
                    </Typography>
                    <Box sx={{ m: 2 }} />

                    <Grid >

                        <Grid
                            container
                            direction="column"
                            justifyContent="center"
                            alignItems="center"
                        >
                            <Box>Pacjent: </Box>
                        </Grid>
                        <Box> {location.state.patientFirstName + " " + location.state.patientLastName} </Box>
                        <Box> {'PESEL: ' + location.state.PESEL} </Box>
                        <Box> {'Data urodzenia: ' + location.state.dateOfBirth} </Box>
                        <Box> {'Dawka szczepionki: ' + location.state.whichVaccineDose} </Box>
                        <Box sx={{ m: 2 }} />

                        <Grid
                            container
                            direction="column"
                            justifyContent="center"
                            alignItems="center"
                        >
                            <Box>Szczepionka: </Box>
                        </Grid>
                        <Box> {location.state.vaccineName} </Box>
                        <Box> {'Firma: ' + location.state.vaccineCompany} </Box>
                        <Box> {'Wirus: ' + location.state.virusName} </Box>
                        <Box> {'Wiek pacjenta: ' + location.state.minPatientAge + ' - ' + location.state.maxPatientAge} </Box>
                        <Box> {'Liczba dawek: ' + location.state.numberOfDoses} </Box>
                        <Box> {'Dni pomiędzy dawkami: ' + location.state.minDaysBetweenDoses + ' - ' + location.state.maxDaysBetweenDoses} </Box>
                        <Box sx={{ m: 2 }} />

                        <Grid
                            container
                            direction="column"
                            justifyContent="center"
                            alignItems="center"
                        >
                            <Button
                                type="submit"
                                fullWidth
                                variant="contained"
                                sx={{ mt: 3, mb: 2 }}
                            >
                                Przeprowadź szczepienie
                            </Button>
                            <Button
                                type="submit"
                                fullWidth
                                variant="contained"
                                sx={{ mt: 3, mb: 2 }}
                                onClick={() => { console.log(location.appointmentId) /*vaccinationDidNotHappen(localStorage.getItem('userID'), location.appointmentId)*/ }}
                            >
                                Anuluj szczepienie
                            </Button>
                            <Button
                                type="submit"
                                fullWidth
                                variant="contained"
                                sx={{ mt: 3, mb: 2 }}
                                onClick={() => navigate('/doctor/incomingAppointments')}
                            >
                                Wróć
                            </Button>

                        </Grid>



                    </Grid>
                </Box>
            </Container>
        </ThemeProvider >
    );
}