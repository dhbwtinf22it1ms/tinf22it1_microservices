import smtplib
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart

# SMTP server details
SMTP_SERVER = "localhost"  # Since smtp4dev runs locally
SMTP_PORT = 2525  # Port configured in Docker

# Email details

USERNAME = "admin"
PASSWORD = "secret"
FROM_EMAIL = "test@example.com"
TO_EMAIL = "recipient@example.com"
TO_EMAIL2 = "recipient2@example.com"
SUBJECT = "Test Email via smtp4dev"

class mail_client:
    def __init__(self, smtp_server:str, smtp_port:int):
        self.__smtp_server__ = smtp_server
        self.__smtp_port__ = smtp_port
        
    def send_mail(self, sender:str, receiver:list, subject:str, body:str, usr:str=None, password:str=None):
        # Create email message 
        message = MIMEMultipart()
        message["From"] = sender
        message["To"] = ", ".join(receiver)
        message["Subject"] = subject
        message.attach(MIMEText(body, "plain"))

        # Connect to SMTP server and send email
        try:
            with smtplib.SMTP(self.__smtp_server__, self.__smtp_port__) as server:
                if (usr!=None and password!=None):
                    server.login(USERNAME, PASSWORD)
                server.sendmail(message["From"], message["To"], message.as_string())
            print("Email sent successfully!")
        except Exception as e:
            print("Error", e)
            
if __name__ == "__main__":
    client = mail_client(SMTP_SERVER, SMTP_PORT)
    print("Without credentials")
    client.send_mail(FROM_EMAIL,[TO_EMAIL, TO_EMAIL2], "This is a test", "Hello Microservises!")
    print("Wit credentials")
    client.send_mail(FROM_EMAIL,[TO_EMAIL, TO_EMAIL2], "This is a test", "Hello World! With auth", USERNAME, PASSWORD)