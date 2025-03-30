import { TextField, Select, MenuItem, SelectChangeEvent, Button, Container, Paper, Typography, Box, Card, CardContent, Divider, Snackbar, Alert } from "@mui/material";
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import Grid from '@mui/material/Grid2';
import { useState } from "react";
import dayjs from 'dayjs';

function ThesesFormular() {
    const [studentTitle, setStudentTitle] = useState("Herr");
    const [supervisorTitle, setSupervisorTitle] = useState("Herr");
    const [academicTitle, setAcademicTitle] = useState("Prof. Dr.");

    const [thesisTopic, setThesisTopic] = useState("");
    const [prepStart, setPrepStart] = useState(dayjs());
    const [prepEnd, setPrepEnd] = useState(dayjs().add(6, 'month'));
    
    const [studentFirstName, setStudentFirstName] = useState("");
    const [studentLastName, setStudentLastName] = useState("");
    const [registrationNumber, setRegistrationNumber] = useState("");
    const [studentCourse, setStudentCourse] = useState("");
    
    const [supervisorFirstName, setSupervisorFirstName] = useState("");
    const [supervisorLastName, setSupervisorLastName] = useState("");
    const [supervisorDepartment, setSupervisorDepartment] = useState("");
    
    const [companyName, setCompanyName] = useState("");
    const [companyDepartment, setCompanyDepartment] = useState("");
    const [companyStreet, setCompanyStreet] = useState("");
    const [companyZip, setCompanyZip] = useState("");
    const [companyCity, setCompanyCity] = useState("");
    
    const [locationName, setLocationName] = useState("");
    const [locationDepartment, setLocationDepartment] = useState("");
    const [locationStreet, setLocationStreet] = useState("");
    const [locationZip, setLocationZip] = useState("");
    const [locationCity, setLocationCity] = useState("");
    const [locationCountry, setLocationCountry] = useState("Germany");
    
    const [excludeCompanies, setExcludeCompanies] = useState("");
    
    const [notification, setNotification] = useState({
        open: false,
        message: "",
        severity: "success" as "success" | "error" | "info" | "warning"
    });

    function handleStudentTitleChange(event: SelectChangeEvent) {
        setStudentTitle(String(event.target.value));
    }

    function handleSupervisorTitleChange(event: SelectChangeEvent) {
        setSupervisorTitle(String(event.target.value));
    }

    function handleAcademicTitleChange(event: SelectChangeEvent) {
        setAcademicTitle(String(event.target.value));
    }

    const handleSubmit = async () => {
        if (!validateForm()) {
            return;
        }

        const thesisData = {
            topic: thesisTopic,
            preparationPeriod: {
                from: prepStart.toISOString(),
                to: prepEnd.toISOString()
            },
            student: {
                title: studentTitle,
                firstName: studentFirstName,
                lastName: studentLastName,
                registrationNumber: registrationNumber,
                course: studentCourse
            },
            inCompanySupervisor: {
                title: supervisorTitle,
                academicTitle: academicTitle,
                firstName: supervisorFirstName,
                lastName: supervisorLastName,
                department: supervisorDepartment
            },
            partnerCompany: {
                name: companyName,
                department: companyDepartment,
                address: {
                    street: companyStreet,
                    zipCode: companyZip,
                    city: companyCity
                }
            },
            operationalLocation: {
                companyName: locationName || companyName,
                department: locationDepartment || companyDepartment,
                address: {
                    street: locationStreet || companyStreet,
                    zipCode: locationZip || companyZip,
                    city: locationCity || companyCity,
                    country: locationCountry
                }
            },
            excludeSupervisorsFromCompanies: excludeCompanies ? excludeCompanies.split(',').map(company => company.trim()) : []
        };

        try {
            // TODO: Replace with actual API call
            console.log("Submitting thesis data:", thesisData);
                        
            // Show success notification
            setNotification({
                open: true,
                message: "Thesis successfully submitted!",
                severity: "success"
            });
            
            // TODO: Add Redirect or further actions after successful submission
            resetForm();
        } catch (error) {
            console.error("Error submitting thesis:", error);
            setNotification({
                open: true,
                message: "Error submitting thesis. Please try again.",
                severity: "error"
            });
        }
    };

    const validateForm = () => {
        //TODO: Add validation logic for required fields
        
        return true; //TODO: Replace with actual validation result
    };

    const resetForm = () => {
        setThesisTopic("");
        setPrepStart(dayjs());
        setPrepEnd(dayjs().add(6, 'month'));
        setStudentFirstName("");
        setStudentLastName("");
        setRegistrationNumber("");
        setStudentCourse("");
        setSupervisorFirstName("");
        setSupervisorLastName("");
        setSupervisorDepartment("");
        setCompanyName("");
        setCompanyDepartment("");
        setCompanyStreet("");
        setCompanyZip("");
        setCompanyCity("");
        setLocationName("");
        setLocationDepartment("");
        setLocationStreet("");
        setLocationZip("");
        setLocationCity("");
        setLocationCountry("Germany");
        setExcludeCompanies("");
    };

    // Close notification
    const handleCloseNotification = () => {
        setNotification({...notification, open: false});
    };

    const textFieldStyle = { width: '100%' };

    return (
        <Container maxWidth="lg" sx={{ py: 4 }}>
            <Paper elevation={3} sx={{ p: 3, borderRadius: 2 }}>
                <Typography variant="h4" component="h1" gutterBottom sx={{ mb: 3, fontWeight: 'bold', color: 'primary.main' }}>
                    Theses Formular
                </Typography>

                <Card sx={{ mb: 4 }}>
                    <CardContent>
                        <Typography variant="h5" component="h2" gutterBottom sx={{ mb: 2 }}>
                            Thesis Details
                        </Typography>
                        <Grid container spacing={3}>
                            <Grid sx={{ xs:12, md:6 }}>
                                <TextField 
                                    id="thesis-topic" 
                                    label="Thesis Topic" 
                                    variant="outlined" 
                                    fullWidth
                                    value={thesisTopic}
                                    onChange={(e) => setThesisTopic(e.target.value)}
                                    required
                                    sx={textFieldStyle}
                                />
                            </Grid>
                            <Grid sx={{ xs:12, md:3 }}>
                                <LocalizationProvider dateAdapter={AdapterDayjs}>
                                    <DatePicker 
                                        label="Preparation Start"
                                        value={prepStart}
                                        onChange={(date) => date && setPrepStart(date)}
                                        sx={textFieldStyle} 
                                    />
                                </LocalizationProvider>
                            </Grid>
                            <Grid sx={{ xs:12, md:3 }}>
                                <LocalizationProvider dateAdapter={AdapterDayjs}>
                                    <DatePicker 
                                        label="Preparation End"
                                        value={prepEnd}
                                        onChange={(date) => date && setPrepEnd(date)}
                                        sx={textFieldStyle} 
                                    />
                                </LocalizationProvider>
                            </Grid>
                        </Grid>
                    </CardContent>
                </Card>

                <Card sx={{ mb: 4 }}>
                    <CardContent>
                        <Typography variant="h5" component="h2" gutterBottom sx={{ mb: 2 }}>
                            Student Information
                        </Typography>
                        <Grid container spacing={3}>
                            <Grid sx={{ xs:12, sm:2, md:1 }}>
                                <Select
                                    value={studentTitle}
                                    label="Title"
                                    onChange={handleStudentTitleChange}
                                    fullWidth
                                >
                                    <MenuItem value="Herr">Herr</MenuItem>
                                    <MenuItem value="Frau">Frau</MenuItem>
                                    <MenuItem value="Divers">Divers</MenuItem>
                                </Select>
                            </Grid>
                            <Grid sx={{ xs:12, sm:5, md:3 }}>
                                <TextField 
                                    id="student-first-name" 
                                    label="First Name" 
                                    variant="outlined" 
                                    value={studentFirstName}
                                    onChange={(e) => setStudentFirstName(e.target.value)}
                                    required
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                            <Grid sx={{ xs:12, sm:5, md:3 }}>
                                <TextField 
                                    id="student-last-name" 
                                    label="Last Name" 
                                    variant="outlined" 
                                    value={studentLastName}
                                    onChange={(e) => setStudentLastName(e.target.value)}
                                    required
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                            <Grid sx={{ xs:12, sm:6, md:2.5 }}>
                                <TextField 
                                    id="registration-number" 
                                    label="Registration Number" 
                                    variant="outlined" 
                                    value={registrationNumber}
                                    onChange={(e) => setRegistrationNumber(e.target.value)}
                                    required
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                            <Grid sx={{ xs:12, sm:6, md:2.5 }}>
                                <TextField 
                                    id="student-course" 
                                    label="Course" 
                                    variant="outlined" 
                                    value={studentCourse}
                                    onChange={(e) => setStudentCourse(e.target.value)}
                                    required
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                        </Grid>
                    </CardContent>
                </Card>

                <Card sx={{ mb: 4 }}>
                    <CardContent>
                        <Typography variant="h5" component="h2" gutterBottom sx={{ mb: 2 }}>
                            In Company Supervisor
                        </Typography>
                        <Grid container spacing={3}>
                            <Grid sx={{ xs:4, sm:2, md:1 }}>
                                <Select
                                    value={supervisorTitle}
                                    label="Title"
                                    onChange={handleSupervisorTitleChange}
                                    fullWidth
                                >
                                    <MenuItem value="Herr">Herr</MenuItem>
                                    <MenuItem value="Frau">Frau</MenuItem>
                                    <MenuItem value="Divers">Divers</MenuItem>
                                </Select>
                            </Grid>
                            <Grid sx={{ xs:8, sm:4, md:4 }}>
                                <Select
                                    value={academicTitle}
                                    label="Academic Title"
                                    onChange={handleAcademicTitleChange}
                                    fullWidth
                                >
                                    <MenuItem value="Prof. Dr.">Prof. Dr.</MenuItem>
                                    <MenuItem value="Dr.">Dr.</MenuItem>
                                    <MenuItem value="Prof.">Prof.</MenuItem>
                                </Select>
                            </Grid>
                            <Grid sx={{ xs:12, sm:6, md:3 }}>
                                <TextField 
                                    id="supervisor-first-name" 
                                    label="First Name" 
                                    variant="outlined" 
                                    value={supervisorFirstName}
                                    onChange={(e) => setSupervisorFirstName(e.target.value)}
                                    required
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                            <Grid sx={{ xs:12, sm:6, md:3 }}>
                                <TextField 
                                    id="supervisor-last-name" 
                                    label="Last Name" 
                                    variant="outlined" 
                                    value={supervisorLastName}
                                    onChange={(e) => setSupervisorLastName(e.target.value)}
                                    required
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                            <Grid sx={{ xs:12, sm:6 }}>
                                <TextField 
                                    id="department" 
                                    label="Department" 
                                    variant="outlined" 
                                    value={supervisorDepartment}
                                    onChange={(e) => setSupervisorDepartment(e.target.value)}
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                        </Grid>
                    </CardContent>
                </Card>

                <Card sx={{ mb: 4 }}>
                    <CardContent>
                        <Typography variant="h5" component="h2" gutterBottom sx={{ mb: 2 }}>
                            Partner Company
                        </Typography>
                        <Grid container spacing={3}>
                            <Grid sx={{ xs:12, md:6 }}>
                                <TextField 
                                    id="company-name" 
                                    label="Company Name" 
                                    variant="outlined" 
                                    value={companyName}
                                    onChange={(e) => setCompanyName(e.target.value)}
                                    required
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                            <Grid sx={{ xs:12, md:6 }}>
                                <TextField 
                                    id="company-department" 
                                    label="Department" 
                                    variant="outlined" 
                                    value={companyDepartment}
                                    onChange={(e) => setCompanyDepartment(e.target.value)}
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                            <Grid sx={{ xs:12, md:6 }}>
                                <TextField 
                                    id="company-street" 
                                    label="Street" 
                                    variant="outlined" 
                                    value={companyStreet}
                                    onChange={(e) => setCompanyStreet(e.target.value)}
                                    required
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                            <Grid sx={{ xs:12, sm:4, md:2 }}>
                                <TextField 
                                    id="company-zip" 
                                    label="Zip Code" 
                                    variant="outlined" 
                                    value={companyZip}
                                    onChange={(e) => setCompanyZip(e.target.value)}
                                    required
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                            <Grid sx={{ xs:12, sm:8, md:4 }}>
                                <TextField 
                                    id="company-city" 
                                    label="City" 
                                    variant="outlined" 
                                    value={companyCity}
                                    onChange={(e) => setCompanyCity(e.target.value)}
                                    required
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                        </Grid>
                    </CardContent>
                </Card>

                <Card sx={{ mb: 4 }}>
                    <CardContent>
                        <Typography variant="h5" component="h2" gutterBottom sx={{ mb: 2 }}>
                            Operational Location
                        </Typography>
                        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                            Leave empty if same as partner company
                        </Typography>
                        <Grid container spacing={3}>
                            <Grid sx={{ xs:12, md:6 }}>
                                <TextField 
                                    id="location-name" 
                                    label="Company Name" 
                                    variant="outlined" 
                                    value={locationName}
                                    onChange={(e) => setLocationName(e.target.value)}
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                            <Grid sx={{ xs:12, md:6 }}>
                                <TextField 
                                    id="location-department" 
                                    label="Department" 
                                    variant="outlined" 
                                    value={locationDepartment}
                                    onChange={(e) => setLocationDepartment(e.target.value)}
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                            <Grid sx={{ xs:12, md:6 }}>
                                <TextField 
                                    id="location-street" 
                                    label="Street" 
                                    variant="outlined" 
                                    value={locationStreet}
                                    onChange={(e) => setLocationStreet(e.target.value)}
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                            <Grid sx={{ xs:12, sm:4, md:2 }}>
                                <TextField 
                                    id="location-zip" 
                                    label="Zip Code" 
                                    variant="outlined" 
                                    value={locationZip}
                                    onChange={(e) => setLocationZip(e.target.value)}
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                            <Grid sx={{ xs:12, sm:4, md:2 }}>
                                <TextField 
                                    id="location-city" 
                                    label="City" 
                                    variant="outlined" 
                                    value={locationCity}
                                    onChange={(e) => setLocationCity(e.target.value)}
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                            <Grid sx={{ xs:12, sm:4, md:2 }}>
                                <TextField 
                                    id="location-country" 
                                    label="Country" 
                                    variant="outlined" 
                                    value={locationCountry}
                                    onChange={(e) => setLocationCountry(e.target.value)}
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                        </Grid>
                    </CardContent>
                </Card>

                <Card sx={{ mb: 4 }}>
                    <CardContent>
                        <Typography variant="h5" component="h2" gutterBottom sx={{ mb: 2 }}>
                            Exclude Supervisors from Companies
                        </Typography>
                        <Grid container spacing={3}>
                            <Grid sx={{ xs:12 }}>
                                <TextField 
                                    id="exclude-companies" 
                                    label="Company Names (comma separated)" 
                                    variant="outlined" 
                                    value={excludeCompanies}
                                    onChange={(e) => setExcludeCompanies(e.target.value)}
                                    sx={textFieldStyle} 
                                />
                            </Grid>
                        </Grid>
                    </CardContent>
                </Card>

                <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
                    <Button 
                        variant="contained" 
                        color="primary" 
                        size="large"
                        onClick={handleSubmit}
                        sx={{ px: 5, py: 1.5, borderRadius: 2, fontWeight: 'bold' }}
                    >
                        Submit
                    </Button>
                </Box>
            </Paper>
            
            <Snackbar
                open={notification.open}
                autoHideDuration={6000}
                onClose={handleCloseNotification}
            >
                <Alert 
                    onClose={handleCloseNotification} 
                    severity={notification.severity}
                    sx={{ width: '100%' }}
                >
                    {notification.message}
                </Alert>
            </Snackbar>
        </Container>
    )
}

export default ThesesFormular;