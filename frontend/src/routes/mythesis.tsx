import  Grid from '@mui/material/Grid2';
import { serviceURLs } from '../interfaces/services';

import Thesis from './thesis';
import ThesisFormular from './thesis_formular';

function userHasThesis() {
  // TODO: Replace with actual API call to check if user has a thesis
  fetch(`${serviceURLs.api}/theses/mine`)
    .then(response => {
      if (response.status === 404){
        return false;
      } else {
        return true;
      }
    })
  
  return false; // TODO: Remove this line after implementing the API call
}

function MyThesis() {
  if(userHasThesis()) {
    return (
      <>
        <Grid container>
          <Grid size={2}></Grid>
          <Grid size={8}>
            <h1>Your Thesis</h1>
            <Thesis/>
          </Grid>
          <Grid size={2}></Grid>
        </Grid>
      </>
    )
  } else {
    return (
      <>
        <Grid container>
          <Grid size={2}></Grid>
          <Grid size={8}>
            <h1>Create Your Thesis</h1>
            <ThesisFormular />
          </Grid>
          <Grid size={2}></Grid>
        </Grid>
      </>
    )
  }
}

export default MyThesis
