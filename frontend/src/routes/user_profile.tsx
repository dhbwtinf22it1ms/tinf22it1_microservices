import { useState } from 'react';
import { Container, Typography, Box, Paper, TextField, Button, Stack, Divider, FormControlLabel, Switch, Alert, Snackbar} from '@mui/material';

function UserProfile() {
  // TODO: Replace with actual user data from API
  const [user, setUser] = useState({
    name: "Max Mustermann",
    email: "max.mustermann@example.com",
    darkMode: false
  });

  const [newEmail, setNewEmail] = useState("");
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  
  const [notification, setNotification] = useState({
    open: false,
    message: "",
    type: "success"
  });

  const handlePasswordChange = () => {
    if (newPassword !== confirmPassword) {
      showNotification("Die Passwörter stimmen nicht überein!", "error");
      return;
    }
    
    // TODO: Replace with actual API call to change password
    showNotification("Passwort erfolgreich geändert!", "success");
    
    setCurrentPassword("");
    setNewPassword("");
    setConfirmPassword("");
  };

  const handleEmailChange = () => {
    if (!newEmail.includes('@')) {
      showNotification("Bitte geben Sie eine gültige E-Mail-Adresse ein!", "error");
      return;
    }
    
    // TODO: Replace with actual API call to change email
    setUser({...user, email: newEmail});
    showNotification("E-Mail-Adresse erfolgreich geändert!", "success");
    setNewEmail("");
  };

  const handleThemeChange = () => {
    //TODO: Replace with actual logic to change theme
    setUser({...user, darkMode: !user.darkMode});
    showNotification(`Theme auf ${!user.darkMode ? 'Dunkel' : 'Hell'} geändert!`, "success");
  };

  const showNotification = (message: string, type: "success" | "error" | "info" | "warning") => {
    setNotification({
      open: true,
      message,
      type
    });
  };

  return (
    <Container maxWidth="sm">
      <Box sx={{ my: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom align="center">
          Mein Benutzerprofil
        </Typography>
        
        <Paper elevation={3} sx={{ p: 3, mt: 3 }}>
          <Typography variant="h6" gutterBottom>
            Persönliche Informationen
          </Typography>
          <Typography>Name: {user.name}</Typography>
          <Typography>E-Mail: {user.email}</Typography>
          
          <Divider sx={{ my: 2 }} />
          
          <Typography variant="h6" gutterBottom>Passwort ändern</Typography>
          <Stack spacing={2}>
            <TextField
              label="Aktuelles Passwort"
              type="password"
              value={currentPassword}
              onChange={(e) => setCurrentPassword(e.target.value)}
              fullWidth
              size="small"
            />
            <TextField
              label="Neues Passwort"
              type="password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              fullWidth
              size="small"
            />
            <TextField
              label="Passwort bestätigen"
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              fullWidth
              size="small"
            />
            <Button 
              variant="contained" 
              onClick={handlePasswordChange}
              disabled={!currentPassword || !newPassword || !confirmPassword}
            >
              Passwort ändern
            </Button>
          </Stack>
          
          <Divider sx={{ my: 2 }} />
          
          <Typography variant="h6" gutterBottom>
            E-Mail ändern
          </Typography>
          <Stack spacing={2}>
            <TextField
              label="Neue E-Mail-Adresse"
              type="email"
              value={newEmail}
              onChange={(e) => setNewEmail(e.target.value)}
              fullWidth
              size="small"
            />
            <Button 
              variant="contained" 
              onClick={handleEmailChange}
              disabled={!newEmail}
            >
              E-Mail ändern
            </Button>
          </Stack>
          
            <Divider sx={{ my: 2 }} />
          
            <Typography variant="h6" gutterBottom>
                Einstellungen
            </Typography>
            <FormControlLabel
              control={
                <Switch 
                  checked={user.darkMode} 
                  onChange={handleThemeChange}
                />
              }
              label="Dunkles Theme"
            />
        </Paper>
      </Box>
      
        <Snackbar
        open={notification.open}
        autoHideDuration={6000}
        onClose={() => setNotification({...notification, open: false})}
        >
        <Alert severity={notification.type as "success" | "error" | "info" | "warning"} onClose={() => setNotification({...notification, open: false})}>
            {notification.message}
        </Alert>
        </Snackbar>
    </Container>
  );
}

export default UserProfile;