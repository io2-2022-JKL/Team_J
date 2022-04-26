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
import validator from 'validator';
import { CoPresent } from '@mui/icons-material';
import axios from 'axios';
import CircularProgress from '@mui/material/CircularProgress';
import { blue } from '@mui/material/colors';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';
import { SYSTEM_SZCZEPIEN_URL } from '../api/Api';

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
    const [loading, setLoading] = useState(false);

    const location = useLocation();
    const [snackbar, setSnackbar] = useState(location.state != null ? location.state.openSnackbar : false);

    const handleClose = (event, reason) => {
        if (reason === 'clickaway') {
            return;
        }
        setSnackbar(false);
    };
    const handleEmialChangeWithValidation = (e) => {
        var emailFiledValue = e.target.value

        console.log({
            emailFiledValue
        })

        if (validator.isEmail(emailFiledValue)) {
            setEmailError('')
            setEmailErrorState(false)
            setEmail(emailFiledValue)
        } else {
            setEmailError('Wprowadź poprawny adres email!')
            setEmailErrorState(true)
        }
    }

    const handlePasswordChange = (e) => {
        var passwordFiledValue = e.target.value

        setPassword(passwordFiledValue);
    }

    const handleSubmit = (event) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        setEmail(data.get('email'));
        setPassword(data.get('password'));
        console.log({
            email: mail,
            password: password,
        });
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
            console.log({
                response
            })



        } catch (error) {
            console.error(error.message);
            setEmailError("Nieprawidłowy email lub hasło")
            setEmailErrorState(true)
            setLoading(false)
        }

        localStorage.setItem('userID', response.data.userId)

        //console.log("local sotrage", localStorage.getItem('userID'))
        setLoading(false);


        console.log({
            response,
            mail,
            //bool: mail.includes("admin"),
            userType: response.data.userType,

        })
        switch (response.data.userType) {
            case "admin":
                navigate("/admin");
                break;
            case "patient":
                navigate("/patient", { state: { name: "Jan", surname: "Kowalski" } });
                break;
            case "doctor":
                navigate("/patient", { state: { name: "Jan", surname: "Kowalski" } });
            //navigate("/doctor",);
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
                            onChange={(e) => handleEmialChangeWithValidation(e)}
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
                            onChange={(e) => handlePasswordChange(e)}
                        />
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            disabled={loading}
                            onClick={() => { logInUser(mail, password) }}
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
                                        position: 'absolute',
                                        alignSelf: 'center',
                                        bottom: '37%',
                                        left: '50%'
                                    }}
                                />

                            )
                        }
                        <Snackbar open={snackbar} autoHideDuration={6000} onClose={handleClose}>
                            <Alert onClose={handleClose} severity="success" sx={{ width: '100%' }}>
                                Rejestarcja powiodła się
                            </Alert>
                        </Snackbar>


                        <Grid container>
                            <Grid item>
                                <Link to='/register' variant="body2">
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