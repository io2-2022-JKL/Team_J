import * as React from 'react';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline, TextField } from '@mui/material';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import { useLocation, useNavigate } from "react-router-dom";
import Grid from "@material-ui/core/Grid";
import { GridActionsCellItem } from '@mui/x-data-grid';
import DeleteIcon from '@mui/icons-material/Delete';
import clsx from 'clsx';
import PeopleOutlineIcon from '@mui/icons-material/PeopleOutline';
import Avatar from '@mui/material/Avatar';
import DataDisplayArray from '../DataDisplayArray';
import { addDoctor, deletePatient, getPatientsData, getVaccinationCentersData } from './AdminApi';
import FilteringHelepers from '../../tools/FilteringHelepers';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import { activeOptionsEmptyPossible } from '../../tools/ActiveOptions';
import { ErrorSnackbar, SuccessSnackbar } from '../Snackbars';

const theme = createTheme();

export default function PatientsPage() {

    const navigate = useNavigate();
    const location = useLocation();

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
                    onClick={deactivatePatient(params.id)}
                />,
            ],
        },
    ];

    const [loading, setLoading] = React.useState(true);
    const [rows, setRows] = React.useState([]);
    const [filteredRows, setFilteredRows] = React.useState(rows);
    const [openCentersDialog, setOpenCentersDialog] = React.useState(false)
    const [vaccinationCenters, setVaccinationCenters] = React.useState()
    const [openSnackBar, setOpenErrorSnackBar] = React.useState(false)
    const [snackBarMessage, setSnackBarMessage] = React.useState('')
    const [option, setOption] = React.useState('');
    const [selectedRow, setSelectedRow] = React.useState();
    const [selectedCenterId, setSelectedCenterId] = React.useState();
    const [successState, setSuccessState] = React.useState(false)

    React.useEffect(() => {

        const fetchData = async () => {
            setLoading(true);
            let [data, err] = await getPatientsData();
            if (data != null) {
                setRows(data);
                setFilteredRows(data)
            }
            setLoading(false);
        }
        fetchData();
    }, []);


    const deactivatePatient = React.useCallback(
        (id) => async () => {
            let error = await deletePatient(id);
            console.log(error)
            if (error !== '200') {
                setSnackBarMessage(error)
                setOpenErrorSnackBar(true)
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

    async function handleRowClick(row) {
        if (location.state == true) {
            setSelectedRow(row)
            const [data, err] = await getVaccinationCentersData()
            if (err != '200') {
                setOpenErrorSnackBar(true)
                setSnackBarMessage('Nie udało się pobrac danych')
            }
            else {
                setVaccinationCenters(data)
                setOpenCentersDialog(true)
                setSelectedCenterId(data[0].id)
            }
        }
        else
            navigate('/admin/patients/editPatient', {
                state: {
                    id: row.id, pesel: row.PESEL, firstName: row.firstName, lastName: row.lastName, mail: row.mail,
                    dateOfBirth: row.dateOfBirth, phoneNumber: row.phoneNumber, active: row.active
                }
            })
    }

    const handleCenterChoice = async () => {
        const err = await addDoctor(selectedRow.id, selectedCenterId)
        if (err != '200') {
            setSnackBarMessage('Nie udało się dodać lekarza')
            setOpenErrorSnackBar(true)
        }
        else {
            setSnackBarMessage('Dodano lekarza lekarza')
            setSuccessState(true)
        }
        setOpenCentersDialog(false)
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

    const handleChange = (event) => {
        setOption(event.target.value);
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
                                        select
                                        label="Aktywny"
                                        name="activeFilter"
                                        value={option}
                                        onChange={handleChange}
                                        SelectProps={{
                                            native: true,
                                        }}
                                    >
                                        {activeOptionsEmptyPossible.map((option) => (
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
                            density="compact"
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
                                {openCentersDialog &&
                                    <TextField
                                        fullWidth
                                        id="centerSelection"
                                        select
                                        name="centerSelection"
                                        value={selectedCenterId}
                                        onChange={(e) => { setSelectedCenterId(e.target.value); }}
                                        SelectProps={{
                                            native: true,
                                        }}
                                    >
                                        {vaccinationCenters.map((center) => (
                                            <option key={center.id} value={center.id}>
                                                {center.name + ",  " + center.city}
                                            </option>
                                        ))}
                                    </TextField>}
                            </Box>
                        </DialogContent>
                        <DialogActions>
                            <Button onClick={() => setOpenCentersDialog(false)}>Wróc</Button>
                            <Button onClick={handleCenterChoice}>Wybierz</Button>
                        </DialogActions>
                    </Dialog>
                    <ErrorSnackbar
                        error={snackBarMessage}
                        errorState={openSnackBar}
                        setErrorState={setOpenErrorSnackBar}
                    />
                    <SuccessSnackbar
                        success={successState}
                        setSuccess={setSuccessState}
                    />
                </CssBaseline>
            </Container >
        </ThemeProvider >
    );
}

