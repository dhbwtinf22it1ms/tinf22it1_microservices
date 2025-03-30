import { createTheme } from '@mui/material/styles';

// Definieren Sie hier Ihr zentrales Farbschema
const theme = createTheme({
  palette: {
    primary: {
      main: '#0F3057', // Dunkleres Blau für einen akademischen, seriösen Look
      light: '#2D5F87',
      dark: '#0A1C33',
    },
    secondary: {
      main: '#E5B45B', // Warmer Goldton, passend für akademische Anwendungen
      light: '#ECC883',
      dark: '#C69C43',
    },
    // Angepasste Systemfarben
    error: {
      main: '#C62828', // Dunkleres Rot
    },
    warning: {
      main: '#F9A826', // Wärmerer Orangeton
    },
    info: {
      main: '#0277BD', // Zum Primärblau passender Info-Ton
    },
    success: {
      main: '#2E7D39', // Etwas wärmeres Grün
    },
    // Angepasste Hintergrund- und Text-Farben
    background: {
      default: '#F7F8FC', // Leicht bläulicher weißer Hintergrund
      paper: '#FFFFFF',
    },
    text: {
      primary: '#1E272E', // Dunkelgrau statt Schwarz für bessere Lesbarkeit
      secondary: '#546E7A', // Mittleres Blaugrau
    }
  },
  // Optional: Typografie, Abstände, Schatten usw. anpassen

});

export default theme;
