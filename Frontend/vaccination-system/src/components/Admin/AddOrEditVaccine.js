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
import { addVaccine } from './AdminApi';
import ValidationHelpers from '../../tools/ValidationHelpers';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';

const theme = createTheme();

const Alert = React.forwardRef(function Alert(props, ref) {
    return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
});

export default function AddOrEditVaccine(action, ...props) {
    const navigate = useNavigate();
    const [nODErrorState, setNODErrorState] = useState(false);
    const [nODError, setNODError] = useState('');
    const [minDBDErrorState, setMinDBDErrorState] = useState(false);
    const [minDBDError, setMinDBDError] = useState('');
    const [maxDBDErrorState, setMaxDBDErrorState] = useState(false);
    const [maxDBDError, setMaxDBDError] = useState('');
    const [minDBDErrorState2, setMinDBDErrorState2] = useState(false);
    const [minDBDError2, setMinDBDError2] = useState('');
    const [maxDBDErrorState2, setMaxDBDErrorState2] = useState(false);
    const [maxDBDError2, setMaxDBDError2] = useState('');
    const [minPAErrorState, setMinPAErrorState] = useState(false);
    const [minPAError, setMinPAError] = useState('');
    const [maxPAErrorState, setMaxPAErrorState] = useState(false);
    const [maxPAError, setMaxPAError] = useState('');
    const [minPAErrorState2, setMinPAErrorState2] = useState(false);
    const [minPAError2, setMinPAError2] = useState('');
    const [maxPAErrorState2, setMaxPAErrorState2] = useState(false);
    const [maxPAError2, setMaxPAError2] = useState('');
    const [minDBD, setMinDBD] = useState(0);
    const [maxDBD, setMaxDBD] = useState(0);
    const [minPA, setMinPA] = useState(0);
    const [maxPA, setMaxPA] = useState(0);
    const [addError, setError] = useState('');
    const [addErrorState, setErrorState] = useState(false);
    const [success, setSuccess] = useState(false);

    const [company, setCompany] = useState('')

    React.useEffect(() => {

        if (action === "edit") {
            setCompany(props.company)
        }

        if (maxDBD >= 0 && maxDBD < minDBD) {
            setMinDBDErrorState2(true);
            setMaxDBDErrorState2(true);
            setMaxDBDError2("Wartość maksymalna jest mniejsza od minimalnej!");
            setMinDBDError2("Wartość minimalna jest większa od maksymalnej!");
        }
        else {
            setMinDBDErrorState2(false);
            setMaxDBDErrorState2(false);
            setMaxDBDError2("");
            setMinDBDError2("");
        }

        if (maxPA >= 0 && maxPA < minPA) {
            setMinPAErrorState2(true);
            setMaxPAErrorState2(true);
            setMaxPAError2("Wartość maksymalna jest mniejsza od minimalnej!");
            setMinPAError2("Wartość minimalna jest większa od maksymalnej!");
        }
        else {
            setMinPAErrorState2(false);
            setMaxPAErrorState2(false);
            setMaxPAError2("");
            setMinPAError2("");
        }

    }, [minDBD, maxDBD, minPA, maxPA]);

    const handleSubmit = async (event) => {
        if (minPAErrorState || minPAErrorState2 || minDBDErrorState || minDBDErrorState2 || maxPAErrorState || maxPAErrorState2 || maxDBDErrorState || maxDBDErrorState2 || nODErrorState)
            return
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        console.log({
            company: data.get('company'),
            name: data.get('name'),
            numberOfDoses: Number.parseInt(data.get('numberOfDoses')),
            minDBD: Number.parseInt(data.get('minDaysBetweenDoses')),
            maxDBD: Number.parseInt(data.get('maxDaysBetweenDoses')),
            virus: data.get('virus'),
            minPA: Number.parseInt(data.get('minPatientAge')),
            maxPA: Number.parseInt(data.get('maxPatientAge')),
            active: data.get('active')
        });

        let error;
        if (action === "add")
            error = await addVaccine(data.get('company'), data.get('name'), Number.parseInt(data.get('numberOfDoses')),
                Number.parseInt(data.get('minDaysBetweenDoses')), Number.parseInt(data.get('maxDaysBetweenDoses')),
                data.get('virus'), Number.parseInt(data.get('minPatientAge')), Number.parseInt(data.get('maxPatientAge')),
                data.get('active'));
        else if (error === "edit")
            //editVaccine    
            error = await addVaccine(data.get('company'), data.get('name'), Number.parseInt(data.get('numberOfDoses')),
                Number.parseInt(data.get('minDaysBetweenDoses')), Number.parseInt(data.get('maxDaysBetweenDoses')),
                data.get('virus'), Number.parseInt(data.get('minPatientAge')), Number.parseInt(data.get('maxPatientAge')),
                data.get('active'));
        setError(error);
        if (error != '200')
            setErrorState(true);
        else
            setSuccess(true);
    };

    const [activeOption, setActiveOption] = React.useState('');

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
        setErrorState(false);
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
                        Wpisz dane szczepionki
                    </Typography>
                    <Box component="form" noValidate onSubmit={handleSubmit} sx={{ mt: 3 }}>
                        <Grid container spacing={2}>
                            <Grid item xs={12}>
                                <TextField
                                    defaultValue={company}
                                    name="company"
                                    required
                                    fullWidth
                                    id="company"
                                    label="Firma"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    defaultValue={action == 'edit' ? props.name : null}
                                    required
                                    fullWidth
                                    id="name"
                                    label="Nazwa"
                                    name="name"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    required
                                    fullWidth
                                    id="numberOfDoses"
                                    label="Liczba dawek"
                                    name="numberOfDoses"
                                    onChange={(e) => ValidationHelpers.validateNumberOfDoses(e, setNODError, setNODErrorState)}
                                    helperText={nODError}
                                    error={nODErrorState}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    required
                                    fullWidth
                                    name="minDaysBetweenDoses"
                                    label="Minimalna liczba dni pomiędzy dawkami"
                                    id="minDaysBetweenDoses"
                                    onChange={(e) => {
                                        setMinDBD(Number.parseInt(e.target.value))
                                        ValidationHelpers.validateInt(e, setMinDBDError, setMinDBDErrorState)
                                    }}
                                    helperText={minDBDError + "\n" + minDBDError2}
                                    error={minDBDErrorState || minDBDErrorState2}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    required
                                    fullWidth
                                    name="maxDaysBetweenDoses"
                                    label="Maksymalna liczba dni pomiędzy dawkami"
                                    id="maxDaysBetweenDoses"
                                    onChange={(e) => {
                                        setMaxDBD(Number.parseInt(e.target.value))
                                        ValidationHelpers.validateInt(e, setMaxDBDError, setMaxDBDErrorState)
                                    }}
                                    helperText={maxDBDError + "\n" + maxDBDError2}
                                    error={maxDBDErrorState || maxDBDErrorState2}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    name="virus"
                                    required
                                    fullWidth
                                    id="virus"
                                    label="Nazwa wirusa"
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    required
                                    fullWidth
                                    name="minPatientAge"
                                    label="Minimalny wiek pacjenta"
                                    id="minPatientAge"
                                    onChange={(e) => {
                                        setMinPA(Number.parseInt(e.target.value));
                                        ValidationHelpers.validateInt(e, setMinPAError, setMinPAErrorState)
                                    }}
                                    helperText={minPAError + "\n" + minPAError2}
                                    error={minPAErrorState || minPAErrorState2}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    required
                                    fullWidth
                                    name="maxPatientAge"
                                    label="Maksymalny wiek pacjenta"
                                    id="maxPatientAge"
                                    onChange={(e) => {
                                        setMaxPA(Number.parseInt(e.target.value));
                                        ValidationHelpers.validateInt(e, setMaxPAError, setMaxPAErrorState)
                                    }}
                                    helperText={maxPAError + "\n" + maxPAError2}
                                    error={maxPAErrorState || maxPAErrorState2}
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
                        onClick={() => { navigate("/admin/vaccines") }}
                    >
                        Powrót
                    </Button>
                    <Snackbar open={addErrorState} autoHideDuration={6000} onClose={handleClose}>
                        <Alert onClose={handleClose} severity="error" sx={{ width: '100%' }}>
                            {renderError(addError)}
                        </Alert>
                    </Snackbar>
                    <Snackbar open={success} autoHideDuration={6000} onClose={handleClose2}>
                        <Alert onClose={handleClose2} severity="success" sx={{ width: '100%' }}>
                            Pomyślnie dodano szczepionkę
                        </Alert>
                    </Snackbar>
                </Box>
            </Container>
        </ThemeProvider>
    )
}