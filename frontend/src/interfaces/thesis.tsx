import ThesisComment from "./comment";

interface Thesis {
    id: number;
    title: string;
    author: string;
    description: string;
    likes: number;
    dislikes: number;
    comments: ThesisComment[];
  }

  export default Thesis;