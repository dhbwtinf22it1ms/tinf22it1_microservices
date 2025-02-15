import { Link, useOutlet } from 'react-router';

function Theses() {
  let outlet = useOutlet();

  return (
    <>
      <h1>Theses</h1>
      This page will list all theses for supervisors.
      
      Here are some temporary links to some sub-pages for the thesis with ids 1 to 3:
      <ul>
        <li>
          <Link to="/theses/1">
            Thesis 1
          </Link>
        </li>
        <li>
          <Link to="/theses/2">
            Thesis 2
          </Link>
        </li>
        <li>
          <Link to="/theses/3">
            Thesis 3
          </Link>
        </li>
      </ul>

      {outlet || "No thesis selected"}
    </>
  )
}

export default Theses
