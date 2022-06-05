import { Button, Container, CssBaseline, Dialog, DialogActions, DialogContent, DialogTitle, Grid, TextField, Typography } from '@mui/material';
import { Box, createTheme } from '@mui/system';
import * as React from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { ThemeProvider } from 'styled-components';
import ValidationHelpers from '../../tools/ValidationHelpers';
import { ErrorSnackbar, SuccessSnackbar } from '../Snackbars';
import { confirmVaccination, vaccinationDidNotHappen } from './DoctorApi';

const theme = createTheme();

export default function Vaccinate() {
    const location = useLocation();
    const navigate = useNavigate();

    const [errCode, setErrCode] = React.useState()
    const [errState, setErrState] = React.useState(false)
    const [succes, setSuccess] = React.useState(false)
    const [batchError, setBatchError] = React.useState('');
    const [batchErrorState, setBatchErrorState] = React.useState(false);
    const [batchNumber, setBatchNumber] = React.useState(null)

    const [openConfirmDialog, setOpenConfrimDialog] = React.useState(false)
    const [openCancelDialog, setOpenCancelDialog] = React.useState(false)

    function handleConfrimClose() {
        setOpenConfrimDialog(false)
    }

    const handleVaccineConfirmation = async () => {
        if (batchNumber == '') return

        let code = await confirmVaccination(localStorage.getItem('doctorID'), location.state.appointmentId, batchNumber)
        if (code == '200') {
            setSuccess(true)
        }
        else {
            setErrCode(code)
            setErrState(true)
        }

        setOpenConfrimDialog(false)
    }

    const handleCancelConfirmation = async () => {
        let code = await vaccinationDidNotHappen(localStorage.getItem('doctorID'), location.state.appointmentId)
        if (code == '200') {
            setSuccess(true)
        }
        else {
            setErrCode(code)
            setErrState(true)
        }

        setOpenCancelDialog(false)
    }

    function handleUnlimited(num) {
        if (num < 0)
            return "brak limitu"
        else return num
    }

    return (
        <ThemeProvider theme={theme}>
            <Container component="main" maxWidth="lg">
                <CssBaseline />
                <Box
                    sx={{
                        margin: 8,
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                    }}
                >
                    <Typography component="h1" variant="h5" alignSelf={'center'}>
                        Szczepienie
                    </Typography>
                    <Box sx={{ m: 2 }} />

                    <Grid >

                        <Grid
                            container
                            direction="column"
                            justifyContent="center"
                            alignItems="center"
                        >
                            <Box>Pacjent: </Box>
                        </Grid>
                        <Box> {location.state.appointmentData.patientFirstName + " " + location.state.appointmentData.patientLastName} </Box>
                        <Box> {'PESEL: ' + location.state.appointmentData.PESEL} </Box>
                        <Box> {'Data urodzenia: ' + location.state.appointmentData.dateOfBirth} </Box>
                        <Box> {'Dawka szczepionki: ' + location.state.appointmentData.whichVaccineDose} </Box>
                        <Box sx={{ m: 2 }} />

                        <Grid
                            container
                            direction="column"
                            justifyContent="center"
                            alignItems="center"
                        >
                            <Box>Szczepionka: </Box>
                        </Grid>
                        <Box> {location.state.appointmentData.vaccineName} </Box>
                        <Box> {'Firma: ' + location.state.appointmentData.vaccineCompany} </Box>
                        <Box> {'Wirus: ' + location.state.appointmentData.virusName} </Box>
                        <Box> {'Wiek pacjenta: ' + handleUnlimited(location.state.appointmentData.minPatientAge) + ' - '
                            + handleUnlimited(location.state.appointmentData.maxPatientAge)} </Box>
                        <Box> {'Liczba dawek: ' + handleUnlimited(location.state.appointmentData.numberOfDoses)} </Box>
                        <Box> {'Dni pomiędzy dawkami: ' + handleUnlimited(location.state.appointmentData.minDaysBetweenDoses) + ' - '
                            + handleUnlimited(location.state.appointmentData.maxDaysBetweenDoses)} </Box>
                        <Box sx={{ m: 2 }} />

                        <Grid
                            container
                            direction="column"
                            justifyContent="center"
                            alignItems="center"
                        >
                            <Button
                                type="submit"
                                fullWidth
                                variant="contained"
                                sx={{ mt: 3, mb: 2 }}
                                onClick={() => { setOpenConfrimDialog(true) }}
                            >
                                Przeprowadź szczepienie
                            </Button>
                            <Button
                                type="submit"
                                fullWidth
                                variant="contained"
                                sx={{ mt: 3, mb: 2 }}
                                onClick={() => { setOpenCancelDialog(true) }}
                            >
                                Anuluj szczepienie
                            </Button>
                            <Button
                                type="submit"
                                fullWidth
                                variant="contained"
                                sx={{ mt: 3, mb: 2 }}
                                onClick={() => navigate('/doctor/incomingAppointments')}
                            >
                                Wróć
                            </Button>

                        </Grid>

                    </Grid>
                </Box>

                <Dialog
                    open={openConfirmDialog}
                    onClose={handleConfrimClose}
                    aria-labelledby="alert-dialog-title"
                    aria-describedby="alert-dialog-description"
                >
                    <DialogTitle id="alert-dialog-title">
                        {"Wpisz numer partii szczepionki i potwierdź szczepienie"}
                    </DialogTitle>
                    <DialogContent>
                        <Box
                            component='form'
                            noValidate
                            sx={{
                                marginTop: 2,
                                marginBottom: 2,
                                display: 'flex',
                            }}
                        >
                            <Grid item xs={3}>
                                <TextField
                                    fullWidth
                                    id="batchNumber"
                                    label="Numer Partii"
                                    name="batchNumber"
                                    error={batchErrorState}
                                    helperText={batchError}
                                    onChange={(e) => { ValidationHelpers.validateInt(e, setBatchError, setBatchErrorState); setBatchNumber(e.target.value) }}
                                />
                            </Grid>
                        </Box>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={handleConfrimClose}>Nie</Button>
                        <Button onClick={handleVaccineConfirmation} autoFocus>
                            Tak
                        </Button>
                    </DialogActions>
                </Dialog>

                <Dialog
                    open={openCancelDialog}
                    aria-labelledby="alert-dialog"
                    aria-describedby="alert-dialog"
                >
                    <DialogTitle id="alert-dialog-title">
                        {"Czy potwierdzasz, że szczepienie nie powiodło się?"}
                    </DialogTitle>
                    <DialogActions>
                        <Button onClick={() => setOpenCancelDialog(false)}>Nie</Button>
                        <Button onClick={handleCancelConfirmation} autoFocus>
                            Tak
                        </Button>
                    </DialogActions>
                </Dialog>

                <ErrorSnackbar
                    error={errCode}
                    errorState={errState}
                    setErrorState={setErrState}
                />
                <SuccessSnackbar
                    success={succes}
                    setSuccess={setSuccess}
                />
            </Container>
        </ThemeProvider >
    );
}