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
import { confirm } from "react-confirm-box";
import DataDisplayArray from '../DataDisplayArray';
import { getPatientsData, getRandomPatientData } from './AdminApi';
import { FilteringHelepers } from '../FilteringHelepers';

const theme = createTheme();

export default function PatientsPage() {

    const navigate = useNavigate();

    const columns = [
        {
            field: 'id',
            flex: 2
        },
        {
            field: 'PESEL',
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
            field: 'email',
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

    const [loading, setLoading] = React.useState(true);

    const [rows, setRows] = React.useState();

    //const [rows, setRows] = React.useState([]);

    React.useEffect(() => {
        let patientData;
        const fetchData = async () => {
            setLoading(true);

            patientData = await getPatientsData();
            console.log(patientData)
            //if (patientData.length === 0)
            patientData = getRandomPatientData();

            console.log(getRandomPatientData())

            //setTimeout(() => {
            setRows(patientData);
            setFilteredRows(rows);
            //});

            console.log(filteredRows)

            setLoading(false);

        }

        fetchData();

        setRows(getRandomPatientData())
        console.log("run useEffect")


    }, []);

    const [filteredRows, setFilteredRows] = React.useState(rows);

    const deleteUser = React.useCallback(
        (id) => () => {
            setTimeout(() => {
                setRows((prevRows) => prevRows.filter((row) => row.id !== id));
                setFilteredRows((prevRows) => prevRows.filter((row) => row.id !== id));
            });
        },
        [],
    );

    const editCell = async (params, event) => {
        const result = await confirm("Czy na pewno chcesz edytować pacjenta?", confirmOptionsInPolish);
        if (result) {
            console.log("You click yes!");
            rows[params.id] = params.value;
            filteredRows[params.id] = params.value;
            setRows(rows);
            setFilteredRows(filteredRows);
            return;
        }
        else {
            rows[params.id] = params.value;
            filteredRows[params.id] = params.value;
            setFilteredRows(filteredRows);
        }
        setFilteredRows(filteredRows);
        console.log("You click No!");

        if (!event.ctrlKey) {
            event.defaultMuiPrevented = true;
        }
        console.log(params.row.PESEL);
    }

    const confirmOptionsInPolish = {
        labels: {
            confirmable: "Tak",
            cancellable: "Nie"
        }
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
        setFilteredRows(result);
    };

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
                            Pacjenci
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
                                    <TextField
                                        fullWidth
                                        id="activeFilter"
                                        label="Aktywny"
                                        name="activeFilter"
                                    />
                                </Grid>
                            </Grid>
                        </Box>
                        <DataDisplayArray
                            loading={loading}
                            editCell={editCell}
                            columns={columns}
                            filteredRows={filteredRows}
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
                    </Box>
                </CssBaseline>
            </Container >
        </ThemeProvider >
    );
}

