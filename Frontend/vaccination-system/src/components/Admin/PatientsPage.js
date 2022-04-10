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
import { confirm } from "react-confirm-box";
import { PlayCircleFilledWhiteRounded } from '@mui/icons-material';

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
                                marginBottom: 2,
                                display: 'flex',
                                //flexDirection: 'row',
                                //alignItems: 'center'
                            }}
                        >
                            <Grid container direction={"row"} spacing={1} //flex={"wrap"}
                                /*sx={{
                                    display: 'flex',
                                    flexDirection: 'row',
                                    alignItems: 'center',
                                }}*/>
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
                                onCellEditCommit={async (params, event) => {
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
                                }}
                                disableColumnFilter
                                autoHeight
                                pageSize={pageSize}
                                onPageSizeChange={(newPageSize) => setPageSize(newPageSize)}
                                rowsPerPageOptions={[5, 10, 15, 20]}
                                pagination
                                columns={columns}
                                rows={filteredRows}
                                initialState={{
                                    columns: {
                                        columnVisibilityModel: {
                                            // Hide column id, the other columns will remain visible
                                            id: false,
                                        },
                                    },
                                }
                                }
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

const confirmOptionsInPolish = {
    labels: {
        confirmable: "Tak",
        cancellable: "Nie"
    }
}