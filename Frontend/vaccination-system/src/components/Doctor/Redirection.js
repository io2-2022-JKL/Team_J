import * as React from 'react';
import { createTheme } from '@mui/material/styles';
import { Tab, Tabs } from '@mui/material';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import { useNavigate } from "react-router-dom";
import LoginHelpers from '../../tools/LoginHelpers';
import PropTypes from 'prop-types';
import PatientMainPage from '../Patient/PatientMainPage';
import DoctorMainPage from './DoctorMainPage';

const theme = createTheme();

export default function Redirection() {
    const navigate = useNavigate();
    React.useEffect(() => {
        LoginHelpers.preventGoingBack();
    }, [])

    const [value, setValue] = React.useState(0);

    const handleChange = (event, newValue) => {
        setValue(newValue);
    };

    function TabPanel(props) {
        const { children, value, index, ...other } = props;

        return (
            <div
                role="tabpanel"
                hidden={value !== index}
                id={`simple-tabpanel-${index}`}
                aria-labelledby={`simple-tab-${index}`}
                {...other}
            >
                {value === index && (
                    <Box sx={{ p: 3 }}>
                        <Typography>{children}</Typography>
                    </Box>
                )}
            </div>
        );
    }

    TabPanel.propTypes = {
        children: PropTypes.node,
        index: PropTypes.number.isRequired,
        value: PropTypes.number.isRequired,
    };

    function a11yProps(index) {
        return {
            id: `simple-tab-${index}`,
            'aria-controls': `simple-tabpanel-${index}`,
        };
    }

    return (
        <Box sx={{ width: '100%' }}>
            <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                <Tabs value={value} onChange={handleChange} aria-label="basic tabs example">
                    <Tab label="Strona pacjenta" {...a11yProps(0)} />
                    <Tab label="Strona lekarza" {...a11yProps(1)} />
                </Tabs>
            </Box>
            <TabPanel name="patientTab" value={value} index={0}>
                <PatientMainPage />
            </TabPanel>
            <TabPanel name="doctorTab" value={value} index={1}>
                <DoctorMainPage />
            </TabPanel>
        </Box>
    );
}