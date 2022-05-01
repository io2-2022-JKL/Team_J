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
  const [peselError,setPeselError] = useState('');
  const [peselErrorState,setPeselErrorState] = useState(false);
  const [firstNameError,setFirstNameError] = useState('');
  const [firstNameErrorState,setFirstNameErrorState] = useState(false);
  const [lastNameError,setLastNameError] = useState('');
  const [lastNameErrorState,setLastNameErrorState] = useState(false);
  //const [dateError,setDateError] = useState(false);
  const [phoneNumberError,setPhoneNumberError] = useState('');
  const [phoneNumberErrorState,setPhoneNumberErrorState] = useState('');

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
  const validatePESEL = (e) => {
    var pesel = e.target.value;
    if(pesel.length === 11 && validator.isNumeric(pesel))
    {
      setPeselError('')
      setPeselErrorState(false)
    }
    else
    {
      setPeselError('Wprowadź poprawny PESEL!')
      setPeselErrorState(true)
    }
  }
  const validateFirstName = (e) => {
    var name = e.target.value;
    const substrings = ['!','@','#','$','%','^','&','*','(',')','_','+','=','[','{',']','}',';',':','"','\\','|','<','>',',','.','/']
    if(name.length > 0 && !substrings.some(c => name.includes(c)))
    {
      setFirstNameError('')
      setFirstNameErrorState(false)
    }
    else
    {
      setFirstNameError('Imię nie może zawierać symboli !@#$%^&*()_+=[{]};:"\|<>,./ i musi mieć przynajmniej jeden znak!')
      setFirstNameErrorState(true)
    }
  }
  const validateLastName = (e) => {
    var name = e.target.value;
    const substrings = ['!','@','#','$','%','^','&','*','(',')','_','+','=','[','{',']','}',';',':','"','\\','|','<','>',',','.','/']
    if(name.length > 0 && !substrings.some(c => name.includes(c)))
    {
      setLastNameError('')
      setLastNameErrorState(false)
    }
    else
    {
      setLastNameError('Nazwisko nie może zawierać symboli !@#$%^&*()_+=[{]};:"|<>,./\\ i musi mieć przynajmniej jeden znak!')
      setLastNameErrorState(true)
    }
  }
  const validatePassword1 = (e) => {
    var password1 = e.target.value;
    setPassword(password1)
    setPassword2('')
    if (password1.length > 0) {
      setPassword1Error('')
      setPassword1ErrorState(false)
    }
    else {
      setPassword1Error('Hasło musi mieć co najmniej 1 znak')
      setPassword1ErrorState(true)
    }
  }
  const validatePassword2 = (e) => {
    var password2 = e.target.value;
    setPassword2(password2);
    if (validator.equals(password2, password)) {
      setPassword2Error('')
      setPassword2ErrorState(false)
    }
    else {
      setPassword2Error('Hasła się nie pokrywają ze sobą')
      setPassword2ErrorState(true)
    }
  }
  
  const validatePhoneNumber = (e) => {
    var number = e.target.value;
    var phoneNumberWithoutSpaces = number.replace(/ /g,"");
    var regEx = /^[+]?[0-9]*$/
    if(number.length > 0 && phoneNumberWithoutSpaces.length <= 15 && regEx.test(phoneNumberWithoutSpaces))
    {
      setPhoneNumberError('');
      setPhoneNumberErrorState(false);
    }
    else
    {
      setPhoneNumberError('Niepoprawny numer telefonu!');
      setPhoneNumberErrorState(true);
    }
  }
  
  const signInUser = async (pesel, firstName, lastName, mail, dateOfBirth, password, phoneNumber) => {
    // request i response
    if (emailErrorState) return;
    if (password2ErrorState) return;
    if (password1ErrorState) return;
    if (peselErrorState) return;
    if(firstNameErrorState) return;
    if(lastNameErrorState) return;
    //if(dateError) return;

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
                  onChange={(e) => validatePESEL(e)}
                  helperText = {peselError}
                  error = {peselErrorState}
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
                  onChange={(e) => validateFirstName(e)}
                  helperText = {firstNameError}
                  error = {firstNameErrorState}
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
                  onChange={(e) => validateLastName(e)}
                  helperText = {lastNameError}
                  error = {lastNameErrorState}
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
                  error={password1ErrorState}
                  required
                  fullWidth
                  name="password"
                  label="Hasło"
                  type="password"
                  id="password"
                  autoComplete="new-password"
                  onChange={(e) => validatePassword1(e)}
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
                  onChange={(e) => validatePassword2(e)}
                  helperText={password2Error}
                  disabled = {password1ErrorState || password.length === 0}
                  value = {password2}
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
                    minDate = {new Date("01/01/1900")}
                    maxDate = {new Date()}
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
                  onChange={(e) => validatePhoneNumber(e)}
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
            >
              Zarejestruj się
            </Button>
            {loading && (
              <CircularProgress
                size={24}
                sx={{
                  color: blue,
                  position:'relative',
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