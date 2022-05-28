import * as React from 'react';
import { Box } from '@mui/material';
import { FixedSizeList } from 'react-window';
import Button from '@mui/material/Button';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { Container, CssBaseline } from '@mui/material';
import Typography from '@mui/material/Typography';
import CircularProgress from '@mui/material/CircularProgress';
import { blue } from '@mui/material/colors';
import SummarizeIcon from '@mui/icons-material/Summarize';
import Avatar from '@mui/material/Avatar';
import { ErrorSnackbar } from '../tools/Snackbars';

const theme = createTheme();


export default function ItemListPageTemplate(props) //title, data, renderRow, renderError, errorState, error, handleSnackBarClose, handleBack, loading) 
{
    return (
        <ThemeProvider theme={theme}>
            <Container component="main" maxWidth="lg">
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
                            <SummarizeIcon />
                        </Avatar>
                        <Typography component="h1" variant='h5'>
                            {props.title}
                        </Typography>

                        <Box sx={{ marginTop: 2, }} />

                        <FixedSizeList
                            height={Math.min(window.innerHeight - 200, props.data.length * 100)}
                            width="70%"
                            itemSize={100}
                            itemCount={props.data.length}
                            overscanCount={5}
                            itemData={props.data}
                        >
                            {props.renderRow}
                        </FixedSizeList>
                        <Button
                            type="submit"
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                            onClick={props.handleBack}
                        >
                            Powrót
                        </Button>
                        {
                            props.loading &&
                            (
                                <CircularProgress
                                    size={24}
                                    sx={{
                                        color: blue,
                                        position: 'relative',
                                        alignSelf: 'center',
                                        left: '50%'
                                    }}
                                />
                            )
                        }
                    </Box>
                    <ErrorSnackbar
                        error = {props.error}
                        errorState = {props.errorState}
                        setErrorState = {props.setErrorState}
                    />
                </CssBaseline>
            </Container>
        </ThemeProvider>
    );
}