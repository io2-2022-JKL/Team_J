import * as React from 'react';
import { useState } from 'react';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import TextField from '@mui/material/TextField';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { useLocation, useNavigate } from "react-router-dom";
import ValidationHelpers from '../../tools/ValidationHelpers';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';

const theme = createTheme();

const Alert = React.forwardRef(function Alert(props, ref) {
    return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
});

export default function EditDoctor(action) {
    const navigate = useNavigate();
    const [peselError, setPeselError] = useState('')
    const [peselErrorState, setPeselErrorState] = useState(false)
    const [firstNameError, setFirstNameError] = useState('')
    const [firstNameErrorSate, setFirstNameErrorState] = useState(false)
    const [lastNameError, setLastNameError] = useState('')
    const [lastNameErrorState, setLastNameErrorState] = useState(false)
    const [mailError, setMailError] = useState('')
    const [mailErrorState, setMailErrorState] = useState(false)
    //const [dateOfBirth, setDateOfBirth] = useState(location.state != null ? location.state.dateOfBirth ? location.state.dateOfBirth.replaceAll('-', '/') : '' : '')
    const [dateOfBirthError, setDateOfBirthError] = useState('')
    const [dateOfBirthErrorState, setDateOfBirthErrorState] = useState(false)
    const [phoneNumberError, setPhoneNumberError] = useState('')
    const [phoneNumberErrorState, setPhoneNumberErrorState] = useState(false)

    const [operationError, setOperationError] = useState('');
    const [operationErrorState, setOperationErrorState] = useState(false);
    const [success, setSuccess] = useState(false);
    const location = useLocation();

    const handleSubmit = async (event) => {

        event.preventDefault();
        const data = new FormData(event.currentTarget);

        let error;
        /*if (action === "add")
            error = await addVaccine(data.get('company'), data.get('name'), Number.parseInt(data.get('numberOfDoses')),
                Number.parseInt(data.get('minDaysBetweenDoses')), Number.parseInt(data.get('maxDaysBetweenDoses')),
                data.get('virus'), Number.parseInt(data.get('minPatientAge')), Number.parseInt(data.get('maxPatientAge')),
                data.get('active'));
        else if (action === "edit")
            //editVaccine    
            error = await editVaccine(location.state.id, data.get('company'), data.get('name'), Number.parseInt(data.get('numberOfDoses')),
                Number.parseInt(data.get('minDaysBetweenDoses')), Number.parseInt(data.get('maxDaysBetweenDoses')),
                data.get('virus'), Number.parseInt(data.get('minPatientAge')), Number.parseInt(data.get('maxPatientAge')),
                data.get('active'));*/
        setOperationError(error);
        if (error != '200')
            setOperationErrorState(true);
        else
            setSuccess(true);
    };

    const [activeOption, setActiveOption] = React.useState(location.state != null ? location.state.active ? 'aktywny' : 'nieaktywny' : '');

    const handleChange = (event) => {
        setActiveOption(event.target.value);
    };

    const activeOptions = [
        {
            value: 'aktywny',
            label: 'aktywny',
        },
        {
            value: 'nieaktywny',
            label: 'nieaktywny',
        },
    ];

    const handleClose = (event, reason) => {
        if (reason === 'clickaway') {
            return;
        }
        setOperationErrorState(false);
    };

    const handleClose2 = (event, reason) => {
        if (reason === 'clickaway') {
            return;
        }
        setSuccess(false);
    };

    function renderError(param) {
        switch (param) {
            case '400':
                return 'Złe dane. Czyżbyś próbował dodać nieistniejącego wirusa?';
            case '401':
                return 'Użytkownik nieuprawniony do dodania szczepionki'
            case '403':
                return 'Użytkownikowi zabroniono dodawania szczepionki'
            case '404':
                return 'Nie znaleziono szczepionki do dodania'
            default:
                return 'Wystąpił błąd!';
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
                    <Typography component="h1" variant="h5">
                        Wpisz dane lekarza
                    </Typography>
                    <Box component="form" noValidate onSubmit={handleSubmit} sx={{ mt: 3 }}>
                        <Grid container spacing={2}>
                            <Grid item xs={12}>
                                <TextField
                                    defaultValue={location.state != null ? location.state.pesel : null}
                                    required
                                    fullWidth
                                    name="pesel"
                                    label="PESEL"
                                    id="pesel"
                                    onChange={(e) => {
                                        ValidationHelpers.validatePESEL(e, setPeselError, setPeselErrorState)
                                    }}
                                    helperText={peselError}
                                    error={peselErrorState}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    defaultValue={location.state != null ? location.state.firstName : null}
                                    required
                                    fullWidth
                                    name="firtsName"
                                    label="Imię"
                                    id="firtsName"
                                    onChange={(e) => {
                                        ValidationHelpers.validateFirstName(e, setFirstNameError, setFirstNameErrorState)
                                    }}
                                    helperText={firstNameError}
                                    error={firstNameErrorSate}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    defaultValue={location.state != null ? location.state.lastName : null}
                                    required
                                    fullWidth
                                    name="lastName"
                                    label="Nazwisko"
                                    id="lastName"
                                    onChange={(e) => {
                                        ValidationHelpers.validateLastName(e, setLastNameError, setLastNameErrorState)
                                    }}
                                    helperText={lastNameError}
                                    error={lastNameErrorState}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    defaultValue={location.state != null ? location.state.mail : null}
                                    required
                                    fullWidth
                                    name="mail"
                                    label="E-Mail"
                                    id="mail"
                                    onChange={(e) => {
                                        ValidationHelpers.validateEmail(e, setMailError, setMailErrorState)
                                    }}
                                    helperText={mailError}
                                    error={mailErrorState}
                                />
                            </Grid>
                            {/*<Grid item xs={12}>
                                <LocalizationProvider dateAdapter={AdapterDateFns}>
                                    <DatePicker
                                        label="Data urodzenia"
                                        views={['year', 'month', 'day']}
                                        inputFormat="dd-MM-yyyy"
                                        mask="__-__-____"
                                        value={dateOfBirth}
                                        minDate={new Date("01/01/1900")}
                                        maxDate={new Date()}
                                        onChange={(newDate) => {
                                            setDateOfBirth(newDate);
                                        }}
                                        renderInput={(params) => <TextField
                                            {...params}
                                            fullWidth
                                            id='dateOfBirth'
                                            name='dateOfBirth'
                                        />}
                                    />
                                </LocalizationProvider>
                                        </Grid>*/}
                            <Grid item xs={12}>
                                <TextField
                                    defaultValue={location.state != null ? location.state.dateOfBirth : null}
                                    required
                                    fullWidth
                                    name="dateOfBirth"
                                    label="Data urodzenia"
                                    id="dateOfBirth"
                                    onChange={(e) => {
                                        ValidationHelpers.validateDate(e, setDateOfBirthError, setDateOfBirthErrorState)
                                    }}
                                    helperText={dateOfBirthError}
                                    error={dateOfBirthErrorState}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    defaultValue={location.state != null ? location.state.phoneNumber : null}
                                    required
                                    fullWidth
                                    name="phoneNumber"
                                    label="Numer telefonu"
                                    id="phoneNumber"
                                    onChange={(e) => {
                                        ValidationHelpers.validatePhoneNumber(e, setPhoneNumberError, setPhoneNumberErrorState)
                                    }}
                                    helperText={phoneNumberError}
                                    error={phoneNumberErrorState}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    fullWidth
                                    id="active"
                                    select
                                    label="Aktywny"
                                    name="active"
                                    value={activeOption}
                                    onChange={handleChange}
                                    SelectProps={{
                                        native: true,
                                    }}
                                >
                                    {activeOptions.map((option) => (
                                        <option key={option.value} value={option.value}>
                                            {option.label}
                                        </option>
                                    ))}
                                </TextField>
                            </Grid>
                        </Grid>
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                        >
                            Zatwierdź
                        </Button>
                    </Box>
                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        sx={{ mt: 3, mb: 2 }}
                        onClick={() => { navigate("/admin/doctors") }}
                    >
                        Powrót
                    </Button>
                    <Snackbar open={operationErrorState} autoHideDuration={6000} onClose={handleClose}>
                        <Alert onClose={handleClose} severity="error" sx={{ width: '100%' }}>
                            {renderError(operationError)}
                        </Alert>
                    </Snackbar>
                    <Snackbar open={success} autoHideDuration={6000} onClose={handleClose2}>
                        <Alert onClose={handleClose2} severity="success" sx={{ width: '100%' }}>
                            Akcja wykonana pomyślnie
                        </Alert>
                    </Snackbar>
                </Box>
            </Container>
        </ThemeProvider>

    )
}