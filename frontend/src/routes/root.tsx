import { AppBar, Box, Stack, Toolbar, Typography, ThemeProvider, Link } from '@mui/material'
import { Link as RouterLink, Outlet } from 'react-router'
import PersonIcon from '@mui/icons-material/Person'
import LogoutIcon from '@mui/icons-material/Logout'
import ArticleIcon from '@mui/icons-material/Article';
import FormatListBulletedIcon from '@mui/icons-material/FormatListBulleted';
import PeopleIcon from '@mui/icons-material/People';
import theme from '../theme/theme'

function Root() {
  return (
    <>
      <ThemeProvider theme={theme}>
        <Box sx={{ flexGrow: 1 }}>
          <AppBar position="static">
            <Toolbar disableGutters sx={{ columnGap: 4, mx: 2, p: 0, my: 0 }}>
              <Box sx={{ '&:hover': { bgcolor: 'primary.dark', m: 'auto', borderRadius: 1 }, p: 1, borderRadius: 1 }}>
                <Link component={RouterLink} to="/mythesis" underline="none" sx={{ color: 'white' }}>
                  <Stack alignItems={'center'} direction={'row'} spacing={1}>
                    <ArticleIcon sx={{ color: 'white' }} />
                    <Typography color="white">Personal Thesis</Typography>
                  </Stack>
                </Link>
              </Box>
              <Box sx={{ '&:hover': { bgcolor: 'primary.dark', m: 'auto', borderRadius: 1 }, p:1, borderRadius: 1 }}>
                <Link component={RouterLink} to="/theses" underline="none" sx={{ color: 'white' }}>
                  <Stack alignItems={'center'} direction={'row'} spacing={1}>
                    <FormatListBulletedIcon sx={{ color: 'white' }} />
                    <Typography color="white">Theses List</Typography>
                  </Stack>
                </Link>
              </Box>
              <Box sx={{ '&:hover': { bgcolor: 'primary.dark', m: 'auto', borderRadius: 1 }, p:1, borderRadius: 1 }}>
                <Link component={RouterLink} to="/usermanagement" sx={{ flexGrow: 1, color: 'white' }} underline="none">
                  <Stack alignItems={'center'} direction={'row'} spacing={1}>
                    <PeopleIcon sx={{ color: 'white' }} />
                    <Typography color="white">User Management</Typography>
                  </Stack>
                </Link>
              </Box>
              <Box sx={{ flexGrow: 1 }} />
              <Stack direction="row" spacing={1} alignItems="center" sx={{ columnGap: 2 }}>
                <Box sx={{ '&:hover': { bgcolor: 'primary.dark', borderRadius: 1 }, p: 1, borderRadius: 1 }}>
                  <Link component={RouterLink} to="/profile" sx={{ color: 'white' }} underline='none'>
                    <Stack direction="row" spacing={1} alignItems="center">
                      <PersonIcon sx={{ color: 'white' }} />
                      <Typography color="white">Profile</Typography>
                    </Stack>
                  </Link>
                </Box>
        
                <Box sx={{ '&:hover': { bgcolor: 'primary.dark', borderRadius: 1 }, p: 1, borderRadius: 1 }}>
                  <Link component={RouterLink} to="/logout" sx={{ color: 'white' }}>
                    <Stack direction="row" spacing={1} alignItems="center">
                      <LogoutIcon sx={{ color: 'white' }}/>
                    </Stack>
                  </Link>
                </Box>
              </Stack>
            </Toolbar>
          </AppBar>
        </Box >
      </ThemeProvider>
      <Outlet />
    </>
  )
}

export default Root
