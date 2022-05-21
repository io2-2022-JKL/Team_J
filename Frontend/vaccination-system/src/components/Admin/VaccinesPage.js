import * as React from 'react';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline, TextField } from '@mui/material';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import { useNavigate } from "react-router-dom";
import Grid from "@material-ui/core/Grid";
import { DataGrid, GridActionsCellItem } from '@mui/x-data-grid';
import DeleteIcon from '@mui/icons-material/Delete';
import clsx from 'clsx';
import VaccinesIcon from '@mui/icons-material/Vaccines';
import Avatar from '@mui/material/Avatar';
import { confirm } from "react-confirm-box";
import FilteringHelepers from '../../tools/FilteringHelepers';
import DataDisplayArray from '../DataDisplayArray';
import { getVaccinesData } from './AdminApi';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';
import LoginHelpers from '../../tools/LoginHelpers';

const theme = createTheme();

const Alert = React.forwardRef(function Alert(props, ref) {
    return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
});

export default function DoctorsPage() {

    const navigate = useNavigate();

    const columns = [
        {
            field: 'id',
            flex: 2
        },
        {
            field: 'company',
            headerName: 'Firma',
            minWidth: 110,
            flex: 0.5,
            editable: true
        },
        {
            field: 'name',
            headerName: 'Nazwa',
            flex: 1,
            editable: true
        },
        {
            field: 'numberOfDoses',
            headerName: 'Liczba dawek',
            flex: 0.5,
            editable: true
        },
        {
            field: 'minDaysBetweenDoses',
            headerName: 'Minimalna liczba dni pomiędzy dawkami',
            minWidth: 125,
            flex: 0.5,
            editable: true
        },
        {
            field: 'maxDaysBetweenDoses',
            headerName: 'Maksymalna liczba dni pomiędzy dawkami',
            minWidth: 125,
            flex: 0.5,
            editable: true
        },
        {
            field: 'virus',
            headerName: 'Nazwa wirusa',
            flex: 0.75,
            editable: true
        },
        {
            field: 'minPatientAge',
            headerName: 'Minimalny wiek pacjenta',
            minWidth: 125,
            flex: 0.5,
            editable: true
        },
        {
            field: 'maxPatientAge',
            headerName: 'Maksymalny wiek pacjenta',
            minWidth: 125,
            flex: 0.5,
            editable: true
        },
        {
            field: 'active',
            headerName: 'Aktywny',
            type: 'boolean',
            editable: true,
            flex: 0.5,
            cellClassName: (params) => {
                if (params.value == null) {
                    return '';
                }
                return clsx('active', {
                    negative: params.value === false,
                    positive: params.value === true,
                });
            },
        },
        {
            field: 'actions',
            type: 'actions',
            width: 80,
            getActions: (params) => [
                <GridActionsCellItem
                    icon={<DeleteIcon color='error' />}
                    label="Delete"
                    onClick={deleteUser(params.id)}
                />,
            ],
        },
    ];

    const [rows, setRows] = React.useState([]);
    const [filteredRows, setFilteredRows] = React.useState(rows);
    const [loading, setLoading] = React.useState(false);
    const [error, setError] = React.useState('');
    const [errorState, setErrorState] = React.useState(false);

    React.useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            let [data, err] = await getVaccinesData();
            if (data != null) {
                setRows(data);
                setFilteredRows(data)
            }
            else {
                setError(err);
                setErrorState(true);  
            }
            console.log(err)
            setLoading(false);
        }
        fetchData();
    }, []);

    const deleteUser = React.useCallback(
        (id) => () => {
            setTimeout(() => {
                setRows((prevRows) => prevRows.filter((row) => row.id !== id));
                setFilteredRows((prevRows) => prevRows.filter((row) => row.id !== id));
            });
        },
        [],
    );

    const handleSubmit = (event) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        let result = rows;
        result = FilteringHelepers.filterId(result, data.get('vaccineIdFilter'));
        result = FilteringHelepers.filterCompany(result, data.get('companyFilter'));
        result = FilteringHelepers.filterName(result, data.get('nameFilter'));
        result = FilteringHelepers.filterNumberOfDoses(result, data.get('numberOfDosesFilter'));
        result = FilteringHelepers.filterActive(result, data.get('activeFilter'));
        result = FilteringHelepers.filterMinDaysBetweenDoses(result, data.get('minDaysBetweenDosesFilter'));
        result = FilteringHelepers.filterMaxDaysBetweenDoses(result, data.get('maxDaysBetweenDosesFilter'));
        result = FilteringHelepers.filterVirus(result, data.get('virusFilter'));
        result = FilteringHelepers.filterMinPatientAge(result, data.get('minPatientAgeFilter'));
        result = FilteringHelepers.filterMaxPatientAge(result, data.get('maxPatientAgeFilter'));
        setFilteredRows(result);
    };
    const [currency, setCurrency] = React.useState('');

    const handleChange = (event) => {
        setCurrency(event.target.value);
    };

    const currencies = [
        {
            value: 'aktywny',
            label: 'aktywny',
        },
        {
            value: 'nieaktywny',
            label: 'nieaktywny',
        },
        {
            value: '',
            label: '',
        }
    ];
    const handleClose = (event, reason) => {
        if (reason === 'clickaway') {
            return;
        }
        if (errorState) {
            if (error === '401' || error === '403')
            {
                LoginHelpers.logOut();
                navigate('/signin');
            }
                
        }
        setErrorState(false);
    };

    function handleRowClick(row) {
        navigate('/admin/vaccines/editVaccine', {
            state: {
                action: "edit", id: row.id, company: row.company, name: row.name, numberOfDoses: row.numberOfDoses,
                minDaysBetweenDoses: row.minDaysBetweenDoses, maxDaysBetweenDoses: row.maxDaysBetweenDoses, virusName: row.virus, minPatientAge: row.minPatientAge, maxPatientAge: row.maxPatientAge, active: row.active
            }
        })
    }

    function renderError(param) {
        switch (param) {
            case '401':
                return 'Użytkownik nieuprawniony do pobrania szczepionek'
            case '403':
                return 'Użytkownikowi zabroniono pobierania szczepionek'
            case '404':
                return 'Nie znaleziono szczepionek'
            case 'ECONNABORTED':
                return 'Przekroczono limit połączenia'
            default:
                return 'Wystąpił błąd!';
        }
    }

    return (
        <ThemeProvider theme={theme}>
            <Container component="main" maxWidth='lg'>
                <CssBaseline>
                    <Box
                        sx={{
                            marginTop: 2,
                            display: 'flex',
                            flexDirection: 'column',
                            alignItems: 'center',
                        }}
                    >
                        <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
                            <VaccinesIcon />
                        </Avatar>
                        <Typography component="h1" variant='h4'>
                            Szczepionki
                        </Typography>
                        <Box
                            component='form'
                            noValidate
                            onChange={handleSubmit}
                            //onSubmit={handleSubmit}
                            sx={{
                                marginTop: 2,
                                marginBottom: 2,
                                display: 'flex',
                            }}
                        >
                            <Grid container direction={"row"} spacing={1}>
                                <Grid item>
                                    <TextField
                                        id="vaccineIdFilter"
                                        label="vaccineID"
                                        name="vaccineIdFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="companyFilter"
                                        label="Firma"
                                        name="companyFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="nameFilter"
                                        label="Nazwa"
                                        name="nameFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="numberOfDosesFilter"
                                        label="Liczba dawek"
                                        name="numberOfDosesFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="minDaysBetweenDosesFilter"
                                        label="Minimalna liczba dni pomiędzy dawkami"
                                        name="minDaysBetweenDosesFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="maxDaysBetweenDosesFilter"
                                        label="Maksymalna liczba dni pomiędzy dawkami"
                                        name="maxDaysBetweenDosesFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="virusFilter"
                                        label="Nazwa wirusa"
                                        name="virusFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="minPatinetAgeFilter"
                                        label="Minimalny wiek pacjenta"
                                        name="minPatientAgeFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="maxPatientAgeFilter"
                                        label="Maksymalny wiek pacjenta"
                                        name="maxPatientAgeFilter"
                                    />
                                </Grid>
                                <Grid item >
                                    <TextField
                                        fullWidth
                                        id="activeFilter"
                                        select
                                        label="Aktywny"
                                        name="activeFilter"
                                        value={currency}
                                        onChange={handleChange}
                                        SelectProps={{
                                            native: true,
                                        }}
                                    >
                                        {currencies.map((option) => (
                                            <option key={option.value} value={option.value}>
                                                {option.label}
                                            </option>
                                        ))}
                                    </TextField>
                                </Grid>
                            </Grid>
                            <Button variant='outlined' onClick={() => { navigate("/admin/vaccines/addVaccine", { state: { action: "add" }}) }}>
                                Dodaj nową szczepionkę
                            </Button>
                        </Box>
                        <DataDisplayArray
                            loading={loading}
                            onRowClick={handleRowClick}
                            columns={columns}
                            filteredRows={filteredRows}
                            density="compact"
                        />
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={() => { navigate("/admin") }}
                        >
                            Powrót
                        </Button>
                        <Snackbar open={errorState} autoHideDuration={2000} onClose={handleClose}>
                            <Alert onClose={handleClose} severity="error" sx={{ width: '100%' }}>
                                {renderError(error)}
                            </Alert>
                        </Snackbar>
                    </Box>
                </CssBaseline>
            </Container >
        </ThemeProvider >
    );
}

const confirmOptionsInPolish = {
    labels: {
        confirmable: "Tak",
        cancellable: "Nie"
    }
}