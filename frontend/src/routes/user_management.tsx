import { useState } from 'react';
import './user_management.css';
import { 
  TextField, 
  Button, 
  Select, 
  MenuItem, 
  FormControl, 
  InputLabel, 
  Paper, 
  Typography, 
  IconButton 
} from '@mui/material';
import Grid from '@mui/material/Grid2';
import DeleteIcon from '@mui/icons-material/Delete';

function UserManagement() {
    const [users, setUsers] = useState([
        { id: 1, name: 'Max Mustermann', email: 'max@example.com', role: 'Admin' },
        { id: 2, name: 'Erika Musterfrau', email: 'erika@example.com', role: 'User' }
    ]);
    
    const [formData, setFormData] = useState({
        name: '',
        email: '',
        role: 'User'
    });
    
    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData({
            ...formData,
            [name]: value
        });
    };
    
    const handleSubmit = (e) => {
        e.preventDefault();
        // TODO: Add API call to save the new user
        const newUser = {
            id: users.length + 1,
            ...formData
        };
        setUsers([...users, newUser]);
        setFormData({ name: '', email: '', role: 'User' });
    };
    
    const deleteUser = (id) => {
        //TODO: Add API call to delete the user
        setUsers(users.filter(user => user.id !== id));
    };

    return (
        <div className="user-management-container">
            <Typography variant="h4" component="h1" gutterBottom className="user-management-title">
                Benutzerverwaltung
            </Typography>
            
            <Paper elevation={3} className="user-form-container">
                <Typography variant="h5" component="h2" gutterBottom>
                    Neuen Benutzer anlegen
                </Typography>
                <form onSubmit={handleSubmit} className="user-form">
                    <Grid container spacing={4}>
                        <Grid sx={{ xs: 3, sm: 6 }}>
                            <TextField
                                fullWidth
                                label="Name"
                                name="name"
                                variant="outlined"
                                value={formData.name}
                                onChange={handleInputChange}
                                required
                                placeholder="Vollständiger Name"
                                margin="normal"
                            />
                        </Grid>
                        
                        <Grid sx={{ xs:3, sm:6 }}>
                            <TextField
                                fullWidth
                                label="E-Mail"
                                name="email"
                                type="email"
                                variant="outlined"
                                value={formData.email}
                                onChange={handleInputChange}
                                required
                                placeholder="email@beispiel.de"
                                margin="normal"
                            />
                        </Grid>
                        
                        <Grid sx={{ xs:3 }}>
                            <FormControl fullWidth variant="outlined" margin="normal">
                                <InputLabel id="role-label">Rolle</InputLabel>
                                <Select
                                    labelId="role-label"
                                    id="role"
                                    name="role"
                                    value={formData.role}
                                    onChange={handleInputChange}
                                    label="Rolle"
                                >
                                    <MenuItem value="Admin">Administrator</MenuItem>
                                    <MenuItem value="User">Benutzer</MenuItem>
                                    <MenuItem value="Guest">Gast</MenuItem>
                                </Select>
                            </FormControl>
                        </Grid>
                        
                        <Grid sx={{ textAlign: 'right', xs: 3}}>
                            <Button 
                                type="submit" 
                                variant="contained" 
                                color="primary" 
                                size="large"
                                sx={{ m: '20px'}}
                            >
                                Benutzer hinzufügen
                            </Button>
                        </Grid>
                    </Grid>
                </form>
            </Paper>
            
            <Paper elevation={3} className="user-list-container">
                <Typography variant="h5" component="h2" gutterBottom>
                    Benutzerübersicht
                </Typography>
                <table className="user-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Name</th>
                            <th>E-Mail</th>
                            <th>Rolle</th>
                            <th>Aktionen</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.map(user => (
                            <tr key={user.id}>
                                <td>{user.id}</td>
                                <td>{user.name}</td>
                                <td>{user.email}</td>
                                <td>{user.role}</td>
                                <td>
                                    <IconButton 
                                        color="error" 
                                        aria-label="löschen"
                                        onClick={() => deleteUser(user.id)}>
                                    <DeleteIcon />
                                    </IconButton>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </Paper>
        </div>
    );
}

export default UserManagement;