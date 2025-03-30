import { useParams } from 'react-router';
import { useState, useEffect } from 'react';
import { 
  Typography, Box, Paper, Container, Divider, 
  Card, CardContent, Table, TableBody, TableCell, 
  TableContainer, TableRow, Chip, CircularProgress, Alert
} from '@mui/material';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import BusinessIcon from '@mui/icons-material/Business';
import PersonIcon from '@mui/icons-material/Person';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import BlockIcon from '@mui/icons-material/Block';
import Grid from '@mui/material/Grid2';

import ThesisDetails from '../interfaces/thesisDetails';

const formatDate = (dateString: string): string => {
  const date = new Date(dateString);
  return date.toLocaleDateString('de-DE', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  });
};

// TODO: Remove this mock data and replace it with an API call to fetch the thesis details
const mockThesisData: ThesisDetails = {
  id: 38729,
  topic: "Thesis mangagement systems and their importance for universities",
  student: {
    id: 83791,
    title: "Herr",
    firstName: "Max",
    lastName: "Mustermann",
    registrationNumber: "1234567",
    course: "TINF22IT1"
  },
  preparationPeriod: {
    from: "2024-06-01T00:00:00+0000",
    to: "2025-01-01T23:59:59+0000"
  },
  partnerCompany: {
    name: "DHBW Innovation Center",
    address: {
      street: "Coblitzallee",
      zipCode: 68163,
      city: "Mannheim"
    }
  },
  operationalLocation: {
    companyName: "DHBW Innovation Center",
    department: "Administration",
    address: {
      street: "Coblitzallee",
      zipCode: 68163,
      city: "Mannheim",
      country: "Germany"
    }
  },
  inCompanySupervisor: {
    title: "Frau",
    academicTitle: "Prof. Dr.",
    firstName: "Erika",
    lastName: "Musterfrau",
    phoneNumber: "+49 0621000000",
    email: "erika.musterfrau@dhbw-mannheim.de",
    academicDegree: "Master of Science"
  },
  excludeSupervisorsFromCompanies: [
    "Universität Mannheim"
  ]
};

function Thesis() {
  let { id } = useParams();
  const [thesis, setThesis] = useState<ThesisDetails | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    // TODO: Replace with actual API call to fetch thesis data
    const fetchThesisData = async () => {
      try {        
        // TODO: Remove this mock data and replace it with an API call to fetch the thesis details
        setThesis(mockThesisData);
        setLoading(false);
      } catch (e) {
        setError("Fehler beim Laden der Thesis-Daten.");
        setLoading(false);
      }
    };

    fetchThesisData();
  }, [id]);

  if (loading) {
    return (
      <Container>
        <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
          <CircularProgress />
        </Box>
      </Container>
    );
  }

  if (error) {
    return (
      <Container>
        <Alert severity="error" sx={{ my: 2 }}>{error}</Alert>
      </Container>
    );
  }

  if (!thesis) {
    return (
      <Container>
        <Alert severity="info" sx={{ my: 2 }}>Keine Thesis-Daten gefunden.</Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="md">
      <Paper elevation={3} sx={{ p: 3, my: 4 }}>
        <Box sx={{ mb: 3 }}>
          <Typography variant="h4" component="h1" gutterBottom>
            Bachelor Thesis
          </Typography>
          <Typography variant="h5" color="primary" gutterBottom>
            {thesis.topic}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Thesis ID: {thesis.id}
          </Typography>
        </Box>

        <Divider sx={{ my: 3 }} />

        {/* Student Information */}
        <Box sx={{ mb: 3 }}>
          <Grid container spacing={2} alignItems="center" sx={{ mb: 1 }}>
            <Grid sx={{ xs: 'auto' }}>
              <PersonIcon color="primary" />
            </Grid>
            <Grid>
              <Typography variant="h6">Student</Typography>
            </Grid>
          </Grid>
          
          <Card variant="outlined" sx={{ mb: 2 }}>
            <CardContent>
              <Typography variant="h6">
                {thesis.student.title} {thesis.student.firstName} {thesis.student.lastName}
              </Typography>
              <TableContainer>
                <Table size="small">
                  <TableBody>
                    <TableRow>
                      <TableCell component="th" scope="row" sx={{ fontWeight: 'bold', width: '40%' }}>
                        Matrikelnummer
                      </TableCell>
                      <TableCell>{thesis.student.registrationNumber}</TableCell>
                    </TableRow>
                    <TableRow>
                      <TableCell component="th" scope="row" sx={{ fontWeight: 'bold' }}>
                        Studiengang
                      </TableCell>
                      <TableCell>{thesis.student.course}</TableCell>
                    </TableRow>
                    <TableRow>
                      <TableCell component="th" scope="row" sx={{ fontWeight: 'bold' }}>
                        Student ID
                      </TableCell>
                      <TableCell>{thesis.student.id}</TableCell>
                    </TableRow>
                  </TableBody>
                </Table>
              </TableContainer>
            </CardContent>
          </Card>
        </Box>

        {/* Preparation Period */}
        <Box sx={{ mb: 3 }}>
          <Grid container spacing={2} alignItems="center" sx={{ mb: 1 }}>
            <Grid sx={{ xs: 'auto' }}>
              <CalendarTodayIcon color="primary" />
            </Grid>
            <Grid>
              <Typography variant="h6">Bearbeitungszeitraum</Typography>
            </Grid>
          </Grid>
          
          <Card variant="outlined" sx={{ mb: 2 }}>
            <CardContent>
              <Grid container spacing={2}>
                <Grid sx={{ xs:12, sm:6 }}>
                  <Typography variant="body1">
                    Von: <strong>{formatDate(thesis.preparationPeriod.from)}</strong>
                  </Typography>
                </Grid>
                <Grid sx={{ xs:12, sm:6 }}>
                  <Typography variant="body1">
                    Bis: <strong>{formatDate(thesis.preparationPeriod.to)}</strong>
                  </Typography>
                </Grid>
              </Grid>
            </CardContent>
          </Card>
        </Box>

        {/* Partner Company */}
        <Box sx={{ mb: 3 }}>
          <Grid container spacing={2} alignItems="center" sx={{ mb: 1 }}>
            <Grid sx={{ xs: 'auto' }}>
              <BusinessIcon color="primary" />
            </Grid>
            <Grid>
              <Typography variant="h6">Kooperationspartner</Typography>
            </Grid>
          </Grid>
          
          <Card variant="outlined" sx={{ mb: 2 }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                {thesis.partnerCompany.name}
              </Typography>
              <Typography variant="body1">
                {thesis.partnerCompany.address.street}
              </Typography>
              <Typography variant="body1">
                {thesis.partnerCompany.address.zipCode} {thesis.partnerCompany.address.city}
              </Typography>
            </CardContent>
          </Card>
        </Box>

        {/* Operational Location */}
        <Box sx={{ mb: 3 }}>
          <Grid container spacing={2} alignItems="center" sx={{ mb: 1 }}>
            <Grid sx={{ xs: 'auto' }}>
              <LocationOnIcon color="primary" />
            </Grid>
            <Grid>
              <Typography variant="h6">Einsatzort</Typography>
            </Grid>
          </Grid>
          
          <Card variant="outlined" sx={{ mb: 2 }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                {thesis.operationalLocation.companyName}
              </Typography>
              <Typography variant="body1" gutterBottom>
                Abteilung: {thesis.operationalLocation.department}
              </Typography>
              <Typography variant="body1">
                {thesis.operationalLocation.address.street}
              </Typography>
              <Typography variant="body1">
                {thesis.operationalLocation.address.zipCode} {thesis.operationalLocation.address.city}
              </Typography>
              {thesis.operationalLocation.address.country && (
                <Typography variant="body1">
                  {thesis.operationalLocation.address.country}
                </Typography>
              )}
            </CardContent>
          </Card>
        </Box>

        {/* In-Company Supervisor */}
        <Box sx={{ mb: 3 }}>
          <Grid container spacing={2} alignItems="center" sx={{ mb: 1 }}>
            <Grid sx={{ xs: 'auto' }}>
              <PersonIcon color="primary" />
            </Grid>
            <Grid>
              <Typography variant="h6">Betreuende Person</Typography>
            </Grid>
          </Grid>
          
          <Card variant="outlined" sx={{ mb: 2 }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                {thesis.inCompanySupervisor.academicTitle} {thesis.inCompanySupervisor.title} {thesis.inCompanySupervisor.firstName} {thesis.inCompanySupervisor.lastName}
              </Typography>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                {thesis.inCompanySupervisor.academicDegree}
              </Typography>
              <TableContainer>
                <Table size="small">
                  <TableBody>
                    <TableRow>
                      <TableCell component="th" scope="row" sx={{ fontWeight: 'bold', width: '40%' }}>
                        Telefon
                      </TableCell>
                      <TableCell>{thesis.inCompanySupervisor.phoneNumber}</TableCell>
                    </TableRow>
                    <TableRow>
                      <TableCell component="th" scope="row" sx={{ fontWeight: 'bold' }}>
                        E-Mail
                      </TableCell>
                      <TableCell>{thesis.inCompanySupervisor.email}</TableCell>
                    </TableRow>
                  </TableBody>
                </Table>
              </TableContainer>
            </CardContent>
          </Card>
        </Box>

        {/* Excluded Supervisors */}
        {thesis.excludeSupervisorsFromCompanies && thesis.excludeSupervisorsFromCompanies.length > 0 && (
          <Box sx={{ mb: 3 }}>
            <Grid container spacing={2} alignItems="center" sx={{ mb: 1 }}>
              <Grid sx={{ xs: 'auto' }}>
                <BlockIcon color="primary" />
              </Grid>
              <Grid>
                <Typography variant="h6">Ausgeschlossene Unternehmen für Betreuer</Typography>
              </Grid>
            </Grid>
            
            <Card variant="outlined">
              <CardContent>
                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                  {thesis.excludeSupervisorsFromCompanies.map((company, index) => (
                    <Chip key={index} label={company} color="error" variant="outlined" />
                  ))}
                </Box>
              </CardContent>
            </Card>
          </Box>
        )}
      </Paper>
    </Container>
  );
}

export default Thesis;
