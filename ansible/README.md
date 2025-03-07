# Ansible Deployment for Dhbw.ThesisManager

This directory contains Ansible playbooks and templates to deploy the Dhbw.ThesisManager application on an Ubuntu server.

## Prerequisites

1. An Ubuntu server (tested on Ubuntu 22.04 LTS)
2. Ansible installed on your local machine
3. SSH access to the target server
4. The application source code

## Directory Structure

```
.
├── playbook.yml            # Main Ansible playbook
├── README.md               # This README file
├── build.sh                # Script to build the application
├── deploy.sh               # Script to deploy the application
├── inventory.ini           # Sample inventory file
└── templates/              # Jinja2 templates for configuration files
    ├── api-appsettings.json.j2
    ├── keycloak.service.j2
    ├── nginx-thesismanager.conf.j2
    ├── notification-appsettings.json.j2
    ├── setup-appsettings.json.j2
    ├── thesismanager-api.service.j2
    └── thesismanager-notification.service.j2
```

## Configuration

Before running the playbook, you may want to customize the variables in the `playbook.yml` file:

```yaml
vars:
  app_name: thesismanager
  app_user: thesismanager
  app_group: thesismanager
  app_base_dir: /opt/thesismanager
  dotnet_version: "9.0"
  postgres_user: thesis_manager
  postgres_password: thesis_manager
  postgres_db: thesis_manager
  rabbitmq_user: thesis_manager
  rabbitmq_password: thesis_manager
  keycloak_admin_user: admin
  keycloak_admin_password: admin
  keycloak_version: "26.1"
  keycloak_port: 8080
  smtp_host: localhost
  smtp_port: 25
  notification_email: admin@thesismanager.local
```

## Inventory

Create an inventory file (e.g., `inventory.ini`) with your target server information:

```ini
[thesismanager]
your-server-ip ansible_user=your-ssh-user

[thesismanager:vars]
ansible_ssh_private_key_file=/path/to/your/private/key
```

## Building the Application

Before deploying, you need to build the application

```bash
cddec/don
dotnet publish Dhbw.ThesisManager/Dhbw.ThesisManager.Api/Dhbw.ThesisManager.Api.csproj -c Release
dotnet publish Dhbw.ThesisManager/Dhbw.ThesisManager.NotificationService/Dhbw.ThesisManager.NotificationService.csproj -c Release
dotnet publish Dhbw.ThesisManager/Setup/Setup.csproj -c Release
```

## Running the Playbook

You can use the provided deploy.sh script to run the playbook:

```bash
# Make the script executable (on Linux/macOS)
chmod +x deploy.sh

# Run the deploy script
./deploy.sh inventory.ini
```

Or run the playbook directly with:

```bash
ansible-playbook -i inventory.ini playbook.yml
```

## Post-Installation

After the installation is complete, you can access the application at:

- Main application: http://your-server-ip/
- Keycloak admin console: http://your-server-ip/auth/admin/
- SMTP web interface: http://your-server-ip/smtp/

## Troubleshooting

Check the service status:

```bash
sudo systemctl status thesismanager-api
sudo systemctl status thesismanager-notification
sudo systemctl status keycloak
```

View logs:

```bash
sudo journalctl -u thesismanager-api
sudo journalctl -u thesismanager-notification
sudo journalctl -u keycloak
```

## Security Considerations

For production use, consider:

1. Using strong passwords
2. Enabling SSL/TLS
3. Configuring firewall rules
4. Setting up proper backup procedures
5. Implementing monitoring
