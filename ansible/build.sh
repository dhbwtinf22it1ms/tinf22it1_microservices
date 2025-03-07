#!/bin/bash

# Script to build the Dhbw.ThesisManager application for deployment

set -e

echo "Building Dhbw.ThesisManager application..."

# Navigate to the dotnet directory
cd src/dotnet

# Build the API project
echo "Building API project..."
dotnet publish Dhbw.ThesisManager/Dhbw.ThesisManager.Api/Dhbw.ThesisManager.Api.csproj -c Release

# Build the NotificationService project
echo "Building NotificationService project..."
dotnet publish Dhbw.ThesisManager/Dhbw.ThesisManager.NotificationService/Dhbw.ThesisManager.NotificationService.csproj -c Release

# Build the Setup project
echo "Building Setup project..."
dotnet publish Dhbw.ThesisManager/Setup/Setup.csproj -c Release

echo "Build completed successfully!"
echo "You can now run the Ansible playbook to deploy the application."
