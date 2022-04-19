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
import { Link, useNavigate } from "react-router-dom";
import { useState } from 'react';
import validator from 'validator';
import { CoPresent } from '@mui/icons-material';
import axios from 'axios';

const theme = createTheme();

export default function LoginPage() {
    const navigate = useNavigate();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [emailError, setEmailError] = useState('');
    const [emailErrorState, setEmailErrorState] = useState(false);

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
        console.log({
            email: data.get('email'),
            password: data.get('password'),
        });
    };

    async function getPatientsData() {
        try {
            const { data: response } = await axios.get('https://localhost:5001/admin/patients');
            return response;
        } catch (error) {
            console.error(error.message);
        }
    }
  
    const logInUser = async () => {
        // request i response
        if (emailError) return;

        try {
            const { data: response } = await axios({
                method: 'post',
                //url: 'http://systemszczepien.azurewebsites.net/login',
                url: 'https://localhost:5001/login',
                data: {
                    email: 'adi222@wp.pl',
                    password: 'haslohaslo'
                }
            });
            return response;
        } catch (error) {
            console.error(error.message);
        }

        const userType = email.includes("admin") ? "admin" : email.includes("patient") ? "patient" : "doctor";
        console.log({
            email,
            bool: email.includes("admin"),
            userType,
        })
        switch (userType) {
            case "admin":
                navigate("/admin");
                break;
            case "patient":
                navigate("/patient", { state: { name: "Jan", surname: "Kowalski" } });
                break;
            case "doctor":
                navigate("/doctor",);
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
                            onClick={() => { logInUser() }}
                        >
                            Zaloguj się
                        </Button>
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