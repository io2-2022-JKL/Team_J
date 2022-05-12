import * as React from 'react';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import TextField from '@mui/material/TextField';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import ValidationHelpers from '../../../tools/ValidationHelpers';

const theme = createTheme();

export default function PatientForm({ handleFormClose }, { handleFormSubmit }, row) {
    //const [peselErrorMessage, setPeselErrorMessage] = React.useState('')
    //const [peselErrorState, setPeselErrorState] = React.useState(false)

    /*const [firstaNameErrorMessage, setFirstaNameErrorMessage] = React.useState('')
    const [firstNameErrorState, setFirstNameErrorState] = React.useState(false)
    const [lastNameErrorMessage, setLastNameErrorMessage] = React.useState('')
    const [lastNameErrorState, setLastNameErrorState] = React.useState(false)
    const [mailErrorMessage, setMailErrorMessage] = React.useState('')
    const [mailErrorState, setMailErrorState] = React.useState(false)*/
    //const [phoneNumberErrorMessage, setPhoneNumberErrorMessage] = React.useState('')
    //const [phoneNumberErrorState, setPhoneNumberErrorState] = React.useState(false)

    //const [phoneNumber, setPhoneNumber] = React.useState('')

    //React.useEffect(() => {
    //setPhoneNumber(row.phoneNumber)
    //}, []);

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
                        Wpisz dane pacjenta
                    </Typography>
                    <Box component="form" noValidate sx={{ mt: 3 }}>
                        <Grid container spacing={2}>
                            <Grid item xs={12}>
                                <TextField
                                    //value={row.id}
                                    name="id"
                                    required
                                    fullWidth
                                    id="id"
                                    label="Id pacjenta"
                                    autoFocus
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    //value={row.pesel}
                                    name="pesel"
                                    required
                                    fullWidth
                                    id="pesel"
                                    label="PESEL"
                                    autoFocus
                                //onChange={(e) => ValidationHelpers.validatePESEL(e, setPeselErrorMessage, setPeselErrorState)}
                                //helperText={peselErrorMessage}
                                //error={peselErrorState}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    //value={row.firstName}
                                    name="firstName"
                                    required
                                    fullWidth
                                    id="firstName"
                                    label="Imię"
                                    autoFocus
                                //onChange={(e) => ValidationHelpers.validateFirstName(e, setFirstaNameErrorMessage, setFirstNameErrorState)}
                                //helperText={firstaNameErrorMessage}
                                //error={firstNameErrorState}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    //value={row.lastName}
                                    required
                                    fullWidth
                                    id="lastName"
                                    label="Nazwisko"
                                    name="lastName"
                                //onChange={(e) => ValidationHelpers.validateLastName(e, setLastNameErrorMessage, setLastNameErrorState)}
                                //helperText={lastNameErrorMessage}
                                //error={lastNameErrorState}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    //value={row.mail}
                                    required
                                    fullWidth
                                    id="email"
                                    label="Adres Email"
                                    name="email"
                                    autoComplete="email"
                                    type="email"
                                //onChange={(e) => ValidationHelpers.validateEmail(e, setMailErrorMessage, setMailErrorState)}
                                //helperText={mailErrorMessage}
                                //error={mailErrorState}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    //value={row.dateOfBirth}
                                    required
                                    fullWidth
                                    name="dateOfBirth"
                                    label="Data urodzenia"
                                    id="dateOfBirth"

                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    //value={row.phoneNumber}
                                    name="phoneNumber"
                                    required
                                    fullWidth
                                    id="phoneNumber"
                                    label="Numer telefonu"
                                    type="tel"
                                //onChange={(e) => ValidationHelpers.validatePhoneNumber(e, setPhoneNumberErrorMessage, setPhoneNumberErrorState)}
                                //helperText={phoneNumberErrorMessage}
                                //error={phoneNumberErrorState}
                                />
                            </Grid>
                        </Grid>
                    </Box>
                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        sx={{ mt: 3, mb: 2 }}
                        onClick={() => { handleFormSubmit() }}
                    >
                        Zatwierdź
                    </Button>
                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        sx={{ mt: 3, mb: 2 }}
                        onClick={() => { handleFormClose() }}
                    >
                        Powrót
                    </Button>
                </Box>
            </Container>
        </ThemeProvider>
    )
}