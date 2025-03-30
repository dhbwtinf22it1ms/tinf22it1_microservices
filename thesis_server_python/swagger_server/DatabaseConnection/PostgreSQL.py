from sqlalchemy.ext.automap import automap_base
from sqlalchemy import create_engine
from ..models.thesis_summary import ThesisSummary
from ..models.thesis import Thesis
from ..models.thesis_student import ThesisStudent
from ..models.thesis_partner_company import ThesisPartnerCompany, ThesisPartnerCompanyAddress
from ..models.thesis_in_company_supervisor import ThesisInCompanySupervisor
from ..models.thesis_preparation_period import ThesisPreparationPeriod
from ..models.thesis_operational_location import ThesisOperationalLocation, ThesisOperationalLocationAddress


Base = automap_base()

# engine, suppose it has two tables 'user' and 'address' set up
engine = create_engine("postgresql+psycopg2://admin:admin@database:5432/ThesisManagement")

# reflect the tables
Base.prepare(autoload_with=engine)

# mapped classes are now created with names by default
# matching that of the table name.
ThesisDB = Base.classes.thesis

def convertToThesisSummary(thesis) -> ThesisSummary:
    return ThesisSummary(
        id=thesis.thesisId,
        title=thesis.topic,
        student_first_name=thesis.studentFirstName,
        student_last_name=thesis.studentLastName,
    )

def convertToThesis(thesis) -> Thesis:
    return Thesis(
        id=thesis.thesisId,
        topic=thesis.topic,
        student=ThesisStudent(
            id=thesis.studentId,
            first_name=thesis.studentFirstName,
            last_name=thesis.studentLastName,
            registration_number=thesis.studentRegistrationNumber,
            course=thesis.studentCourse,
        ),
        partner_company=ThesisPartnerCompany(
            name=thesis.companyName,
            address=ThesisPartnerCompanyAddress(
                street=thesis.companyStreet,
                zip_code=thesis.companyZipcode,
                city=thesis.companyCity,
            )
        ),
        in_company_supervisor=ThesisInCompanySupervisor(
            academic_title=thesis.inCompanySupervisorAcademicTitle,
            title=thesis.inCompanySupervisorTitle,
            first_name=thesis.inCompanySupervisorFirstName,
            last_name=thesis.inCompanySupervisorLastName,
            email=thesis.inCompanySupervisorEmail,
            phone_number=thesis.inCompanySupervisorPhoneNumber,
            academic_degree=thesis.inCompanySupervisorAcademicDegree,
        ),
        preparation_period=ThesisPreparationPeriod(
            _from=thesis.preparationPeriodBegin,
            to=thesis.preparationPeriodEnd,
        ),
        operational_location=ThesisOperationalLocation(
            company_name=thesis.companyName,
            department=thesis.operationalLocationDepartment,
            address=ThesisOperationalLocationAddress(
                street=thesis.operationalLocationStreet,
                zip_code=thesis.operationalLocationZipcode,
                city=thesis.operationalLocationCity,
                country=thesis.operationalLocationCountry
            )
        ),
        exclude_supervisors_from_companies=[thesis.excludedCompanies],
    )

def thesisToDict(thesis: Thesis, ) -> dict:
    return {
        "thesisId": thesis.id,
        "topic": thesis.topic,
        "studentId": thesis.student.id,
        "studentFirstName": thesis.student.first_name,
        "studentLastName": thesis.student.last_name,
        "studentRegistrationNumber": thesis.student.registration_number,
        "studentCourse": thesis.student.course,
        "companyName": thesis.partner_company.name,
        "companyStreet": thesis.partner_company.address.street,
        "companyZipcode": thesis.partner_company.address.zip_code,
        "companyCity": thesis.partner_company.address.city,
        "inCompanySupervisorAcademicTitle": thesis.in_company_supervisor.academic_title,
        "inCompanySupervisorTitle": thesis.in_company_supervisor.title,
        "inCompanySupervisorFirstName": thesis.in_company_supervisor.first_name,
        "inCompanySupervisorLastName": thesis.in_company_supervisor.last_name,
        "inCompanySupervisorEmail": thesis.in_company_supervisor.email,
        "inCompanySupervisorPhoneNumber": thesis.in_company_supervisor.phone_number,
        "inCompanySupervisorAcademicDegree": thesis.in_company_supervisor.academic_degree,
        "preparationPeriodBegin": thesis.preparation_period._from,
        "preparationPeriodEnd": thesis.preparation_period.to,
        "operationalLocationDepartment": thesis.operational_location.department,
        "operationalLocationStreet": thesis.operational_location.address.street,
        "operationalLocationZipcode": thesis.operational_location.address.zip_code,
        "operationalLocationCity": thesis.operational_location.address.city,
        "operationalLocationCountry": thesis.operational_location.address.country,
        "excludedCompanies": thesis.exclude_supervisors_from_companies,
    }

