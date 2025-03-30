import { 
  Typography, Box,
  Card, CardContent, CardActions, Button, TextField, Divider, 
  Avatar, Container
} from '@mui/material';
import Grid from '@mui/material/Grid2';
import ThumbUpIcon from '@mui/icons-material/ThumbUp';
import ThumbDownIcon from '@mui/icons-material/ThumbDown';
import SendIcon from '@mui/icons-material/Send';
import { useState, } from 'react';

import Thesis from '../interfaces/thesis.tsx';
import ThesisComment from '../interfaces/comment.tsx';

//TODO: Remove mock data and replace with API call to fetch theses
const mockTheses: Thesis[] = [
  {
    id: 1,
    title: "Microservices in der modernen Softwareentwicklung",
    author: "Max Mustermann",
    description: "Eine Analyse über den Einsatz von Microservices-Architekturen in großen Softwareprojekten und deren Vorteile gegenüber monolithischen Ansätzen.",
    likes: 12,
    dislikes: 3,
    comments: [
      { id: 1, author: "Anna Schmidt", text: "Sehr interessantes Thema!", timestamp: "2023-10-15T14:30:00" }
    ]
  },
  {
    id: 2,
    title: "Künstliche Intelligenz im Gesundheitswesen",
    author: "Laura Meyer",
    description: "Einsatzmöglichkeiten von KI-gestützten Systemen zur Verbesserung der Diagnose und Behandlung von Patienten.",
    likes: 8,
    dislikes: 1,
    comments: []
  },
  {
    id: 3,
    title: "Blockchain-Technologie in Supply Chain Management",
    author: "Thomas Müller",
    description: "Untersuchung der Potenziale von Blockchain zur Erhöhung der Transparenz und Effizienz in Lieferketten.",
    likes: 5,
    dislikes: 2,
    comments: [
      { id: 1, author: "Julia Berg", text: "Würde gerne mehr über konkrete Anwendungsfälle erfahren.", timestamp: "2023-10-14T09:15:00" }
    ]
  }
];

function ThesisItem({ thesis, onLike, onDislike, onAddComment }: { thesis: Thesis; onLike: (id: number) => void; onDislike: (id: number) => void; onAddComment: (id: number, comment: ThesisComment) => void }) {
  const [commentText, setCommentText] = useState('');
  const [showComments, setShowComments] = useState(false);
  
  const handleCommentSubmit = () => {
    if (commentText.trim()) {
      onAddComment(thesis.id, {
        id: Date.now(),
        author: "Aktueller Benutzer", // TODO: Replace with actual user data
        text: commentText,
        timestamp: new Date().toISOString()
      });
      setCommentText('');
    }
  };
  
  return (
    <Card variant="outlined" sx={{ mb: 3 }}>
      <CardContent>
        <Typography variant="h5" component="div">
          {thesis.title}
        </Typography>
        <Typography sx={{ mb: 1.5 }} color="text.secondary">
          Von {thesis.author}
        </Typography>
        <Typography variant="body1" sx={{ mb: 2 }}>
          {thesis.description}
        </Typography>
        
        <CardActions sx={{ justifyContent: 'space-between' }}>
          <Box>
            <Button 
              startIcon={<ThumbUpIcon />} 
              onClick={() => onLike(thesis.id)}
            >
              {thesis.likes}
            </Button>
            <Button 
              startIcon={<ThumbDownIcon />} 
              onClick={() => onDislike(thesis.id)}
            >
              {thesis.dislikes}
            </Button>
          </Box>
          
          <Button 
            onClick={() => setShowComments(!showComments)}
          >
            {showComments ? 'Kommentare ausblenden' : `Kommentare anzeigen (${thesis.comments.length})`}
          </Button>
        </CardActions>
        
        {showComments && (
          <>
            <Divider sx={{ my: 2 }} />
            
            <Typography variant="h6">Kommentare</Typography>
            {thesis.comments.length > 0 ? (
              thesis.comments.map(comment => (
                <Box key={comment.id} sx={{ mb: 2, p: 1, bgcolor: 'background.paper', borderRadius: 1 }}>
                  <Grid container spacing={1}>
                    <Grid>
                      <Avatar>{comment.author.charAt(0)}</Avatar>
                    </Grid>
                    <Grid>
                      <Typography variant="subtitle2">{comment.author}</Typography>
                      <Typography variant="body2" color="text.secondary">
                        {new Date(comment.timestamp).toLocaleString()}
                      </Typography>
                      <Typography variant="body1">{comment.text}</Typography>
                    </Grid>
                  </Grid>
                </Box>
              ))
            ) : (
              <Typography variant="body2" color="text.secondary">
                Noch keine Kommentare vorhanden.
              </Typography>
            )}
            
            <Box sx={{ mt: 2, display: 'flex', alignItems: 'flex-start' }}>
              <TextField
                fullWidth
                multiline
                rows={2}
                label="Kommentar hinzufügen"
                value={commentText}
                onChange={(e) => setCommentText(e.target.value)}
                variant="outlined"
                margin="normal"
                sx={{ mr: 1 }}
              />
              <Button 
                variant="contained" 
                onClick={handleCommentSubmit}
                disabled={!commentText.trim()}
                sx={{ mt: 2 }}
                endIcon={<SendIcon />}
              >
                Absenden
              </Button>
            </Box>
          </>
        )}
      </CardContent>
    </Card>
  );
}

function Theses() {
  const [theses, setTheses] = useState<Thesis[]>(mockTheses);

  const handleLike = (thesisId: number) => {
    //TODO: Replace with API call to like a thesis
    setTheses(prevTheses => 
      prevTheses.map(thesis => 
        thesis.id === thesisId ? { ...thesis, likes: thesis.likes + 1 } : thesis
      )
    );
  };
  
  const handleDislike = (thesisId: number) => {
    //TODO: Replace with API call to dislike a thesis 
    setTheses(prevTheses => 
      prevTheses.map(thesis => 
        thesis.id === thesisId ? { ...thesis, dislikes: thesis.dislikes + 1 } : thesis
      )
    );
  };
  
  const handleAddComment = (thesisId: number, comment: ThesisComment) => {
    //TODO: Replace with API call to add a comment to a thesis
    setTheses(prevTheses => 
      prevTheses.map(thesis => 
        thesis.id === thesisId 
          ? { ...thesis, comments: [...thesis.comments, comment] } 
          : thesis
      )
    );
  };

  return (
    <Container maxWidth="md">
      <Box sx={{ my: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Alle Thesen
        </Typography>
        
        {theses.map(thesis => (
          <ThesisItem 
            key={thesis.id}
            thesis={thesis} 
            onLike={handleLike} 
            onDislike={handleDislike}
            onAddComment={handleAddComment}
          />
        ))}

      </Box>
    </Container>
  );
}

export default Theses;
