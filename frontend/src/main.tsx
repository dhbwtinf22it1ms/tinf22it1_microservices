
import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter, Navigate, Route, Routes } from "react-router";
import MyThesis from './routes/mythesis.tsx'
import Theses from './routes/theses.tsx'
import Thesis from './routes/thesis.tsx'
import Root from './routes/root.tsx';
import CssBaseline from '@mui/material/CssBaseline';
import UserManagement from './routes/user_management.tsx';
import UserProfile from './routes/user_profile.tsx';

import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';


const root = document.getElementById("root")!;

createRoot(root).render(
  <StrictMode>
    <CssBaseline />
    <BrowserRouter>
      <Routes>
        <Route path="/" element={ <Root /> } >
          {/*
            Later we want the / route to redirect to /mythesis or /theses depending on the role of the current user.
            For now just navigate to /mythesis by default.
          */}
          <Route
            index
            element={ <Navigate to="/mythesis" replace /> }
          />

          <Route path="mythesis" element={ <MyThesis /> } />
          <Route path="theses" element={ <Theses /> } >
            <Route path=":id" element={ <Thesis /> }/>
          </Route>
          <Route path="profile" element={ <UserProfile /> } />
          <Route path="usermanagement" element={ <UserManagement /> } />
        </Route>
      </Routes>
    </BrowserRouter>
  </StrictMode>
)
