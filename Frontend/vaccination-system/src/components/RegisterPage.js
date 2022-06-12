import * as React from 'react';
import { useState } from 'react';
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
import axios from 'axios';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import CircularProgress from '@mui/material/CircularProgress';
import { blue } from '@mui/material/colors';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';
import { SYSTEM_SZCZEPIEN_URL } from '../api/Api';
import ValidationHelpers from '../tools/ValidationHelpers';

const theme = createTheme();

const Alert = React.forwardRef(function Alert(props, ref) {
  return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
});

export default function SignUp() {
  const [emailError, setEmailError] = useState('');
  const [emailErrorState, setEmailErrorState] = useState(false);
  const [password, setPassword] = useState('');
  const [password2, setPassword2] = useState('');
  const [password2Error, setPassword2Error] = useState('');
  const [password1Error, setPassword1Error] = useState('');
  const [password1ErrorState, setPassword1ErrorState] = useState(false);
  const [password2ErrorState, setPassword2ErrorState] = useState(false);
  const [loading, setLoading] = useState(false);
  const [date, setDate] = useState(null);
  const [registerError, setRegisterError] = useState(false);
  const [success, setSuccess] = useState(false);
  const [peselError, setPeselError] = useState('');
  const [peselErrorState, setPeselErrorState] = useState(false);
  const [firstNameError, setFirstNameError] = useState('');
  const [firstNameErrorState, setFirstNameErrorState] = useState(false);
  const [lastNameError, setLastNameError] = useState('');
  const [lastNameErrorState, setLastNameErrorState] = useState(false);
  const [phoneNumberError, setPhoneNumberError] = useState('');
  const [phoneNumberErrorState, setPhoneNumberErrorState] = useState('');

  const navigate = useNavigate();

  const signInUser = async (pesel, firstName, lastName, mail, dateOfBirth, password, phoneNumber) => {
    // request i response
    if (emailErrorState) return;
    if (password2ErrorState) return;
    if (password1ErrorState) return;
    if (peselErrorState) return;
    if (firstNameErrorState) return;
    if (lastNameErrorState) return;

    setLoading(true);
    try {
      const { data: response } = await axios({
        method: 'post',
        url: SYSTEM_SZCZEPIEN_URL + '/register',
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
                  onChange={(e) => ValidationHelpers.validatePESEL(e, setPeselError, setPeselErrorState)}
                  helperText={peselError}
                  error={peselErrorState}
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
                  onChange={(e) => ValidationHelpers.validateName(e, setFirstNameError, setFirstNameErrorState)}
                  helperText={firstNameError}
                  error={firstNameErrorState}
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
                  onChange={(e) => ValidationHelpers.validateLastName(e, setLastNameError, setLastNameErrorState)}
                  helperText={lastNameError}
                  error={lastNameErrorState}
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
                  onChange={(e) => ValidationHelpers.validateEmail(e, setEmailError, setEmailErrorState)}
                  helperText={emailError}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  error={password1ErrorState}
                  required
                  fullWidth
                  name="password"
                  label="Hasło"
                  type="password"
                  id="password"
                  autoComplete="new-password"
                  onChange={(e) => {
                    setPassword(e.target.value);
                    ValidationHelpers.validatePassword(e, setPassword1Error, setPassword1ErrorState)
                  }}
                  helperText={password1Error}
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
                  onChange={(e) => {
                    setPassword2(e.target.value);
                    ValidationHelpers.validateRepeatPassword(e, setPassword2Error, setPassword2ErrorState, password)
                  }}
                  helperText={password2Error}
                  disabled={password1ErrorState || password.length === 0}
                  value={password2}
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
                    minDate={new Date("01/01/1900")}
                    maxDate={new Date()}
                    onChange={(newDate) => {
                      setDate(newDate);
                    }}
                    //onError={()=>{setDateError(true)}}
                    renderInput={(params) => <TextField
                      {...params}
                      fullWidth
                      id='dateOfBirth'
                      name='dateOfBirth'
                    //onBlur={() => {setDateError(false)}}
                    //helperText = {dateError?"Błędna data":""}    
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
                  //type="tel"
                  onChange={(e) => ValidationHelpers.validatePhoneNumber(e, setPhoneNumberError, setPhoneNumberErrorState)}
                  helperText={phoneNumberError}
                  error={phoneNumberErrorState}
                />
              </Grid>
            </Grid>
            <Button
              type="submit"
              fullWidth
              variant="contained"
              disabled={loading}
              sx={{ mt: 3, mb: 2 }}
              name="registerButton"
            >
              Zarejestruj się
            </Button>
            {loading && (
              <CircularProgress
                size={24}
                sx={{
                  color: blue,
                  position: 'relative',
                  alignSelf: 'center',
                  left: '50%',
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