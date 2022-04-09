import * as React from 'react';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline, TextField } from '@mui/material';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import { useNavigate } from "react-router-dom";
import Grid from "@material-ui/core/Grid";
import { DataGrid, GridActionsCellItem } from '@mui/x-data-grid';
import { randomDate, randomEmail, randomId, randomPhoneNumber, randomTraderName, randomBoolean, randomInt } from '@mui/x-data-grid-generator';
import dateFormat from 'dateformat';
import DeleteIcon from '@mui/icons-material/Delete';
import clsx from 'clsx';
import PeopleOutlineIcon from '@mui/icons-material/PeopleOutline';
import Avatar from '@mui/material/Avatar';

const theme = createTheme();

const filterFirstName = (array, filter) => {
    return array.filter((item) => item.firstName.includes(filter));
};
const filterLastName = (array, filter) => {
    return array.filter((item) => item.lastName.includes(filter));
};
const filterEmail = (array, filter) => {
    return array.filter((item) => item.email.includes(filter));
};
const filterPhoneNumber = (array, filter) => {
    return array.filter((item) => item.phoneNumber.includes(filter));
};
const filterPESEL = (array, filter) => {
    return array.filter((item) => item.PESEL.includes(filter));
};

const filterId = (array, filter) => {
    return array.filter((item) => item.id.includes(filter));
};

const filterDate = (array, filter) => {
    return array.filter((item) => {
        if (filter.length !== 10)
            return true;
        else
            return item.dateOfBirth === filter;
    });
}

const filterActive = (array, filter) => {
    return array.filter((item) => {
        if (filter === "aktywny")
            return item.active === true;
        else if (filter === "nieaktywny")
            return item.active === false;
        else
            return true;
    });
}

let id = randomId();
const createRandomRow = () => {
    id = randomId();
    return {
        id: id,
        PESEL: randomInt(10000000000, 100000000000).toString(),
        firstName: randomTraderName().split(' ')[0],
        lastName: randomTraderName().split(' ')[1],
        email: randomEmail(),
        dateOfBirth: dateFormat(randomDate(new Date(50, 1), new Date("1/1/30")), "isoDate").toString(),
        phoneNumber: randomPhoneNumber(),
        active: randomBoolean()
    }
};

export default function PatientsPage() {

    const navigate = useNavigate();

    const [pageSize, setPageSize] = React.useState(10);

    const columns = [
        {
            field: 'PESEL',
            minWidth: 110,
            flex: 0.5,
            editable: true
        },
        {
            field: 'firstName',
            headerName: 'Imię',
            editable: true
        },
        {
            field: 'lastName',
            headerName: 'Nazwisko',
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
            flex: 0.5,
            editable: true
        },
        {
            field: 'phoneNumber',
            headerName: 'Numer telefonu',
            width: 125,
            editable: true
        },
        {
            field: 'active',
            headerName: 'Aktywny',
            type: 'boolean',
            editable: true,
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

    const [rows, setRows] = React.useState(() => [
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
        createRandomRow(),
    ]);

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

    const handleSubmit = (event) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        let result = rows;
        result = filterId(result, data.get('idFilter'));
        result = filterPESEL(result, data.get('peselFilter'));
        result = filterFirstName(result, data.get('firstNameFilter'));
        result = filterLastName(result, data.get('lastNameFilter'));
        result = filterEmail(result, data.get('emailFilter'));
        result = filterDate(result, data.get('dateOfBirthFilter'));
        result = filterPhoneNumber(result, data.get('phoneNumberFilter'));
        result = filterActive(result, data.get('activeFilter'));
        setFilteredRows(result);
        /*
        console.log(
            {
                filter: data.get('firstNameFilter'),
                date: data.get('dateOfBirthFilter'),
                date2: new Date(data.get('dateOfBirthFilter'))
            }
        )
        */
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
                            //onSubmit={handleSubmit}
                            sx={{
                                marginTop: 2,
                                display: 'flex',
                                flexDirection: 'row',
                                alignItems: 'center'
                            }}
                        >
                            <Grid container direction={"row"} spacing={1}>
                                <Grid item>
                                    <TextField
                                        id="idFilter"
                                        label="ID"
                                        name="idFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="peselFilter"
                                        label="PESEL"
                                        name="peselFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="firstNameFilter"
                                        label="Imię"
                                        name="firstNameFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="lastNameFilter"
                                        label="Nazwisko"
                                        name="lastNameFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="emailFilter"
                                        label="Email"
                                        name="emailFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="dateOfBirthFilter"
                                        label="Data urodzenia"
                                        name="dateOfBirthFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="phoneNumberFilter"
                                        label="Numer telefonu"
                                        name="phoneNumberFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="activeFilter"
                                        label="Aktywny"
                                        name="activeFilter"
                                    />
                                </Grid>
                            </Grid>
                        </Box>
                        <Box
                            sx={{
                                width: '100%',
                                display: 'flex',
                                flexDirection: 'column',
                                '& .active.positive': {
                                    backgroundColor: 'rgba(157, 255, 118, 0.49)',
                                    color: '#1a3e72',
                                    fontWeight: '600',
                                },
                                '& .active.negative': {
                                    backgroundColor: '#d47483',
                                    color: '#1a3e72',
                                    fontWeight: '600',
                                },
                            }}
                        >
                            <DataGrid
                                disableColumnFilter
                                autoHeight
                                pageSize={pageSize}
                                onPageSizeChange={(newPageSize) => setPageSize(newPageSize)}
                                rowsPerPageOptions={[5, 10, 15, 20]}
                                pagination
                                columns={columns}
                                rows={filteredRows}
                            />
                        </Box>
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