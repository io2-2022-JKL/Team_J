import * as React from 'react';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline, Snackbar, TextField } from '@mui/material';
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
import { addDoctor, getPatientsData, getRandomPatientData, getVaccinationCenters } from './AdminApi';
import FilteringHelepers from '../../tools/FilteringHelepers';
import Autocomplete from '@mui/material/Autocomplete';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import Select from '@mui/material/Select';

const theme = createTheme();

export default function PatientsPage(addingDoctor = false) {

    const navigate = useNavigate();

    const columns = [
        {
            field: 'id',
            flex: 2
        },
        {
            field: 'pesel',
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

    //const [rows, setRows] = React.useState();

    const [rows, setRows] = React.useState([]);


    React.useEffect(() => {

        const fetchData = async () => {
            setLoading(true);
            let [data, err] = await getPatientsData();
            if (data != null) {
                setRows(data);
                setFilteredRows(data)
            }
            else {
                //setError(err);
                //setErrorState(true);
            }
            setLoading(false);
        }
        fetchData();
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

    const [openCentersDialog, setOpenCentersDialog] = React.useState(false)
    const [vaccinationCenters, setVaccinationCenters] = React.useState()
    const [openSnackBar, setOpenSnackBar] = React.useState(false)
    const [snackBarMessage, setSnackBarMessage] = React.useState('')

    const [selectedRow, setSelectedRow] = React.useState();
    const [selectedCenter, setSelectedCenter] = React.useState();

    function handleRowClick(row) {
        if (addingDoctor) {
            setSelectedRow(row)
            const [response, err] = getVaccinationCenters()
            if (err != '200') {
                setOpenSnackBar(true)
                setSnackBarMessage('Nie udało się pobrac danych')
            }
            else {
                setOpenCentersDialog(true)
                setVaccinationCenters(response.data)
            }
        }
        else
            navigate('/admin/patients/editPatient', {
                state: {
                    id: row.id, pesel: row.pesel, firstName: row.firstName, lastName: row.lastName, mail: row.mail,
                    dateOfBirth: row.dateOfBirth, phoneNumber: row.phoneNumber, active: row.active
                }
            })
    }

    const handleCenterChange = (e) => {
        setSelectedCenter(e.target.value)
    }

    const handleDialogClose = () => {
        setOpenCentersDialog(false)
    }

    const handleCenterChoice = async () => {
        const err = addDoctor(selectedRow.id, selectedCenter)
        if (err != '200') {
            setOpenSnackBar(true)
            setSnackBarMessage('Nie udało się dodać lekarza')
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
                            onClick={async () => { navigate("/admin") }}
                        >
                            Powrót
                        </Button>
                    </Box>

                    <Dialog
                        fullWidth
                        open={openCentersDialog}
                    >
                        <DialogTitle>Wybierz Centrum Szczepień</DialogTitle>
                        <DialogContent>
                            <Box
                                noValidate
                                component="form"
                                sx={{
                                    display: 'flex',
                                    flexDirection: 'column',
                                    m: 'auto',
                                    width: 'fit-content',
                                }}
                            >
                                {openCentersDialog && <FormControl sx={{ mt: 2, minWidth: 120 }}>
                                    <InputLabel htmlFor="max-width">Wybierz</InputLabel>
                                    <Select
                                        value={vaccinationCenters[0]}
                                        autoFocus
                                        onChange={handleCenterChange}
                                        label="maxWidth"
                                    >
                                        {vaccinationCenters && vaccinationCenters.map(center =>
                                            <MenuItem value={center}>{center.name + ",  " + center.city}</MenuItem>)}
                                    </Select>
                                </FormControl>}
                            </Box>
                        </DialogContent>
                        <DialogActions>
                            <Button onClick={handleDialogClose}>Wróc</Button>
                            <Button onClick={handleCenterChoice}>Wybierz</Button>
                        </DialogActions>
                    </Dialog>
                    <Snackbar
                        open={openSnackBar}
                        autoHideDuration={6000}
                        onClose={() => { setOpenSnackBar(false) }}
                        message={snackBarMessage}
                    />
                </CssBaseline>
            </Container >
        </ThemeProvider >
    );
}

