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
import { DatePicker, LocalizationProvider, TimePicker, StaticTimePicker, DateTimePicker } from '@mui/x-date-pickers';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { ErrorSnackbar, SuccessSnackbar } from '../Snackbars';
import { createTimeSlots, modifyTimeSlots } from './DoctorApi';

const theme = createTheme();

export default function ModifyCreateTimeSlot() {

    const navigate = useNavigate();
    const location = useLocation();
    const [operationError, setOperationError] = useState('');
    const [operationErrorState, setOperationErrorState] = useState(false);
    const [success, setSuccess] = useState(false);
    const [timeFrom, setTimeFrom] = useState(location.state != null && location.state.dateFrom != null && location.state.timeFrom != null ? new Date(location.state.dateFrom.split('-')[2], location.state.dateFrom.split('-')[1] - 1, location.state.dateFrom.split('-')[0], location.state.timeFrom.split(':')[0], location.state.timeFrom.split(':')[1]) : null);
    const [timeTo, setTimeTo] = useState(location.state != null && location.state.dateTo != null && location.state.timeTo != null ? new Date(location.state.dateTo.split('-')[2], location.state.dateTo.split('-')[1] - 1, location.state.dateTo.split('-')[0], location.state.timeTo.split(':')[0], location.state.timeTo.split(':')[1]) : null);

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

        let error
        if (location.state.action === "modify")
            error = await modifyTimeSlots(location.state.doctorId, location.state.timeSlotId, data.get('timeFrom'), data.get('timeTo'));
        else
            error = await createTimeSlots(location.state.doctorId, data.get('timeFrom'), data.get('timeTo'), Number.parseInt(data.get('duration')));
        setOperationError(error);
        if (error != '200')
            setOperationErrorState(true);
        else
            setSuccess(true);

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
                                    <DateTimePicker
                                        label="Początek okna"
                                        views={['year', 'month', 'day', 'hours', 'minutes']}
                                        ampm={false}
                                        inputFormat="dd-MM-yyyy HH:mm"
                                        mask="__-__-____ __:__"
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
                                    <DateTimePicker
                                        label="Koniec okna"
                                        views={['year', 'month', 'day', 'hours', 'minutes']}
                                        ampm={false}
                                        inputFormat="dd-MM-yyyy HH:mm"
                                        mask="__-__-____ __:__"
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
                            {location.state.action === "create" &&
                                <Grid item xs={12}>
                                    <TextField
                                        fullWidth
                                        id="duration"
                                        name="duration"
                                        label="Czas okna w minutach"
                                    />
                                </Grid>
                            }
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