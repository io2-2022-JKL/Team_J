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

const theme = createTheme();

export default function LoginPage() {
    const navigate = useNavigate();
    const [emailError, setEmailError] = useState('');
    const [emailErrorState, setEmailErrorState] = useState(false);

    const validateEmail = (e) => {
        var email = e.target.value

        if (validator.isEmail(email)) {
            setEmailError('')
            setEmailErrorState(false)
        } else {
            setEmailError('Wprowadź poprawny adres email!')
            setEmailErrorState(true)
        }
    }

    const handleSubmit = (event) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        console.log({
            email: data.get('email'),
            password: data.get('password'),
        });
    };

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
                            onChange={(e) => validateEmail(e)}
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
                        />
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={() => { navigate("/patient") }}
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