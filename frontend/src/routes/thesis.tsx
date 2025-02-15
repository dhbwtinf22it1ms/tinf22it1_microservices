import { useParams } from 'react-router';


function Thesis() {
  let { id } = useParams();

  return (
    <>
      <h1>Thesis {id}</h1>
      This component is used to view and manage a specific thesis.
      It will be used for both the supervisor and students pages.
    </>
  )
}

export default Thesis
