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
import ApartmentIcon from '@mui/icons-material/Apartment';
import Avatar from '@mui/material/Avatar';
import { confirm } from "react-confirm-box";
import FilteringHelepers from '../../tools/FilteringHelepers';
import DataDisplayArray from '../DataDisplayArray';
import { getVaccinationCentersData } from './AdminApi';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';
import LoginHelpers from '../../tools/LoginHelpers';

const theme = createTheme();

const Alert = React.forwardRef(function Alert(props, ref) {
    return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
});

const daysOfTheWeek = ["pon","wt","śr","czw","pt","sob","niedz"]

export default function VaccinationCentersPage() {

    const navigate = useNavigate();

    const columns = [
        {
            field: 'id',
            flex: 1.75
        },
        {
            field: 'name',
            headerName: 'Nazwa',
            minWidth: 110,
            flex: 1,
            renderCell: (params) => (
                <div className="multiline">
                    {params.value}
                </div>
            )
        },
        {
            field: 'city',
            headerName: 'Miasto',
            flex: 0.5,
        },
        {
            field: 'street',
            headerName: 'Ulica',
            flex: 0.5,
            renderCell: (params) => (
                <div className="multiline">
                    {params.value}
                </div>
            )
        },
        {
            field: 'vaccines',
            headerName: 'Szczepionki',
            minWidth: 125,
            flex: 1,
            cellClassName: 'list',
            renderCell: (params) => (
                <ul className="list">
                    {params.value.map((vaccine, index) => (
                        <li key={index}>{vaccine.name}: {vaccine.virus}</li>
                    ))}
                </ul>
            ),
        },
        {
            field: 'openingHoursDays',
            headerName: 'Otwarte do - do',
            flex: 0.75,
            renderCell: (params) => (
                <ul className="list">
                    {params.value.map((openingHoursDay, index) => (
                        <li key={index}>{daysOfTheWeek[index]}: {openingHoursDay.from}-{openingHoursDay.to}</li>
                    ))}
                </ul>
            ),
            
        },
        {
            field: 'active',
            headerName: 'Aktywny',
            type: 'boolean',
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
            let [data, err] = await getVaccinationCentersData();
            if (data != null) {
                console.log(data.find((e)=>e.vaccines))
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
        result = FilteringHelepers.filterId(result, data.get('idFilter'));
        result = FilteringHelepers.filterName(result, data.get('nameFilter'));
        result = FilteringHelepers.filterCity(result, data.get('cityFilter'));
        result = FilteringHelepers.filterActive(result, data.get('activeFilter'));
        //result = FilteringHelepers.filterVirus2(result, data.get('virusFilter'));
        result = FilteringHelepers.filterStreet(result, data.get('streetFilter'));
        console.log(result);
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
            if (error === '401' || error === '403') {
                LoginHelpers.logOut();
                navigate('/signin');
            }

        }
        setErrorState(false);
    };

    function handleRowClick(row) {
        /*
        navigate('/admin/vaccines/editVaccine', {
            state: {
                action: "edit", id: row.id, company: row.company, name: row.name, numberOfDoses: row.numberOfDoses,
                minDaysBetweenDoses: row.minDaysBetweenDoses, maxDaysBetweenDoses: row.maxDaysBetweenDoses, virusName: row.virus, minPatientAge: row.minPatientAge, maxPatientAge: row.maxPatientAge, active: row.active
            }
        })
        */
    }

    function renderError(param) {
        switch (param) {
            case '401':
                return 'Użytkownik nieuprawniony do pobrania danuch'
            case '403':
                return 'Użytkownikowi zabroniono pobierania danych'
            case '404':
                return 'Nie znaleziono danych'
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
                            <ApartmentIcon />
                        </Avatar>
                        <Typography component="h1" variant='h4'>
                            Centra Szczepień
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
                                        id="idFilter"
                                        label="id"
                                        name="idFilter"
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
                                        id="cityFilter"
                                        label="Miasto"
                                        name="cityFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="streetFilter"
                                        label="Ulica"
                                        name="streetFilter"
                                    />
                                </Grid>
                                {/*
                                <Grid item>
                                    <TextField
                                        id="vaccineFilter"
                                        label="Nazwa szczepionki"
                                        name="vaccineFilter"
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
                                        id="fromFilter"
                                        label="Otwarte od"
                                        name="fromFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="toFilter"
                                        label="Otwarte do"
                                        name="toFilter"
                                    />
                                </Grid>
                                */}
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
                            <Button variant='outlined' onClick={() => { /*navigate("/admin/vaccines/addVaccine", { state: { action: "add" } })*/}}>
                                Dodaj nowe centrum szczepień
                            </Button>
                        </Box>
                        <DataDisplayArray
                            loading={loading}
                            onRowClick={handleRowClick}
                            columns={columns}
                            filteredRows={filteredRows}
                            density="comfortable"
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