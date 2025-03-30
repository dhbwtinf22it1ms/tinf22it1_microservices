import Company from "./company";
import OperationalLocation from "./operationalLocation";
import Period from "./period";
import Student from "./student";
import Supervisor from "./supervisor";

interface ThesisDetails {
    id: number;
    topic: string;
    student: Student;
    preparationPeriod: Period;
    partnerCompany: Company;
    operationalLocation: OperationalLocation;
    inCompanySupervisor: Supervisor;
    excludeSupervisorsFromCompanies: string[];
}

export default ThesisDetails;