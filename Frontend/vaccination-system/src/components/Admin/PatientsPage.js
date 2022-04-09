import * as React from 'react';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline, TextField } from '@mui/material';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import { useNavigate } from "react-router-dom";
import { DataGrid } from '@mui/x-data-grid';
import { randomDate, randomEmail, randomId, randomPhoneNumber, randomTraderName, randomBoolean, randomInt } from '@mui/x-data-grid-generator';
import dateFormat from 'dateformat';

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
        if (filter === "")
            return true;
        var d1 = new Date(item.dateOfBirth);
        var d2 = new Date(filter);
        return d1.getTime() === d2.getTime();
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
    },
];

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
        console.log(
            {
                filter: data.get('firstNameFilter'),
                date: data.get('dateOfBirthFilter'),
                date2: new Date(data.get('dateOfBirthFilter'))
            }
        )
    };
    return (
        <ThemeProvider theme={theme}>
            <Container component="main" maxWidth="lg">
                <CssBaseline>
                    <Box
                        sx={{
                            marginTop: 8,
                            display: 'flex',
                            flexDirection: 'column',
                            alignItems: 'center',
                        }}
                    >
                        <Typography component="h1" variant='h5'>
                            Pacjenci
                        </Typography>
                        <Box
                            component='form'
                            noValidate
                            onSubmit={handleSubmit}
                            sx={{
                                display: 'flex',
                                flexDirection: 'row',
                                alignItems: 'center'
                            }}>
                            <TextField
                                id="idFilter"
                                label="Filtracja id"
                                name="idFilter"
                            />
                            <TextField
                                id="peselFilter"
                                label="Filtracja PESEL-u"
                                name="peselFilter"
                            />
                            <TextField
                                id="firstNameFilter"
                                label="Filtracja imion"
                                name="firstNameFilter"
                            />
                            <TextField
                                id="lastNameFilter"
                                label="Filtracja nazwiska"
                                name="lastNameFilter"
                            />

                            <TextField
                                id="emailFilter"
                                label="Filtracje e-maila"
                                name="emailFilter"
                            />
                            <TextField
                                id="dateOfBirthFilter"
                                label="Filtracja daty urodzenia"
                                name="dateOfBirthFilter"
                            />
                            <TextField
                                id="phoneNumberFilter"
                                label="Filtracja numeru telefonu"
                                name="phoneNumberFilter"
                            />
                            <TextField
                                id="activeFilter"
                                label="Filtracja aktywności"
                                name="activeFilter"
                            />
                            <Button
                                type='submit'
                                variant='contained'
                            >
                                Filtruj
                            </Button>
                        </Box>
                        <div style={{ width: '100%' }}>
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
                        </div>
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
            </Container>
        </ThemeProvider>
    );
}