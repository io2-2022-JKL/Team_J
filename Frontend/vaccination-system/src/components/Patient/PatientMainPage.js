import * as React from 'react';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline } from '@mui/material';
import AccountBoxRoundedIcon from '@mui/icons-material/AccountBoxRounded';
import Typography from '@mui/material/Typography';
import Avatar from '@mui/material/Avatar';
import Box from '@mui/material/Box';
import { useNavigate } from "react-router-dom";
import { getPatientInfo } from './PatientApi';

const theme = createTheme();

export default function PatientMainPage() {
    const navigate = useNavigate();
    const [patientData, setPatientData] = React.useState();

    /*React.useEffect(() => {
        const fetchData = async () => {
            let userID = localStorage.getItem('userID');
            let [data, err] = await getPatientInfo(userID);
            if (data != null) {
                setPatientData(data);
            }
        }
        fetchData();
    }, []);*/

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
                            Witaj {patientData != null ? patientData.firtsName : ''} {patientData != null ? patientData.lastName : ''}
                        </Typography>
                        <Typography component="h1" variant="h6">
                            Pacjent
                        </Typography>
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={() => {
                                navigate("/patient/appointments/incomingAppointments")
                                console.log("/patient/appointments/incomingAppointments")
                            }}
                        >
                            Twoje szczepienia
                        </Button>
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={() => {
                                navigate("/patient/timeSlots")
                                console.log("patient/timeSlot")
                            }}
                        >
                            Zapisz się na szczepienie
                        </Button>
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={() => {
                                navigate("/patient/appointments/formerAppointments")
                            }}
                        >
                            Historia szczepień
                        </Button>
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={() => {
                                navigate("/patient/certificates")
                            }}
                        >
                            Twoje certyfikaty szczepień
                        </Button>
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={() => { navigate("/signin") }}
                        >
                            Wyloguj się
                        </Button>
                    </Box>
                </CssBaseline>
            </Container>
        </ThemeProvider>
    );
}