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
import { getVaccinationCentersData, deleteVaccinationCenter } from './AdminApi';
import { activeOptionsEmptyPossible } from '../../tools/ActiveOptions';
import { ErrorSnackbar } from '../Snackbars';
import DropDownSelect from '../DropDownSelect';
import { citiesEmptyPossible } from '../../api/Cities';

const theme = createTheme();

const daysOfTheWeek = ["pon", "wt", "śr", "czw", "pt", "sob", "niedz"]


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
                    onClick={deactivateVaccinationCenter(params.id)}
                />,
            ],
        },
    ];

    const [rows, setRows] = React.useState([]);
    const [filteredRows, setFilteredRows] = React.useState(rows);
    const [loading, setLoading] = React.useState(false);
    const [error, setError] = React.useState('');
    const [errorState, setErrorState] = React.useState(false);
    const [selectedCity, setSelectedCity] = React.useState('')

    React.useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            let [data, err] = await getVaccinationCentersData();
            if (data != null) {
                console.log(data.find((e) => e.vaccines))
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

    const deactivateVaccinationCenter = React.useCallback(
        (id) => async () => {
            let error = await deleteVaccinationCenter(id);
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

    const handleSubmit = (event) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        let result = rows;
        result = FilteringHelepers.filterId(result, data.get('idFilter'));
        result = FilteringHelepers.filterName(result, data.get('nameFilter'));
        result = FilteringHelepers.filterCity(result, data.get('cityFilter'));
        result = FilteringHelepers.filterActive(result, data.get('activeFilter'));
        result = FilteringHelepers.filterStreet(result, data.get('streetFilter'));
        console.log(result);
        setFilteredRows(result);
    };
    const [activeOption, setActiveOption] = React.useState('');

    function handleRowClick(row) {
        navigate('/admin/vaccinationCenters/editVaccinationCenter', {
            state: {
                action: "edit", id: row.id, name: row.name, city: row.city, street: row.street,
                active: row.active, openingHours: row.openingHoursDays, vaccines: row.vaccines
            }
        })
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

                                    {DropDownSelect("cityFilter", "Miasto", citiesEmptyPossible, selectedCity, setSelectedCity)}
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="streetFilter"
                                        label="Ulica"
                                        name="streetFilter"
                                    />
                                </Grid>
                                <Grid item >
                                    {DropDownSelect("activeFilter", "Aktywny", activeOptionsEmptyPossible, activeOption, setActiveOption)}
                                </Grid>
                            </Grid>
                            <Button variant='outlined' onClick={() => { navigate("/admin/vaccinationCenters/addVaccinationCenter", { state: { action: "add" } }) }}>
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