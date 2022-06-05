import * as React from 'react';
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
import { Link, useNavigate, useLocation } from "react-router-dom";
import { useState } from 'react';
import axios from 'axios';
import CircularProgress from '@mui/material/CircularProgress';
import { blue } from '@mui/material/colors';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';
import { SYSTEM_SZCZEPIEN_URL } from '../api/Api';
import ValidationHelpers from '../tools/ValidationHelpers';
import { getPatientInfo } from './Patient/PatientApi';
import { getDoctorInfo } from './Doctor/DoctorApi';
import getViruses, { downloadViruses } from '../api/Viruses';
import getCities, { downloadCities } from '../api/Cities';

const theme = createTheme();

const Alert = React.forwardRef(function Alert(props, ref) {
    return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
});

export default function LoginPage() {
    const navigate = useNavigate();
    const [mail, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [emailError, setEmailError] = useState('');
    const [emailErrorState, setEmailErrorState] = useState(false);
    const [passwordError, setPasswordError] = useState('');
    const [passwordErrorState, setPasswordErrorState] = useState(false);
    const [loading, setLoading] = useState(false);

    const location = useLocation();
    const [snackbar, setSnackbar] = useState(location.state != null ? location.state.openSnackbar : false);

    const handleClose = (event, reason) => {
        if (reason === 'clickaway') {
            return;
        }
        setSnackbar(false);
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        setEmail(data.get('email'));
        setPassword(data.get('password'));
    };


    const logInUser = async () => {
        // request i response
        if (emailError) return;
        let response;
        setLoading(true);
        try {
            response = await axios({
                method: 'post',
                url: SYSTEM_SZCZEPIEN_URL + '/signin',
                data: {
                    mail: mail,
                    password: password
                }

            });
        } catch (error) {
            console.error(error.message);
            setEmailError("Logowanie nie powiodło się")
            setEmailErrorState(true)
            setPasswordError("Logowanie nie powiodło się")
            setPasswordErrorState(true)
            setLoading(false)
        }

        const [viruses, virusesError] = await getViruses();
        localStorage.setItem('viruses', JSON.stringify(viruses))
        const [cities, citiesError] = await getCities();
        localStorage.setItem('cities', JSON.stringify(cities))
        downloadCities()
        downloadViruses()

        localStorage.setItem('userID', response.data.userId)

        axios.defaults.headers.common['authorization'] = 'Bearer ' + response.headers.authorization

        setLoading(false);

        localStorage.setItem('isDoctor', false)
        console.log(response.data.userType)
        let [data, err] = [];
        let patientId;
        switch (response.data.userType) {
            case "admin":
            case "Admin":
                navigate("/admin");
                break;
            case "patient":
            case "Patient":
                patientId = localStorage.getItem('userID');
                [data, err] = await getPatientInfo(patientId);
                localStorage.setItem('patientID', patientId)
                localStorage.setItem('userFirstName', data.firstName)
                localStorage.setItem('userLastName', data.lastName)
                navigate("/patient");
                break;
            case "doctor":
            case "Doctor":
                let [doctorData, DoctorErr] = []
                localStorage.setItem('isDoctor', true)
                let doctorId = localStorage.getItem('userID');
                console.log(doctorId);
                [doctorData, DoctorErr] = await getDoctorInfo(doctorId);
                console.log(doctorData.patientId);
                localStorage.setItem('patientID', doctorData.patientId);
                //console.log('doctorData')
                //patientId = doctorData.patientId
                [data, err] = await getPatientInfo(doctorData.patientId);
                localStorage.setItem('userFirstName', data.firstName)
                localStorage.setItem('userLastName', data.lastName)
                localStorage.setItem('doctorID', doctorId)

                navigate("/doctor/redirection", { state: { page: "doctor" } });
                break;
            default:
                break;
        }
    }

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
                    <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
                        <LockOutlinedIcon />
                    </Avatar>
                    <Typography component="h1" variant="h5">
                        Zaloguj się
                    </Typography>
                    <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
                        <TextField
                            error={emailErrorState}
                            margin="normal"
                            required
                            fullWidth
                            id="email"
                            label="Adres Email"
                            name="email"
                            autoComplete="email"
                            autoFocus
                            onChange={(e) => { setEmail(e.target.value); ValidationHelpers.validateEmail(e, setEmailError, setEmailErrorState) }}
                            helperText={emailError}
                        />
                        <TextField
                            margin="normal"
                            required
                            fullWidth
                            name="password"
                            label="Hasło"
                            type="password"
                            id="password"
                            autoComplete="current-password"
                            onChange={(e) => { setPassword(e.target.value); ValidationHelpers.validatePassword(e, setPasswordError, setPasswordErrorState) }}
                            error={passwordErrorState}
                            helperText={passwordError}
                        />
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            disabled={loading}
                            onClick={() => { logInUser(mail, password) }}
                            name="submitButton"
                        >
                            Zaloguj się
                        </Button>
                        {
                            loading &&
                            (
                                <CircularProgress
                                    size={24}
                                    sx={{
                                        color: blue,
                                        position: 'relative',
                                        alignSelf: 'center',
                                        left: '50%'
                                    }}
                                />

                            )
                        }
                        <Snackbar open={snackbar} autoHideDuration={6000} onClose={handleClose}>
                            <Alert onClose={handleClose} severity="success" sx={{ width: '100%' }} name="snackbar">
                                Rejestarcja powiodła się
                            </Alert>
                        </Snackbar>


                        <Grid container>
                            <Grid item>
                                <Link to='/register' variant="body2" name="link">
                                    {"Nie masz konta pacjenckiego? Zarejestruj się."}
                                </Link>
                            </Grid>
                        </Grid>
                    </Box>
                </Box>
            </Container>
        </ThemeProvider>
    );
}