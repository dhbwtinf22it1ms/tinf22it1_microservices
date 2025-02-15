import { AppBar, Box, Stack, Toolbar, Typography } from '@mui/material'
import { Link, Outlet } from 'react-router'
import Person from '@mui/icons-material/Person'

function Root() {
  return (
    <>
      <Box sx={{ flexGrow: 1 }}>
        <AppBar position="static">
          <Toolbar disableGutters sx={{ columnGap: 2, mx: 2, p: 0, my: 0 }}>
            <Typography component={Link} to="/mythesis">My thesis</Typography>
            <Typography component={Link} to="/theses">All theses</Typography>
            <Typography component={Link} to="/usermanagement" sx={{ flexGrow: 1 }}>Manage users</Typography>
            <Person />
            <Stack>
              <Typography>Max Mustermann</Typography>
              <Stack direction="row">
                <Typography component={Link} to="/profile">My profile</Typography>
                <Typography component={Link} to="/logout">Logout</Typography>
              </Stack>
            </Stack>
          </Toolbar>
        </AppBar>
      </Box >
      <Outlet />
    </>
  )
}

export default Root
