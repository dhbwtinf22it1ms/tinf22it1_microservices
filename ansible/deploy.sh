#!/bin/bash

# Script to deploy the Dhbw.ThesisManager application using Ansible

set -e

# Check if inventory file is provided
if [ "$#" -ne 1 ]; then
    echo "Usage: $0 <inventory_file>"
    echo "Example: $0 inventory.ini"
    exit 1
fi

INVENTORY_FILE=$1

# Check if inventory file exists
if [ ! -f "$INVENTORY_FILE" ]; then
    echo "Error: Inventory file '$INVENTORY_FILE' not found."
    exit 1
fi

# Check if ansible-playbook is installed
if ! command -v ansible-playbook &> /dev/null; then
    echo "Error: ansible-playbook is not installed. Please install Ansible first."
    exit 1
fi

echo "Deploying Dhbw.ThesisManager application using Ansible..."

# Run the Ansible playbook
ansible-playbook -i "$INVENTORY_FILE" playbook.yml

echo "Deployment completed!"
