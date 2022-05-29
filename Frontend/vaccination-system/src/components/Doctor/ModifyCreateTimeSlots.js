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
import { DatePicker, LocalizationProvider, TimePicker, StaticTimePicker } from '@mui/x-date-pickers';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { activeOptions } from '../../tools/ActiveOptions';
import { ErrorSnackbar, SuccessSnackbar } from '../../tools/Snackbars';
import { modifyTimeSlots } from './DoctorApi';

const theme = createTheme();

export default function ModifyCreateTimeSlot() {

    const navigate = useNavigate();
    const location = useLocation();
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
    const [activeOption, setActiveOption] = React.useState(location.state != null ? location.state.active ? 'aktywny' : 'nieaktywny' : '');
    const [success, setSuccess] = useState(false);
    const [date, setDate] = useState(location.state != null ? new Date(location.state.date.split('-')[1] + '/' + location.state.date.split('-')[0] + '/' + location.state.date.split('-')[2]) : null);
    const [timeFrom, setTimeFrom] = useState(location.state != null ? new Date(location.state.date.split('-')[2], location.state.date.split('-')[0], location.state.date.split('-')[1], location.state.timeFrom.split(':')[0], location.state.timeFrom.split(':')[1]) : null);
    const [timeTo, setTimeTo] = useState(location.state != null ? new Date(location.state.date.split('-')[2], location.state.date.split('-')[0], location.state.date.split('-')[1], location.state.timeTo.split(':')[0], location.state.timeTo.split(':')[1]) : null);

    const handleSubmit = async (event) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        console.log({
            doctorID: location.state.doctorId,
            timeSlotId: location.state.timeSlotId,
            date: data.get('date'),
            timeFrom: data.get('timeFrom'),
            timeTo: data.get('timeTo')
        })
                
        let error = await modifyTimeSlots(location.state.doctorId, location.state.timeSlotId, data.get('date')+" "+data.get('timeFrom'),data.get('date')+" "+data.get('timeTo'));
        setOperationError(error);
        if (error != '200')
            setOperationErrorState(true);
        else
            setSuccess(true);
        
        /*
         const pom = location.state.date.split('-')
         const pom2 = location.state.timeFrom.split(':')
         console.log({
             loc: Date(location.state.date),
             loc2: location.state.date,
             pim: new Date(pom[1] + '/' + pom[0] + '/' + pom[2])
         })
         console.log(date)
         //console.log('po replace:')
         console.log({
             loc: new Date(pom[2],pom[0],pom[1],pom2[0],pom2[1]),
             loc2: location.state.timeFrom
         })
         console.log(timeFrom)
         */
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
                    <Typography component="h1" variant="h5">
                        Wpisz dane okna godzinowego
                    </Typography>
                    <Box component="form" noValidate onSubmit={handleSubmit} sx={{ mt: 3 }}>
                        <Grid container spacing={2}>
                            <Grid item xs={12}>
                                <LocalizationProvider dateAdapter={AdapterDateFns}>
                                    <DatePicker
                                        label="Data okna"
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
                                            id='date'
                                            name='date'

                                        />}
                                    />
                                </LocalizationProvider>
                            </Grid>
                            <Grid item xs={12}>
                                <LocalizationProvider dateAdapter={AdapterDateFns}>
                                    <TimePicker
                                        label="Początek okna"
                                        ampm={false}
                                        value={timeFrom}
                                        onChange={(newDate) => {
                                            setTimeFrom(newDate);
                                        }}
                                        renderInput={(params) => <TextField
                                            {...params}
                                            fullWidth
                                            id='timeFrom'
                                            name='timeFrom'
                                        />}
                                    />
                                </LocalizationProvider>
                            </Grid>
                            <Grid item xs={12}>
                                <LocalizationProvider dateAdapter={AdapterDateFns}>
                                    <TimePicker
                                        label="Koniec okna"
                                        ampm={false}
                                        value={timeTo}
                                        onChange={(newDate) => {
                                            setTimeTo(newDate);
                                        }}
                                        renderInput={(params) => <TextField
                                            {...params}
                                            fullWidth
                                            id='timeTo'
                                            name='timeTo'
                                        />}
                                    />
                                </LocalizationProvider>
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
                        onClick={() => { navigate("/doctor/timeSlots") }}
                    >
                        Powrót
                    </Button>
                    <ErrorSnackbar
                        error={operationError}
                        errorState={operationErrorState}
                        setErrorState={setOperationErrorState}
                    />
                    <SuccessSnackbar
                        success={success}
                        setSuccess={setSuccess}
                    />
                </Box>
            </Container>
        </ThemeProvider>

    )
}