import * as React from 'react';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline, TextField } from '@mui/material';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import { useNavigate } from "react-router-dom";
import Grid from "@material-ui/core/Grid";
import { GridActionsCellItem } from '@mui/x-data-grid';
import DeleteIcon from '@mui/icons-material/Delete';
import clsx from 'clsx';
import PeopleOutlineIcon from '@mui/icons-material/PeopleOutline';
import Avatar from '@mui/material/Avatar';
import DataDisplayArray from '../DataDisplayArray';
import { deleteDoctor, getDoctorsData } from './AdminApi';
import FilteringHelepers from '../../tools/FilteringHelepers';
import { activeOptionsEmptyPossible } from '../../tools/ActiveOptions';
import { ErrorSnackbar } from '../Snackbars';
import DropDownSelect from '../DropDownSelect';
import { citiesEmptyPossible } from '../../api/Cities';

const theme = createTheme();

export default function DoctorsPage() {

    const navigate = useNavigate();

    const columns = [
        {
            field: 'id',
            flex: 2
        },
        {
            field: 'PESEL',
            headerName: 'PESEL',
            minWidth: 110,
            flex: 0.5,
            editable: true
        },
        {
            field: 'firstName',
            headerName: 'Imię',
            flex: 0.5,
            editable: true
        },
        {
            field: 'lastName',
            headerName: 'Nazwisko',
            flex: 0.5,
            editable: true
        },
        {
            field: 'mail',
            headerName: 'E-Mail',
            minWidth: 125,
            flex: 0.5,
            editable: true
        },
        {
            field: 'dateOfBirth',
            headerName: 'Data urodzenia',
            flex: 0.75,
            editable: true
        },
        {
            field: 'phoneNumber',
            headerName: 'Numer telefonu',
            minWidth: 125,
            flex: 0.5,
            editable: true
        },
        {
            field: 'vaccinationCenterId',
            headerName: 'ID Centrum Sczepień',
            flex: 0.75,
            editable: true
        },
        {
            field: 'name',
            headerName: 'Nazwa Centrum Szczepień',
            flex: 0.75,
            editable: true
        },
        {
            field: 'city',
            headerName: 'Miasto Centrum Sczepień',
            flex: 0.75,
            editable: true
        },
        {
            field: 'street',
            headerName: 'Ulica Centrum Sczepień',
            flex: 0.75,
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
                    onClick={deactivateDoctor(params.id)}
                />,
            ],
        },
    ];

    const [loading, setLoading] = React.useState(true);
    const [rows, setRows] = React.useState([]);
    const [error, setError] = React.useState('');
    const [errorState, setErrorState] = React.useState(false);

    React.useEffect(() => {

        const fetchData = async () => {
            setLoading(true);
            let [data, err] = await getDoctorsData();
            if (data != null) {
                setRows(data);
                setFilteredRows(data)
            }
            else {
                setError(err);
                setErrorState(true);
            }
            setLoading(false);
        }
        fetchData();
    }, []);

    const [filteredRows, setFilteredRows] = React.useState(rows);

    const deactivateDoctor = React.useCallback(
        (id) => async () => {
            let error = await deleteDoctor(id);
            console.log(error)
            if (error !== '200') {
                setError(error)
                setErrorState(true)
            }
            else {
                setTimeout(() => {
                    setRows((prevRows) => prevRows.map((row) => row.id === id ? { ...row, active: false } : row));
                    setFilteredRows((prevRows) => prevRows.map((row) => row.id === id ? { ...row, active: false } : row));
                });
            }
        },
        [],
    );

    function handleRowClick(row) {
        navigate('/admin/doctors/editDoctor', {
            state: {
                id: row.id, pesel: row.PESEL, firstName: row.firstName, lastName: row.lastName, mail: row.mail,
                dateOfBirth: row.dateOfBirth, phoneNumber: row.phoneNumber, active: row.active, vaccinationCenterId: row.vaccinationCenterId
            },
        })
    }

    const handleSubmit = (event) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        let result = rows;
        result = FilteringHelepers.filterId(result, data.get('idFilter'));
        result = FilteringHelepers.filterPESEL(result, data.get('peselFilter'));
        result = FilteringHelepers.filterFirstName(result, data.get('firstNameFilter'));
        result = FilteringHelepers.filterLastName(result, data.get('lastNameFilter'));
        result = FilteringHelepers.filterEmail(result, data.get('emailFilter'));
        result = FilteringHelepers.filterDate(result, data.get('dateOfBirthFilter'));
        result = FilteringHelepers.filterPhoneNumber(result, data.get('phoneNumberFilter'));
        result = FilteringHelepers.filterActive(result, data.get('activeFilter'));
        result = FilteringHelepers.filterVaccinationCenterId(result, data.get('vaccinationCenterIdFilter'));
        result = FilteringHelepers.filterName(result, data.get('nameFilter'));
        result = FilteringHelepers.filterCity(result, data.get('cityFilter'));
        result = FilteringHelepers.filterStreet(result, data.get('streetFilter'));
        setFilteredRows(result);
    };


    const [option, setOption] = React.useState('');
    const [selectedCity, setSelectedCity] = React.useState('')

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
                            <PeopleOutlineIcon />
                        </Avatar>
                        <Typography component="h1" variant='h4'>
                            Lekarze
                        </Typography>
                        <Box
                            component='form'
                            noValidate
                            onChange={handleSubmit}
                            sx={{
                                marginTop: 2,
                                marginBottom: 2,
                                display: 'flex',
                            }}
                        >
                            <Grid container direction={"row"} spacing={1} >
                                <Grid item xs={3}>
                                    <TextField
                                        fullWidth
                                        id="idFilter"
                                        label="ID"
                                        name="idFilter"
                                    />
                                </Grid>
                                <Grid item xs={3}>
                                    <TextField
                                        fullWidth
                                        id="peselFilter"
                                        label="PESEL"
                                        name="peselFilter"
                                    />
                                </Grid>
                                <Grid item xs={3}>
                                    <TextField
                                        fullWidth
                                        id="firstNameFilter"
                                        label="Imię"
                                        name="firstNameFilter"
                                    />
                                </Grid>
                                <Grid item xs={3}>
                                    <TextField
                                        fullWidth
                                        id="lastNameFilter"
                                        label="Nazwisko"
                                        name="lastNameFilter"
                                    />
                                </Grid>
                                <Grid item xs={3}>
                                    <TextField
                                        fullWidth
                                        id="emailFilter"
                                        label="Email"
                                        name="emailFilter"
                                    />
                                </Grid>
                                <Grid item xs={3}>
                                    <TextField
                                        fullWidth
                                        id="dateOfBirthFilter"
                                        label="Data urodzenia"
                                        name="dateOfBirthFilter"
                                    />
                                </Grid>
                                <Grid item xs={3}>
                                    <TextField
                                        fullWidth
                                        id="phoneNumberFilter"
                                        label="Numer telefonu"
                                        name="phoneNumberFilter"
                                    />
                                </Grid>
                                <Grid item xs={3}>
                                    {DropDownSelect("activeFilter", "Aktywny", activeOptionsEmptyPossible, option, setOption)}
                                </Grid>
                                <Grid item xs={3}>
                                    <TextField
                                        fullWidth
                                        id="vaccinationCenterIdFilter"
                                        label="ID Centrum Szczepień"
                                        name="vaccinationCenterIdFilter"
                                    />
                                </Grid>
                                <Grid item xs={3}>
                                    <TextField
                                        fullWidth
                                        id="nameFilter"
                                        label="Nazwa Centrum Szczepień"
                                        name="nameFilter"
                                    />
                                </Grid>
                                <Grid item xs={3}>
                                    {DropDownSelect("cityFilter", "Miasto Centrum Szczepień", citiesEmptyPossible, selectedCity, setSelectedCity)}
                                </Grid>
                                <Grid item xs={3}>
                                    <TextField
                                        fullWidth
                                        id="street"
                                        label="Ulica Centrum Szczepień"
                                        name="streetFilter"
                                    />
                                </Grid>
                            </Grid>
                        </Box>
                        <DataDisplayArray
                            loading={loading}
                            onRowClick={handleRowClick}
                            columns={columns}
                            filteredRows={filteredRows}
                        />
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={async () => { navigate("/admin/patients", { state: true }) }}
                            name="addDoctorButton"
                        >
                            Dodaj lekarza
                        </Button>
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={async () => { navigate("/admin") }}
                        >
                            Powrót
                        </Button>
                        <ErrorSnackbar
                            error={error}
                            errorState={errorState}
                            setErrorState={setErrorState}
                        />
                    </Box>
                </CssBaseline>
            </Container >
        </ThemeProvider >
    );
}

