import * as React from 'react';
import { useState } from 'react';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import TextField from '@mui/material/TextField';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Link, useLocation, useNavigate } from "react-router-dom";
import { addVaccine, editVaccine, getVaccinesData } from './AdminApi';
import ValidationHelpers from '../../tools/ValidationHelpers';
import LoginHelpers from '../../tools/LoginHelpers'
import { activeOptions } from '../../tools/ActiveOptions';
import WeekDays from '../../tools/WeekDays';
import CheckboxList from './DialogChooseVaccines';
import { Dialog } from '@material-ui/core';
import { DialogActions, DialogContent, DialogTitle } from '@mui/material';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import Checkbox from '@mui/material/Checkbox';
import { ErrorSnackbar, SuccessSnackbar } from '../../tools/Snackbars';

const theme = createTheme();

export default function AddOrEditVaccinationCenter() {
    const navigate = useNavigate();
    const location = useLocation();

    const [nameError, setNameError] = useState();
    const [nameErrorState, setNameErrorState] = useState(false);
    const [cityError, setCityError] = useState();
    const [cityErrorState, setCityErrorState] = useState(false);
    const [streetError, setStreetError] = useState();
    const [streetErrorState, setStreetErrorState] = useState(false);
    const [activeOption, setActiveOption] = React.useState(location.state != null ? location.state.active ? 'aktywny' : 'nieaktywny' : '');
    const [openingHoursError, setOpeningHoursError] = useState();
    const [openingHoursErrorState, setOpeningHoursErrorState] = useState();

    const [chooseVaccinesDialogOpen, setChooseVaccinesDialogOpen] = useState();

    const [operationError, setOperationError] = useState('');
    const [operationErrorState, setOperationErrorState] = useState(false);
    const [success, setSuccess] = useState(false);

    React.useEffect(() => {
    }, []);

    const handleSubmit = async (event) => {
        /*if (minPAErrorState || minPAErrorState2 || minDBDErrorState || minDBDErrorState2 || maxPAErrorState || maxPAErrorState2 || maxDBDErrorState || maxDBDErrorState2 || nODErrorState) {
            setOperationError("Błąd walidacji pól")
            setOperationErrorState(true)
            return
        }*/
        event.preventDefault();
        const data = new FormData(event.currentTarget);

        console.log({
            name: data.get('name'),
            city: data.get('city'),
            street: data.get('street'),
            vaccineIds: checkedVaccines.map(vaccine => vaccine.name),
            openingHoursDays: [data.get('0hours'), data.get('1hours'), data.get('2hours'),
            data.get('3hours'), data.get('4hours'), data.get('5hours'), data.get('6hours'),],
            active: data.get('active')
        })

        //return
        let error;
        console.log(location.state.action)

        if (location.state.action === "add")
            error = await addVaccine(data.get('company'), data.get('name'), Number.parseInt(data.get('numberOfDoses')),
                Number.parseInt(data.get('minDaysBetweenDoses')), Number.parseInt(data.get('maxDaysBetweenDoses')),
                data.get('virus'), Number.parseInt(data.get('minPatientAge')), Number.parseInt(data.get('maxPatientAge')),
                data.get('active'));
        else if (location.state.action === "edit")
            //editVaccine    
            error = await editVaccine(location.state.id, data.get('company'), data.get('name'), Number.parseInt(data.get('numberOfDoses')),
                Number.parseInt(data.get('minDaysBetweenDoses')), Number.parseInt(data.get('maxDaysBetweenDoses')),
                data.get('virus'), Number.parseInt(data.get('minPatientAge')), Number.parseInt(data.get('maxPatientAge')),
                data.get('active'));
        setOperationError(error);
        if (error != '200')
            setOperationErrorState(true);
        else
            setSuccess(true);
    };

    function openingHoursTextField(dayNum) {
        return (
            <Grid item xs={12}>
                <TextField
                    defaultValue={location.state != null ?
                        location.state.openingHours[dayNum].from + " - " + location.state.openingHours[dayNum].to : "08:00 - 20:00"}
                    required
                    fullWidth
                    name={dayNum + "hours"}
                    label={"Godziny otwarcia - " + WeekDays[dayNum]}
                    id={dayNum + "hours"}
                    onChange={(e) => {
                        ValidationHelpers.validateOpeningHours(e, setOpeningHoursError, setOpeningHoursErrorState)
                    }}
                    helperText={openingHoursError}
                    error={openingHoursErrorState}
                />
            </Grid>
        )
    }

    const handleClose = (event, reason) => {
        if (reason === 'clickaway') {
            return;
        }
        if (operationErrorState) {
            if (operationError === '401' || operationError === '403') {
                LoginHelpers.logOut();
                navigate('/signin');
            }
        }
        setOperationErrorState(false);
    };

    const handleClose2 = (event, reason) => {
        if (reason === 'clickaway') {
            return;
        }
        setSuccess(false);
    };

    const [vaccines, setVaccines] = useState()

    async function renderVaccinesDialog() {
        const fetchData = async () => {
            let [data, err] = await getVaccinesData();
            if (data != null) {
                setVaccines(data);
            }
            else {
                //setError(err);
                //setErrorState(true);
            }
            console.log(err)
        }
        await fetchData();

        let newChecked = [];
        if (location.state != null) {
            for (let i = 0; i < location.state.vaccines.length; i++)
                newChecked.push(location.state.vaccines[i]);
        }

        setCheckedVaccines(newChecked);
        console.log("checkedVaccines")
        console.log(checkedVaccines)

        setChooseVaccinesDialogOpen(true)
    }

    const [checkedVaccines, setCheckedVaccines] = React.useState([0]);

    const handleToggle = (value) => () => {
        const currentIndex = checkedVaccines.indexOf(value);
        const newChecked = [...checkedVaccines];

        if (currentIndex === -1) {
            newChecked.push(value);
        } else {
            newChecked.splice(currentIndex, 1);
        }

        setCheckedVaccines(newChecked);
    };

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
                        Wpisz dane centrum szczepień
                    </Typography>
                    <Box component="form" noValidate onSubmit={handleSubmit} sx={{ mt: 3 }}>
                        <Grid container spacing={2}>
                            <Grid item xs={12}>
                                <TextField
                                    defaultValue={location.state != null ? location.state.name : null}
                                    required
                                    fullWidth
                                    name="name"
                                    label="Nazwa"
                                    id="name"
                                    onChange={(e) => {
                                        ValidationHelpers.validateName(e, setNameError, setNameErrorState)
                                    }}
                                    helperText={nameError}
                                    error={nameErrorState}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    defaultValue={location.state != null ? location.state.city : null}
                                    required
                                    fullWidth
                                    name="city"
                                    label="Miasto"
                                    id="city"
                                    onChange={(e) => {
                                        ValidationHelpers.validateName(e, setCityError, setCityErrorState)
                                    }}
                                    helperText={cityError}
                                    error={cityErrorState}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    defaultValue={location.state != null ? location.state.street : null}
                                    required
                                    fullWidth
                                    name="street"
                                    label="Ulica"
                                    id="street"
                                    onChange={(e) => {
                                        ValidationHelpers.validateName(e, setStreetError, setStreetErrorState)
                                    }}
                                    helperText={streetError}
                                    error={streetErrorState}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    fullWidth
                                    id="active"
                                    select
                                    label="Aktywne"
                                    name="active"
                                    value={activeOption}
                                    onChange={(event) => setActiveOption(event.target.value)}
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
                            {openingHoursTextField(0)}
                            {openingHoursTextField(1)}
                            {openingHoursTextField(2)}
                            {openingHoursTextField(3)}
                            {openingHoursTextField(4)}
                            {openingHoursTextField(5)}
                            {openingHoursTextField(6)}
                        </Grid>
                        <Button
                            fullWidth
                            variant="outlined"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={() => renderVaccinesDialog()}
                        >
                            Wybierz szczepionki
                        </Button>
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
                        onClick={() => { navigate("/admin/vaccinationCenters") }}
                    >
                        Powrót
                    </Button>
                    <ErrorSnackbar
                        error={operationError}
                        errorState={operationErrorState}
                        setErrorState={setOperationErrorState}
                    />
                    <SuccessSnackbar
                        success={success}
                        setSuccess={setSuccess}
                    />
                </Box>

                <Dialog
                    fullWidth
                    open={chooseVaccinesDialogOpen}
                >
                    <DialogTitle>Wybierz szczepionki</DialogTitle>
                    <DialogContent>
                        <List sx={{ width: '100%', maxWidth: 360, bgcolor: 'background.paper' }}>
                            {vaccines && vaccines.map((vaccine) => {
                                const labelId = `checkbox-list-label-${vaccine}`;
                                return (
                                    <ListItem
                                        key={vaccine.id}
                                        disablePadding
                                    >
                                        <ListItemButton role={undefined} onClick={handleToggle(vaccine)} dense>
                                            <ListItemIcon>
                                                <Checkbox
                                                    edge="start"
                                                    checked={checkedVaccines.indexOf(vaccine) !== -1}
                                                    tabIndex={-1}
                                                    disableRipple
                                                    inputProps={{ 'aria-labelledby': labelId }}
                                                />
                                            </ListItemIcon>
                                            <ListItemText id={labelId} primary={vaccine.name + ", producent: " + vaccine.company} />
                                        </ListItemButton>
                                    </ListItem>
                                );
                            })}
                        </List>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setChooseVaccinesDialogOpen(false)}>Wróć</Button>
                        <Button onClick={() => setChooseVaccinesDialogOpen(false)}>Wybierz</Button>
                    </DialogActions>
                </Dialog>

            </Container>
        </ThemeProvider>

    )
}