import * as React from 'react';
import { Fragment } from 'react';
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
import { Link, useNavigate} from "react-router-dom";
import axios from 'axios';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import CircularProgress from '@mui/material/CircularProgress';
import { blue } from '@mui/material/colors';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';

const theme = createTheme();

const Alert = React.forwardRef(function Alert(props, ref) {
  return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
});

export default function SignUp() {
  const [emailError, setEmailError] = useState('');
  const [password, setPassword] = useState('');
  const [password2Error, setPassword2Error] = useState('');
  const [emailErrorState, setEmailErrorState] = useState(false);
  const [password2ErrorState, setPassword2ErrorState] = useState(false);
  const [loading, setLoading] = useState(false);
  const [date, setDate] = useState(null);
  const [registerError, setRegisterError] = useState(false);
  const [success, setSuccess] = useState(false);

  const navigate = useNavigate();

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
    if (validator.equals(password2, password)) {
      setPassword2Error('')
      setPassword2ErrorState(false)
    }
    else {
      setPassword2Error('Hasła się nie pokrywają ze sobą')
      setPassword2ErrorState(true)
    }
  }

  const signInUser = async (pesel, firstName, lastName, mail, dateOfBirth, password, phoneNumber) => {
    // request i response
    if (emailErrorState) return;
    if (password2ErrorState) return;
    setLoading(true);
    try {
      const { data: response } = await axios({
        method: 'post',
        url: 'https://systemszczepien.azurewebsites.net/register',
        data: {
          PESEL: pesel,
          firstName: firstName,
          lastName: lastName,
          mail: mail,
          dateOfBirth: dateOfBirth,
          password: password,
          phoneNumber: phoneNumber
        }
      });
      console.log({
        response
      })
      setSuccess(true);
    } catch (error) {
      console.error(error.message);
      setRegisterError(true);
      setSuccess(false);
    }
    setLoading(false);
  }

  const handleSubmit = (event) => {
    event.preventDefault();
    const data = new FormData(event.currentTarget);
    console.log({
      PESEL: data.get('PESEL'),
      firstName: data.get('firstName'),
      lastName: data.get('lastName'),
      mail: data.get('mail'),
      dateOfBirth: data.get('dateOfBirth'),
      password: data.get('password'),
      phoneNumber: data.get('phoneNumber')
    });
    signInUser(data.get('PESEL'), data.get('firstName'), data.get('lastName'), data.get('mail'), data.get('dateOfBirth'), data.get('password'), data.get('phoneNumber'));
  };

  const handleClose = (event, reason) => {
    if (reason === 'clickaway') {
      return;
    }

    setRegisterError(false);
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
                  id="PESEL"
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
                  id="mail"
                  label="Adres Email"
                  name="mail"
                  autoComplete="email"
                  type="email"
                  onChange={(e) => validateEmail(e)}
                  helperText={emailError}
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
                <LocalizationProvider dateAdapter={AdapterDateFns}>
                  <DatePicker
                    label="Data urodzenia"
                    views={['year', 'month', 'day']}
                    inputFormat="dd-MM-yyyy"
                    mask="__-__-____"
                    value={date}
                    onChange={(newDate) => {
                      setDate(newDate);
                    }}
                    renderInput={(params) => <TextField
                      {...params}
                      fullWidth
                      id='dateOfBirth'
                      name='dateOfBirth'
                    />}
                  />
                </LocalizationProvider>
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
              disabled={loading}
              sx={{ mt: 3, mb: 2 }}
            >
              Zarejestruj się
            </Button>
            {loading && (
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
            )}
            {registerError && (
              <Snackbar open={registerError} autoHideDuration={6000} onClose={handleClose}>
                <Alert onClose={handleClose} severity="error" sx={{ width: '100%' }}>
                  Rejestarcja nie powiodła się
                </Alert>
              </Snackbar>
            )}
            {success &&
              (
                navigate("/signin", { state: { openSnackbar: success } })
              )
            }
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