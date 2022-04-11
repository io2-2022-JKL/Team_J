import * as React from 'react';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline, TextField } from '@mui/material';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import { useNavigate } from "react-router-dom";
import Grid from "@material-ui/core/Grid";
import { DataGrid, GridActionsCellItem } from '@mui/x-data-grid';
import { randomDate, randomEmail, randomId, randomPhoneNumber, randomTraderName, randomBoolean, randomInt, randomCompanyName } from '@mui/x-data-grid-generator';
import dateFormat from 'dateformat';
import DeleteIcon from '@mui/icons-material/Delete';
import clsx from 'clsx';
import VaccinesIcon from '@mui/icons-material/Vaccines';
import Avatar from '@mui/material/Avatar';
import { confirm } from "react-confirm-box";
import { PlayCircleFilledWhiteRounded } from '@mui/icons-material';

const theme = createTheme();

const filterName = (array, filter) => {
    return array.filter((item) => item.name.includes(filter));
};
const filterNumberOfDoses = (array, filter) => {
    return array.filter((item) => {
        if(filter=='')
            return true;
        return item.numberOfDoses == filter
    });
};
const filterMinDaysBetweenDoses = (array, filter) => {
    return array.filter((item) => {
        if(filter=='')
            return true;
        return item.minDaysBetweenDoses == filter
    });
};
const filterMaxDaysBetweenDoses = (array, filter) => {
    return array.filter((item) => {
        if(filter=='')
            return true;
        return item.maxDaysBetweenDoses == filter
    });
};
const filterVirus = (array, filter) => {
    return array.filter((item) => item.virus.includes(filter));
};
const filterCompany = (array, filter) => {
    return array.filter((item) => item.company.includes(filter));
};

const filterId = (array, filter) => {
    return array.filter((item) => item.id.includes(filter));
};
const filterMinPatientAge = (array, filter) => {
    return array.filter((item) => {
        if(filter=='')
            return true;
        return item.minPatientAge == filter
    });
};
const filterMaxPatientAge = (array, filter) => {
    return array.filter((item) => {
        if(filter=='')
            return true;
        return item.maxPatientAge == filter
    });
};
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
        company: randomCompanyName(),
        name: randomCompanyName(),
        numberOfDoses: randomInt(1,3),
        minDaysBetweenDoses: randomInt(14,30),
        maxDaysBetweenDoses: randomInt(30,60),
        virus: "Koronavirus",
        minPatientAge: randomInt(18,21),
        maxPatientAge: randomInt(80,120),
        active: randomBoolean() 
    }
};

export default function DoctorsPage() {

    const navigate = useNavigate();

    const [pageSize, setPageSize] = React.useState(10);

    const columns = [
        {
            field: 'id',
            flex: 2
        },
        {
            field: 'company',
            headerName: 'Firma',
            minWidth: 110,
            flex: 0.5,
            editable: true
        },
        {
            field: 'name',
            headerName: 'Nazwa',
            flex: 0.5,
            editable: true
        },
        {
            field: 'numberOfDoses',
            headerName: 'Liczba dawek',
            flex: 0.5,
            editable: true
        },
        {
            field: 'minDaysBetweenDoses',
            headerName: 'Minimalna liczba dni pomiędzy dawkami',
            minWidth: 125,
            flex: 0.5,
            editable: true
        },
        {
            field: 'maxDaysBetweenDoses',
            headerName: 'Maksymalna liczba dni pomiędzy dawkami',
            minWidth: 125,
            flex: 0.5,
            editable: true
        },
        {
            field: 'virus',
            headerName: 'Nazwa wirusa',
            flex: 0.75,
            editable: true
        },
        {
            field: 'minPatientAge',
            headerName: 'Minimalny wiek pacjenta',
            minWidth: 125,
            flex: 0.5,
            editable: true
        },
        {
            field: 'maxPatientAge',
            headerName: 'Maksymalny wiek pacjenta',
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
        
        result = filterId(result, data.get('vaccineIdFilter'));
        result = filterCompany(result, data.get('companyFilter'));
        result = filterName(result, data.get('nameFilter'));
        result = filterNumberOfDoses(result, data.get('numberOfDosesFilter'));
        result = filterActive(result, data.get('activeFilter'));
        result = filterMinDaysBetweenDoses(result, data.get('minDaysBetweenDosesFilter'));
        result = filterMaxDaysBetweenDoses(result, data.get('maxDaysBetweenDosesFilter'));
        result = filterVirus(result, data.get('virusFilter'));
        result = filterMinPatientAge(result,data.get('minPatientAgeFilter'));
        result = filterMaxPatientAge(result,data.get('maxPatientAgeFilter'));
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
                            <VaccinesIcon />
                        </Avatar>
                        <Typography component="h1" variant='h4'>
                            Szczepionki
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
                                <Grid item>
                                    <TextField
                                        id="vaccineIdFilter"
                                        label="vaccineID"
                                        name="vaccineIdFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="companyFilter"
                                        label="Firma"
                                        name="companyFilter"
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
                                        id="numberOfDosesFilter"
                                        label="Liczba dawek"
                                        name="numberOfDosesFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="minDaysBetweenDosesFilter"
                                        label="Minimalna liczba dni pomiędzy dawkami"
                                        name="minDaysBetweenDosesFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="maxDaysBetweenDosesFilter"
                                        label="Maksymalna liczba dni pomiędzy dawkami"
                                        name="maxDaysBetweenDosesFilter"
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
                                        id="minPatinetAgeFilter"
                                        label="Minimalny wiek pacjenta"
                                        name="minPatientAgeFilter"
                                    />
                                </Grid>
                                <Grid item>
                                    <TextField
                                        id="maxPatientAgeFilter"
                                        label="Maksymalny wiek pacjenta"
                                        name="maxPatientAgeFilter"
                                    />
                                </Grid>
                                <Grid item >
                                    <TextField
                                        id="activeFilter"
                                        label="Aktywny"
                                        name="activeFilter"
                                    />
                                </Grid>
                            </Grid>
                            <Button variant='outlined' onClick={() => { navigate("/admin/vaccines/addVaccine") }}>
                                Dodaj nową szczepionkę
                            </Button>
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
                                    const result = await confirm("Czy na pewno chcesz edytować szczepionkę?", confirmOptionsInPolish);
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
                                    console.log(params.row.company);
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