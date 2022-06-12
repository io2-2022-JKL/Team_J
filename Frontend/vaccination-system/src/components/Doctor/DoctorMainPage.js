import * as React from 'react';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline } from '@mui/material';
import AccountBoxRoundedIcon from '@mui/icons-material/AccountBoxRounded';
import Typography from '@mui/material/Typography';
import Avatar from '@mui/material/Avatar';
import Box from '@mui/material/Box';
import { useNavigate } from "react-router-dom";
import LoginHelpers from '../../tools/LoginHelpers';

const theme = createTheme();

export default function DoctorMainPage() {
    const navigate = useNavigate();

    React.useEffect(() => {
        LoginHelpers.preventGoingBack();
    }, [])

    return (
        <ThemeProvider theme={theme}>
            <Container component="main" maxWidth="xs">
                <CssBaseline>
                    <Box
                        sx={{
                            marginTop: 8,
                            display: 'flex',
                            flexDirection: 'column',
                            alignItems: 'center',
                        }}
                    >
                        <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
                            <AccountBoxRoundedIcon />
                        </Avatar>
                        <Typography component="h1" variant="h4">
                            Witaj {localStorage.getItem('userFirstName') != null && localStorage.getItem('userLastName') != null
                                && (localStorage.getItem('userFirstName') + ' ' + localStorage.getItem('userLastName'))}
                        </Typography>
                        <Typography component="h1" variant="h6">
                            Lekarz
                        </Typography>
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={() => {
                                navigate("/doctor/timeSlots")
                            }}
                        >
                            Twoje okna godzinowe
                        </Button>
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={() => {
                                navigate("/doctor/incomingAppointments")
                            }}
                            name="doctorIncomingAppointmentsButton"
                        >
                            Przeglądaj przyszłe szczepienia
                        </Button>
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={() => {
                                navigate("/doctor/formerAppointments")
                            }}
                        >
                            Zakończone szczepienia
                        </Button>
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={() => {
                                LoginHelpers.logOut()
                                navigate("/signin")
                            }
                            }
                            name="doctorLogOutButton"
                        >
                            Wyloguj się
                        </Button>
                    </Box>
                </CssBaseline>
            </Container>
        </ThemeProvider>
    );
}