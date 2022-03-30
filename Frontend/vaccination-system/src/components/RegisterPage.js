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
import { Link } from "react-router-dom";

const theme = createTheme();

export default function SignUp() {
  const [emailError,setEmailError] = useState('');
  const [password,setPassword] = useState('');
  const [password2Error,setPassword2Error] = useState('');
  const [emailErrorState,setEmailErrorState] = useState(false);
  const [password2ErrorState,setPassword2ErrorState] = useState(false);

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

  const holdPassword = (e) => {
    var pass = e.target.value
    setPassword(pass)
  }

  const validatePassword2 = (e) => {
    var password2 = e.target.value;
    if(validator.equals(password2,password))
    {
      setPassword2Error('')
      setPassword2ErrorState(false)
    }
    else
    {
      setPassword2Error('Hasła się nie pokrywają ze sobą')
      setPassword2ErrorState(true)
    }
  }
  const handleSubmit = (event) => {
    event.preventDefault();
    const data = new FormData(event.currentTarget);
    console.log({
      name: data.get('firstName'),
      surname: data.get('lastName'),
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
            Zarejestruj się
          </Typography>
          <Box component="form" noValidate onSubmit={handleSubmit} sx={{ mt: 3 }}>
            <Grid container spacing={2}>
            <Grid item xs={12}>
                <TextField
                  name="PESEL"
                  required
                  fullWidth
                  id="pesel"
                  label="PESEL"
                  autoFocus
                />
              </Grid>
              <Grid item xs={12} sm={6}>
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
              <Grid item xs={12} sm={6}>
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
                  error={emailErrorState}
                  required
                  fullWidth
                  id="email"
                  label="Adres Email"
                  name="email"
                  autoComplete="email"
                  type="email"
                  onChange={(e) => validateEmail(e)}
                  helperText = {emailError}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  error={password2ErrorState}
                  required
                  fullWidth
                  name="password"
                  label="Hasło"
                  type="password"
                  id="password"
                  autoComplete="new-password"
                  onChange={(e) => holdPassword(e)}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  error={password2ErrorState}
                  required
                  fullWidth
                  name="password2"
                  label="Wprowadź ponownie hasło"
                  type="password"
                  id="password2"
                  onChange={(e) => validatePassword2(e)}
                  helperText={password2Error}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  name="phoneNumber"
                  required
                  fullWidth
                  id="phoneNumber"
                  label="Numer telefonu"
                  autoFocus
                  type="tel"
                />
              </Grid>
            </Grid>
            <Button
              type="submit"
              fullWidth
              variant="contained"
              sx={{ mt: 3, mb: 2 }}
            >
              Zarejestruj się
            </Button>
            <Grid container justifyContent="flex-end">
              <Grid item>
                <Link to='/signin' variant="body2">
                  Masz już konto? Zaloguj się
                </Link>
              </Grid>
            </Grid>
          </Box>
        </Box>
      </Container>
    </ThemeProvider>
  );
}